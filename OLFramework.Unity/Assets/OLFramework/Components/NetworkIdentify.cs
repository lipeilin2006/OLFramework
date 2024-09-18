using OLFramework.Core;
using System.Collections.Generic;
using UnityEngine;

//��ΪNetworkobject�ı�ʶ������Ҫͬ����GameObject�Ϲ���
public class NetworkIdentify : MonoBehaviour
{
	//NetworkObject��Ψһ��ʶ
    public int ObjectID { get; private set; } = 0;
	//NetworkObject������
	public string ObjectType { get; private set; }
	//����������ʶ
	public bool isLocal { get; set; }
	//�����NetworkObject����������е�SyncVar
	public Dictionary<int, SyncVar> SyncVars { get; private set; } = new Dictionary<int, SyncVar>();
	//�Ƿ���ͬ�������ı�
	public bool isChanged { get; private set; } = false;
	//�Ƿ�׼����ϣ�׼����ϼ��ɿ�ʼͬ��
	public bool isReady { get; private set; } = false;
	// Start is called before the first frame update
	private void Awake()
	{

	}

    //ÿ������ִ֡�еĺ���
    public void NetworkUpdate()
	{
		if (isChanged && isReady)
		{
			//ͬ������
			Message msg = Message.Take("SyncVar", MessageType.Request, GetObjectDict());
			OLClient.Send(msg);
			msg.Recycle();
			isChanged = false;
		}
    }
	//���ͬ������
	public void AddSyncVar(int nameHash,SyncVar syncVar)
	{
		if(!SyncVars.ContainsKey(nameHash))
			SyncVars.Add(nameHash, syncVar);
	}
	//�������򱾵�ͬ������������ʹisChanged�ı�
	public void NetworkSetVar(int nameHash,object value)
	{
        if (SyncVars.ContainsKey(nameHash))
        {
			SyncVars[nameHash].Set(value, SyncDirection.RemoteToLocal);
		}
	}
	//��״̬���ΪChanged������һ������֡���б���ͬ��������Ѿ�����
	public void Changed()
	{
		Debug.Log("Changed");
		isChanged = true;
	}
	//��NetworkObject��״̬���Ϊ����
	public void Ready()
	{
		isReady = true;
	}
	//����Networkobject��ID
	public void SetObjectID(int objectId)
	{
		ObjectID = objectId;
	}
	//����Networkobject������
	public void SetObjectType(string type)
	{
		ObjectType = type;
	}
	//ת�����ͣ����ڱ���ͬ����Dictionary<int,object>��Ϊ�����˷��͵ı�����
	private Dictionary<int,object> GetObjectDict()
	{
		Dictionary<int, object> value = new Dictionary<int, object>();
		foreach (var key in SyncVars.Keys)
		{
			//�жϱ����Ƿ�ı䣬ֻ�øı��˵ı�������Ҫͬ�����Դ˼�С�������
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
