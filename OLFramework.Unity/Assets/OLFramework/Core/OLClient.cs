using System.Collections.Generic;
using UnityEngine;
using kcp2k;
using System;
using MemoryPack;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Timers;
using System.Threading;

namespace OLFramework.Core
{
#nullable enable
	public static class OLClient
	{
		public static List<NetworkIdentify> identifies = new List<NetworkIdentify>();
		/// <summary>
		/// Key:��Ϣid;Value:Message
		/// </summary>
		public static ConcurrentDictionary<int, Message?> ResponseCache = new ConcurrentDictionary<int, Message?>();
		/// <summary>
		/// ��Ϣ����
		/// </summary>
		public static ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();
        /// <summary>
        /// ���ڴ���ע���·��
        /// </summary>
        public static Dictionary<int, IMessageRoute> Routes { get; set; } = new();
		public static string Host { get; set; } = "127.0.0.1";
		public static ushort Port { get; set; } = 12345;

		public static bool isConnected { get { return kcpclient.connected; } }
		public static KcpConfig Config { get; set; } = new KcpConfig(
			NoDelay: true,
			DualMode: false,
			Interval: 10,
			Timeout: 5000,
			SendWindowSize: Kcp.WND_SND * 10000,
			ReceiveWindowSize: Kcp.WND_RCV * 10000,
			CongestionWindow: false,
			MaxRetransmits: Kcp.DEADLINK * 2
		);
		private static KcpClient kcpclient = new(OnConnected, OnData, OnDisconnected, OnError, Config);
        private static bool isStopped = true;
		private static System.Random random = new System.Random();

		private static Thread thread = new(Tick);

		private static Thread[] processThread = new Thread[2];

		private static System.Timers.Timer updateTimer = new System.Timers.Timer();

		private static byte timeoutCount = 0;
        private static void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
			foreach (NetworkIdentify identify in identifies)
			{
				identify.NetworkUpdate();
			}
        }

        /// <summary>
        /// kcp�����tick
        /// </summary>
        private static void Tick()
		{
			while (!isStopped)
			{
				try
				{
					kcpclient.Tick();
				}
				catch
				{
					Debug.Log("Tick Error");
				}
				if (timeoutCount > 3)
				{
					Reconnect();
				}
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		public static void Start()
		{
			updateTimer.Interval = 30;
            updateTimer.Elapsed += UpdateTimer_Elapsed;

            kcpclient.Connect(Host, Port);
			isStopped = false;
			thread.Start();
            updateTimer.Start();

            for (int i = 0; i < processThread.Length; i++)
            {
                processThread[i] = new Thread(ProcessData);
                processThread[i].Start();
            }

            Debug.Log($"Client started");
		}
		/// <summary>
		/// ֹͣ
		/// </summary>
		public static void Stop()
		{
			isStopped = true;
			kcpclient.Disconnect();
			Debug.Log("Client stopped");
		}

		public static void Reconnect()
		{
			kcpclient.Disconnect();
            kcpclient.Connect(Host, Port);
			timeoutCount = 0;
        }
		

		/// <summary>
		/// ����ʱ����
		/// </summary>
		private static void OnConnected()
		{
            Debug.Log("Connected");
		}
		/// <summary>
		/// �յ�����ʱ����
		/// </summary>
		/// <param name="data"></param>
		/// <param name="channel"></param>
		private static void OnData(ArraySegment<byte> data, KcpChannel channel)
		{
			try
			{
				var message = MemoryPackSerializer.Deserialize<Message>(data);
				if (message != null) messages.Enqueue(message);
				Debug.Log("OnData");
			}
			catch
			{

			}
		}

		private static void ProcessData()
		{
			while (!isStopped)
			{
				try
				{

					if (messages.TryDequeue(out var message))
					{
						Debug.Log(message.MessageID);
						if (message.MessageType == MessageType.Request)
						{
							if (message.MessageID == 0)
							{
								Routes[message.Route].OnMessage(message);
								message.Recycle();
							}
							else
							{
								Message resp = Routes[message.Route].OnMessage(message);
								resp.SetID(message.MessageID);
								Send(resp);
								resp.Recycle();
							}
						}
						else
						{
							while (true)
							{
								if (ResponseCache.ContainsKey(message.MessageID))
								{
									break;
								}
								else
								{
									if (ResponseCache.TryAdd(message.MessageID, message))
									{
										break;
									}
								}
							}
						}
					}
				}
				catch
				{

				}
			}
        }

		/// <summary>
		/// ���ӶϿ�ʱ����
		/// </summary>
		private static void OnDisconnected()
		{
			Debug.Log("Disconnected");
		}
		/// <summary>
		/// ����ʱ����
		/// </summary>
		/// <param name="errorCode"></param>
		/// <param name="message"></param>
		private static void OnError(ErrorCode errorCode, string message)
		{
			Debug.Log($"Error:{message}");
		}
		/// <summary>
		/// ���·��
		/// </summary>
		/// <param name="route">·��</param>
		/// <param name="m">·�ɴ���ӿڵ�ʵ��</param>
		public static void AddRoute(string route, IMessageRoute m)
		{
			Routes.Add(route.Hash(), m);
		}
		/// <summary>
		/// ɾ��·��
		/// </summary>
		/// <param name="route">��Ϣ����</param>
		public static void RemoveRoute(string route)
		{
			Routes.Remove(route.Hash());
		}
		/// <summary>
		/// ����Ψһ��Ϣid
		/// </summary>
		/// <returns></returns>
		private static int GenerateMsgID()
		{
			int id = random.Next();
			while (true)
			{
				if (id == 0 || ResponseCache.ContainsKey(id))
				{
					id = random.Next();
				}
				else
				{
					break;
				}
			}
			return id;
		}

		public static void Send(Message message)
		{
            byte[] data = MemoryPackSerializer.Serialize(message);
            kcpclient.Send(data, KcpChannel.Reliable);
        }

		/// <summary>
		/// ����Message
		/// </summary>
		/// <param name="message">��Ϣ������</param>
		/// <param name="timeout">��ʱʱ��</param>
		/// <returns></returns>
		public static async Task<Message?> Send(Message message, int timeout)
		{
			if (!isStopped)
			{
				try
				{
					message.SetID(GenerateMsgID());
					byte[] data = MemoryPackSerializer.Serialize(message);
					kcpclient.Send(data, KcpChannel.Reliable);
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
					Debug.Log("Timeout");
					return null;
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError(e.ToString());
					return null;
				}
			}
			return null;
		}
	}
}