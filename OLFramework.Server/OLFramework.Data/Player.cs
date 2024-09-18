using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLFramework.Data
{
	public class Player
	{
		public int NetID;
		public uint Uid;
		public Dictionary<string, object?> Properties { get; set; } = new();
		public Player(int netid)
		{
			NetID = netid;
		}
		public object? GetProperty(string name)
		{
			if(Properties.ContainsKey(name)) return Properties[name];
			return null;
		}
		public void SetProperty(string name, object value)
		{
			Properties.Add(name, value);
		}
	}
}
