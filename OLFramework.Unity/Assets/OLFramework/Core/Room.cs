using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLFramework.Core
{
	//当前房间，因为同时只能处于一个房间，所以是静态类
	public static class Room
	{
		//当前房间内所有NetworkObject
		public static Dictionary<int, NetworkObject> NetworkObjects { get; private set; } = new Dictionary<int, NetworkObject>();
		public static bool HasObject(int id)
		{
			return NetworkObjects.ContainsKey(id);
		}
		public static void AddObject(int id, GameObject gameObject)
		{
			Debug.Log("ObjectId : " + id);
			if (!NetworkObjects.ContainsKey(id))
			{
				NetworkObjects.Add(id, new NetworkObject(id, gameObject));
			}
		}
		public static NetworkObject GetObject(int id)
		{
			if(NetworkObjects.ContainsKey(id))
				return NetworkObjects[id];
			else 
				return null;
		}
		public static void RemoveObject(int id)
		{
			if(NetworkObjects.ContainsKey(id))
				NetworkObjects.Remove(id);
		}
	}
}
