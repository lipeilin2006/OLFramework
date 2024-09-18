using kcp2k;
using MemoryPack;
using System.Collections.Concurrent;
using OLFramework.Data;
using OLFramework.Utils;

namespace OLFramework.Core
{
	public static class OLServer
	{
		public static ConcurrentDictionary<int, Message?> ResponseCache = new ConcurrentDictionary<int, Message?>();
		public static ConcurrentDictionary<int, Player> Players { get; set; } = new();

		public static ConcurrentQueue<(int, Message)> messages = new();
		public static Dictionary<int, IMessageRoute> Routes { get; private set; } = new();
		public static ushort Port { get; set; } = 12345;
		private static bool isStop = true;
		public static KcpConfig config { get; set; } = new KcpConfig(
			NoDelay: true,
			DualMode: false,
			Interval: 10,
			Timeout: 5000,
			SendWindowSize: Kcp.WND_SND * 10000,
			ReceiveWindowSize: Kcp.WND_RCV * 10000,
			CongestionWindow: false,
			MaxRetransmits: Kcp.DEADLINK * 2
		);
		private static KcpServer kcpServer = new(OnConnected, OnData, OnDisconnected, OnError, config);
        private static Random random = new Random();

		private static Thread[] processThread = new Thread[8];

		private static Thread thread = new(Tick);

		private static void Tick()
		{
			while (!isStop)
			{
				try
				{
					kcpServer?.Tick();
				}
				catch
				{
					Logger.Log("Tick Error", LogLevel.Error);
				}
			}
		}

		public static void Start()
		{
            kcpServer.Start(Port);
			isStop = false;
			thread.Start();

            for (int i = 0; i < processThread.Length; i++)
            {
                processThread[i] = new Thread(ProcessData);
                processThread[i].Start();
            }

            Logger.Log($"Server started at port : {Port}", LogLevel.Debug);
		}
		public static void Stop()
		{
			isStop = true;
			kcpServer?.Stop();
			Logger.Log($"Server stopped", LogLevel.Debug);
		}
		private static void OnConnected(int netid)
		{
			Logger.Log($"Player connected,NetID : {netid}", LogLevel.Debug);
			while (true)
			{
				if (Players.TryAdd(netid, new Player(netid))) break;
			}
		}
		private static void OnData(int netid,ArraySegment<byte> data,KcpChannel channel)
		{
			try
			{
				var message = MemoryPackSerializer.Deserialize<Message>(data);
				if (message != null)
				{
					messages.Enqueue((netid, message));
				}
				Logger.Log("OnData", LogLevel.Debug);
			}
			catch(Exception e)
			{
				Logger.Log(e.ToString(), LogLevel.Error);
			}
        }
		private static void ProcessData()
		{
			while (!isStop)
			{
				try
				{
					if (messages.TryDequeue(out var v))
					{
						var netid = v.Item1;
						var msg = v.Item2;
						Logger.Log($"Player data received,NetID : {netid},MessageID : {msg?.MessageID}", LogLevel.Debug);
						if (msg != null)
						{
							if (msg.MessageType == MessageType.Request)
							{
								if (msg.MessageID == 0)
								{
									Routes[msg.Route].OnMessage(Players[netid], msg);
								}
								else
								{
									Message resp = Routes[msg.Route].OnMessage(Players[netid], msg);
									resp.SetID(msg.MessageID);
									Send(netid, resp);
									resp.Recycle();
								}
							}
							else
							{
								while (true)
								{
									if (ResponseCache.ContainsKey(msg.MessageID))
									{
										break;
									}
									else
									{
										if (ResponseCache.TryAdd(msg.MessageID, msg))
										{
											break;
										}
									}
								}
							}
						}
					}
				}
				catch(Exception e)
				{
					Logger.Log(e.ToString(), LogLevel.Error);
				}
			}
        }
		private static void OnDisconnected(int netid) 
		{
			Logger.Log($"Player disconnected,NetID : {netid}", LogLevel.Debug);
			while (true)
			{
				if (Players.Remove(netid, out _)) break;
			}
		}
		private static void OnError(int netid,ErrorCode errorCode,string message) 
		{
			Logger.Log($"Player error,NetID : {netid},ErrorCode : {errorCode},Message : {message}", LogLevel.Error);
		}
		public static void AddRoute(string messageType,IMessageRoute route)
		{
			Routes.Add(messageType.Hash(), route);
		}
		public static void RemoveRoute(string messageType)
		{
			Routes.Remove(messageType.Hash());
		}
		private static int GenerateMsgID()
		{
			int id = random.Next();
			while (id == 0 || ResponseCache.ContainsKey(id))
			{
				id = random.Next();
			}
			return id;
		}


		public static void Send(int netid, Message message)
		{
            byte[] data = MemoryPackSerializer.Serialize(message);
            kcpServer?.Send(netid, data, KcpChannel.Reliable);
			Logger.Log($"Send data length {data.Length}", LogLevel.Debug);
			message.Recycle();
        }


		/// <summary>
		/// 发送Message
		/// </summary>
		/// <param name="netid">目标的网络ID</param>
		/// <param name="message">信息</param>
		/// <param name="needToRequest">是否需要等待响应</param>
		/// <param name="timeout">超时</param>
		/// <returns>有响应则返回消息，未响应返回null</returns>
		public static async Task<Message?> Send(int netid,Message message, int timeout)
		{
			try
			{
				message.SetID(GenerateMsgID());
				byte[] data = MemoryPackSerializer.Serialize(message);
				kcpServer?.Send(netid, data, KcpChannel.Reliable);
				Logger.Log($"Send data length {data.Length}", LogLevel.Debug);
				message.Recycle();

				int t = 0;
				while (t < timeout)
				{
					if (ResponseCache.ContainsKey(message.MessageID))
					{
						Message? resp = ResponseCache[message.MessageID];
						while (!ResponseCache.TryRemove(message.MessageID, out resp)) ;
						return resp;
					}
					await Task.Delay(10);
					t += 10;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}
	}
}
