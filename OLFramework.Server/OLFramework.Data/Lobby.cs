using System.Collections.Concurrent;
using OLFramework.Utils;

namespace OLFramework.Data
{
	public static class Lobby
	{
		private static Random random = new Random();
		public static ConcurrentDictionary<int, object?> Properties { get; private set; } = new();
		public static ConcurrentDictionary<uint, Room> Rooms { get; private set; } = new();

		private static ConcurrentQueue<uint> reuseRoomID = new ConcurrentQueue<uint>();

		public static object? GetProperty(string name)
		{
			if (Properties.ContainsKey(name.Hash())) return Properties[name.Hash()];
			return null;
		}
		public static void SetProperty(string name,object? value)
		{
			Properties[name.Hash()] = value; 
		}


		public static uint GenerateNewRoomId()
		{
			uint roomID = (uint)random.Next();
			while (true)
			{
				if (Rooms.ContainsKey(roomID))
				{
					roomID = (uint)random.Next();
				}
				else
				{
					break;
				}
			}
			return roomID;
		}

		public static uint? CreateRoom()
		{
			uint roomId;
			if(reuseRoomID.TryDequeue(out roomId))
			{
				if(Rooms.TryAdd(roomId, new Room(roomId)))
				{
                    return roomId;
                }
			}
			else
			{
                roomId = GenerateNewRoomId();
                if(Rooms.TryAdd(roomId, new Room(roomId)))
				{
                    return roomId;
                }
            }
			return null;
		}
		public static Room? GetRoom(uint roomId)
		{
			return Rooms[roomId];
		}
		public static void DeleteRoom(ushort roomId)
		{
			Rooms.Remove(roomId, out _);
			reuseRoomID.Enqueue(roomId);
		}
	}
}
