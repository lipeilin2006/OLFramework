using OLFramework.Data;
using OLFramework.Utils;

namespace OLFramework.Hotfix.Routes
{
	public class JoinRoomRoute : IMessageRoute
	{
		public Message OnMessage(Player player, Message message)
		{
			Logger.Log($"Player request to join room,UID : {message.Values["UID".Hash()]},NetID : {player.NetID},MessageID : {message.MessageID}", LogLevel.Debug);

			if (player.GetProperty("RoomID") != null)
			{
				Logger.Log("Already In Room", LogLevel.Debug);

				return message.ResponseSuccess(new() { { "RoomID".Hash(), (ushort)player.GetProperty("RoomID") } });
			}

			Logger.Log("Join Room", LogLevel.Debug);

			Room? room = (Room?)Lobby.GetProperty("Matching Room");

			Logger.Log("Get Matching Room", LogLevel.Debug);

			if (room == null)
			{
				uint? roomId = Lobby.CreateRoom();
				if (roomId == null) return message.ResponseFailure();

				Logger.Log("Create Room", LogLevel.Debug);

				room = Lobby.GetRoom((uint)roomId);
				Lobby.SetProperty("Matching Room", room);
			}

			room?.Join(player);

			Logger.Log($"Join Room {room?.Id}", LogLevel.Debug);

			player.SetProperty("RoomID", room?.Id);

			return message.ResponseSuccess(new() { { "RoomID".Hash(), room?.Id } });
		}
	}
}