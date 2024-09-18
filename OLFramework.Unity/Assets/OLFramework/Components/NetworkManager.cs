using System.Collections.Generic;
using UnityEngine;
using OLFramework.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OLFramework.Core.Routes;

#nullable enable
//由于unity不允许子线程进行引擎相关操作，因此不得不使用这个组件负责与子线程协调
public class NetworkManager : MonoBehaviour
{
	public NetworkObjectProfile profile;
	public static NetworkObjectProfile Profile { get; private set; }
	//创建NetworkObject的请求队列
	private static ConcurrentQueue<(string, int)> spawnRequests = new();
	//同步变量请求的队列
	private static ConcurrentQueue<(int, Dictionary<int, object>)> syncVarRequests = new();


    public virtual void Awake()
    {
		DontDestroyOnLoad(gameObject);
        Profile = profile;
        RegisterRoutes();
        OLClient.Start();
    }

    /// <summary>
	/// 程序退出时断开连接
	/// </summary>
    public virtual void OnApplicationQuit()
    {
        OLClient.Stop();
    }

    /// <summary>
    /// 注册路由
    /// </summary>
    public virtual void RegisterRoutes()
    {
        OLClient.AddRoute("SyncVar", new SyncVarRoute());
    }

    public virtual void FixedUpdate()
	{
		while (spawnRequests.Count > 0)
		{
			Debug.Log($"spawn req {spawnRequests.Count}");
			(string, int) req;

			if (spawnRequests.TryDequeue(out req))
			{
				string objectType = req.Item1;
				int objectID = req.Item2;
				Debug.Log($"Network Spawn {objectID}");
				if (Room.HasObject(objectID)) continue;

				Debug.Log("Network Spawn");
				GameObject? prefab = null;
				foreach (NetworkObjectBinding binding in profile.networkObjectBindings)
				{
					if (binding.name == objectType)
					{
						Debug.Log("Found");
						prefab = binding.Object;
						break;
					}
				}
				if (prefab == null)
				{
					Debug.Log("Prefab null");
					continue;
				}
				Debug.Log("Ready to spawn");
				GameObject gameObject = GameObject.Instantiate<GameObject>(prefab);
				NetworkIdentify identify = gameObject.GetComponent<NetworkIdentify>();
				OLClient.identifies.Add(identify);

				/*
				Rigidbody rb;
				if(gameObject.TryGetComponent(out rb))
				{
					rb.useGravity = false;
					rb.constraints = RigidbodyConstraints.FreezeAll;
				}

				Rigidbody2D rb2d;
				if (gameObject.TryGetComponent(out rb2d))
				{
					rb2d.gravityScale = 0;
					rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
				}
				*/

				identify.SetObjectID(objectID);
				identify.SetObjectType(objectType);
				identify.isLocal = false;
				Room.AddObject(objectID, gameObject);
				identify.Ready();

				gameObject.SetActive(true);
			}
		}

		while (syncVarRequests.Count > 0)
		{
            Debug.Log($"syncvar req {syncVarRequests.Count}");

			if (syncVarRequests.TryDequeue(out (int, Dictionary<int, object>) req))
			{
				if (Room.HasObject(req.Item1))
				{
					NetworkIdentify identify = Room.GetObject(req.Item1).gameObject.GetComponent<NetworkIdentify>();
					foreach (int key in req.Item2.Keys)
					{
						if (key != "ObjectID".Hash() && key != "ObjectType".Hash())
						{
							Debug.Log("Set");
							identify.NetworkSetVar(key, req.Item2[key]);
						}
					}
					identify.Ready();
				}
			}
		}
	}

	/// <summary>
	/// 本地请求创建NetworkObject
	/// </summary>
	/// <param name="objectType"></param>
	/// <returns></returns>
	public static async Task<GameObject?> Spawn(string objectType)
	{
		Debug.Log("LocalSpawn");
		GameObject? prefab = null;
		foreach (NetworkObjectBinding binding in Profile.networkObjectBindings)
		{
			Debug.Log(binding.name);
			if (binding.name == objectType)
			{
				Debug.Log("Found");
				prefab = binding.Object;
				break;
			}
		}
		if (prefab == null)
		{
			Debug.Log("Prefab null");
			return null;
		}

		int? id = await QueryNewObjectID();

		if (id == null) return null;

		GameObject gameObject = GameObject.Instantiate<GameObject>(prefab);

		NetworkIdentify identify = gameObject.GetComponent<NetworkIdentify>();
		identify.SetObjectID((int)id);
		identify.SetObjectType(objectType);
		identify.isLocal = true;
        OLClient.identifies.Add(identify);

        Room.AddObject(identify.ObjectID, gameObject);
		Debug.Log($"LocalSpawn {identify.ObjectID}");
		identify.Ready();

		gameObject.SetActive(true);
		return gameObject;
	}

	/// <summary>
	/// 远程请求创建NetworkObject，一般用于框架内部
	/// </summary>
	/// <param name="objectType"></param>
	/// <param name="objectID"></param>
	public static void NetworkSpawn(string objectType, int objectID)
	{
		spawnRequests.Enqueue((objectType, objectID));
	}

	/// <summary>
	/// 请求同步变量
	/// </summary>
	/// <param name="objectId"></param>
	/// <param name="vars"></param>
	public static void RequestSyncVar(int objectId, Dictionary<int, object> vars)
	{
		syncVarRequests.Enqueue((objectId, vars));
	}

	/// <summary>
	/// 向服务端请求一个新的唯一NetworkObjectID
	/// </summary>
	/// <returns></returns>
	public static async Task<int?> QueryNewObjectID()
	{
		Message? response = await OLClient.Send(new Message("AddObject", MessageType.Request), 5000);
		if(response == null) return null;
		if (response.isSucceed())
		{
			return (int)response.Values["ObjectID".Hash()];
		}
		return null;
	}
}