using OLFramework.Utils;
using OLFramework.Data;

namespace OLFramework.Hotfix.Routes
{
	public class AddObjectRoute : IMessageRoute
	{
		public Message OnMessage(Player player, Message message)
		{
			uint? roomId = (uint?)player.GetProperty("RoomID");
			if (roomId == null)
			{
				return message.ResponseFailure();
			}
			else
			{
				int? objId = Lobby.GetRoom((uint)roomId)?.AddObject();
				if (objId == null) return message.ResponseFailure();
				return message.ResponseSuccess(new() { { "ObjectID".Hash(), objId } });
			}
		}
	}
}
