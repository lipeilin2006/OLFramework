using OLFramework.Utils;

namespace OLFramework.Data
{
	public class NetworkObject
	{
		public int ID { get; private set; }
		public Dictionary<int, object?> Properties { get; private set; } = new();
		public NetworkObject(int id)
		{
			ID = id;
		}
		public object? GetProperty(string name)
		{
			if (Properties.ContainsKey(name.Hash())) return Properties[name.Hash()];
			return null;
		}
		public void SetProperty(string name, object? value)
		{
			if (Properties.ContainsKey(name.Hash()))
			{
				Properties[name.Hash()] = value;
			}
			else
			{
				Properties.Add(name.Hash(), value);
			}
		}
		public void SetProperty(int nameHash, object? value)
		{
			if (Properties.ContainsKey(nameHash))
			{
				Properties[nameHash] = value;
			}
			else
			{
				Properties.Add(nameHash, value);
			}
		}
	}
}
