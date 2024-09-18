using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLFramework.Data
{
	public class Room
	{
		public uint Id;
		public ConcurrentDictionary<int, Player> Players { get; private set; } = new();
		public ConcurrentDictionary<int, NetworkObject> NetworkObjects { get; private set; } = new();
		public ConcurrentDictionary<int, object?> Properties { get; private set; } = new();

		private Random random = new Random();

		public bool Join(Player player)
		{
			if (!Players.ContainsKey(player.NetID))
			{
				while (true)
				{
					if (Players.TryAdd(player.NetID, player)) break;
				}
				return true;
			}
			return false;
		}
		public Room(uint id)
		{
			Id = id;
		}
		private int GenerateObjectID()
		{
			int id = random.Next();
			while (true)
			{
				if (NetworkObjects.ContainsKey(id))
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
		public int? AddObject()
		{
			int? id = GenerateObjectID();
			if (id == null) return null;
			while (true)
			{
				if (NetworkObjects.TryAdd((int)id, new NetworkObject((int)id))) break;
			}
			return id;
		}
		public NetworkObject? GetNetworkObject(int id)
		{
			if (NetworkObjects.ContainsKey(id)) return NetworkObjects[id];
			return null;
		}
		public void RemoveObject(int id)
		{
			while (true)
			{
				if (NetworkObjects.Remove(id, out _))
				{
					break;
				}
			}
		}
	}
}
