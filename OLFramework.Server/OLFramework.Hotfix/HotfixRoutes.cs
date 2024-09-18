using OLFramework.Data;
using OLFramework.Hotfix.Routes;
using System.Collections.Specialized;

namespace OLFramework.Hotfix
{
    public static class HotfixRoutes
    {
        public static Dictionary<string,IMessageRoute> GetRoutes()
        {
            return new()
            {
                {"JoinRoom", new JoinRoomRoute()},
                {"AddObject", new AddObjectRoute() },
                { "SyncVar", new SyncVarRoute() }
            };
        }
    }
}
