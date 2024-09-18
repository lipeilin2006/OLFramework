using OLFramework.Core;
using System.Collections.Generic;
using UnityEngine;

//作为Networkobject的标识，在需要同步的GameObject上挂在
public class NetworkIdentify : MonoBehaviour
{
	//NetworkObject的唯一标识
    public int ObjectID { get; private set; } = 0;
	//NetworkObject的类型
	public string ObjectType { get; private set; }
	//本地所属标识
	public bool isLocal { get; set; }
	//储存该NetworkObject的所有组件中的SyncVar
	public Dictionary<int, SyncVar> SyncVars { get; private set; } = new Dictionary<int, SyncVar>();
	//是否有同步变量改变
	public bool isChanged { get; private set; } = false;
	//是否准备完毕，准备完毕即可开始同步
	public bool isReady { get; private set; } = false;
	// Start is called before the first frame update
	private void Awake()
	{

	}

    //每个网络帧执行的函数
    public void NetworkUpdate()
	{
		if (isChanged && isReady)
		{
			//同步变量
			Message msg = Message.Take("SyncVar", MessageType.Request, GetObjectDict());
			OLClient.Send(msg);
			msg.Recycle();
			isChanged = false;
		}
    }
	//添加同步变量
	public void AddSyncVar(int nameHash,SyncVar syncVar)
	{
		if(!SyncVars.ContainsKey(nameHash))
			SyncVars.Add(nameHash, syncVar);
	}
	//从网络向本地同步变量，不会使isChanged改变
	public void NetworkSetVar(int nameHash,object value)
	{
        if (SyncVars.ContainsKey(nameHash))
        {
			SyncVars[nameHash].Set(value, SyncDirection.RemoteToLocal);
		}
	}
	//将状态标记为Changed，在下一个网络帧进行变量同步（如果已就绪）
	public void Changed()
	{
		Debug.Log("Changed");
		isChanged = true;
	}
	//将NetworkObject的状态标记为就绪
	public void Ready()
	{
		isReady = true;
	}
	//设置Networkobject的ID
	public void SetObjectID(int objectId)
	{
		ObjectID = objectId;
	}
	//设置Networkobject的类型
	public void SetObjectType(string type)
	{
		ObjectType = type;
	}
	//转换类型，用于变量同步，Dictionary<int,object>即为向服务端发送的变量表
	private Dictionary<int,object> GetObjectDict()
	{
		Dictionary<int, object> value = new Dictionary<int, object>();
		foreach (var key in SyncVars.Keys)
		{
			//判断变量是否改变，只用改变了的变量才需要同步，以此减小网络包体
			if (SyncVars[key].isChanged)
			{
				value.Add(key, SyncVars[key].GetObject());
				SyncVars[key].isChanged = false;
			}
		}

		if(!value.ContainsKey("ObjectID".Hash()))
			value.Add("ObjectID".Hash(), ObjectID);
		if(!value.ContainsKey("ObjectType".Hash()))
			value.Add("ObjectType".Hash(), ObjectType);

		return value;
	}
}
