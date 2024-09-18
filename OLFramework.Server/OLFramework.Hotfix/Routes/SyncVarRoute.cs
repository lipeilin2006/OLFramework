using OLFramework.Data;
using OLFramework.Utils;
using OLFramework.Core;

namespace OLFramework.Hotfix.Routes
{
	public class SyncVarRoute : IMessageRoute
	{
		public Message OnMessage(Player player, Message message)
		{
			Logger.Log("SyncVar Message", LogLevel.Debug);
			uint? roomId = (uint?)player.GetProperty("RoomID");
			int? objectId = (int?)message.GetValue("ObjectID");
			if (roomId == null) { Logger.Log("RoomId is Null", LogLevel.Error); return message.ResponseFailure(); }
			if (objectId == null) { Logger.Log("ObjectId is Null", LogLevel.Error); return message.ResponseFailure(); }

			Room? room = Lobby.GetRoom((uint)roomId);
			NetworkObject? netObj = room?.GetNetworkObject((int)objectId);
			if (netObj != null)
			{
				foreach (int key in message.Values.Keys)
				{
					netObj.SetProperty(key, message.Values[key]);
				}
			}
			foreach(int netid in room.Players.Keys)
			{
				if (netid != player.NetID)
				{
					OLServer.Send(netid, message);
				}
			}
			return message.ResponseSuccess();
		}
	}
}
