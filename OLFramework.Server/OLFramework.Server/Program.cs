using OLFramework.Core;
using OLFramework.Data;
using OLFramework.Utils;
using System.Reflection;


AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

Assembly assembly = Assembly.Load(File.ReadAllBytes("OLFramework.Hotfix.dll"));
Type type = assembly.GetType("OLFramework.Hotfix.HotfixRoutes");
Dictionary<string, IMessageRoute> routes = (Dictionary<string, IMessageRoute>)(type.GetMethod("GetRoutes").Invoke(null, null));
foreach(string route in routes.Keys)
{
    OLServer.AddRoute(route, routes[route]);
}

Logger.Level = LogLevel.Debug;
OLServer.Start();


while (true)
{
	if (Console.ReadLine() == "stop") { OLServer.Stop(); return; }
}


void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    OLServer.Stop();
}