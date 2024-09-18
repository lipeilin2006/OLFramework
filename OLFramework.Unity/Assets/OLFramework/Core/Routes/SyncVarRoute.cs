using UnityEngine;

namespace OLFramework.Core.Routes {
	//同步变量的路由
#nullable enable
	public class SyncVarRoute : IMessageRoute
	{
		public Message? OnMessage(Message message)
		{
			Debug.Log("SyncVar Recv");
			int? objectId = (int?)message.GetValue("ObjectID");
			string objectType = (string)message.GetValue("ObjectType");
			if (objectId == null) { Debug.Log("ObjectId is Null"); return null; }
			if (objectType == null) { Debug.Log("ObjectType is Null"); return null; }
			Debug.Log($"ObjectId : {objectId}");
			Debug.Log($"ObjectType : {objectType}");

			if (!Room.HasObject((int)objectId))
			{
				NetworkManager.NetworkSpawn(objectType, (int)objectId);
			}
			NetworkManager.RequestSyncVar((int)objectId, message.Values);
			return null;
		}
	}
}
