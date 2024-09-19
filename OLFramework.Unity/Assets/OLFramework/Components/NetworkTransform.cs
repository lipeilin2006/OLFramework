using UnityEngine;
using OLFramework.Core;


[EnableSyncVar]
[RequireComponent(typeof(NetworkIdentify))]
public partial class  NetworkTransform : MonoBehaviour
{
	[Range(0.1f,2f)]
	public float posAccuracy = 0.1f;
    [Range(0.5f, 10f)]
    public float rotAccuracy = 0.5f;
    [Range(0.1f, 2f)]
    public float scaleAccuracy = 0.1f;
    //position
    SyncVar<Vector3> pos;
	//rotation
    SyncVar<Vector3> rot;
	//scale
	SyncVar<Vector3> scale;
	
	void Awake()
    {
		InitSyncVars();
		Debug.Log("InitSyncVar");
	}
	private void InitSyncVars()
	{
		pos = new("pos", GetComponent<NetworkIdentify>(), Setpos);
		rot = new("rot", GetComponent<NetworkIdentify>(), Setrot);
		scale = new("scale", GetComponent<NetworkIdentify>(), Setscale);
    }
	private void Start()
	{
		
	}

	private void Setpos(Vector3 value)
	{
		Debug.Log("Network Set Pos");
		if (value != null)
		{
			transform.position = value;
		}
	}

	private void Setrot(Vector3 value)
	{
		if (value != null)
		{
			transform.eulerAngles = value;
		}
	}

	private void Setscale(Vector3 value)
	{
		if (value != null)
		{
			transform.localScale = value;
		}
	}

	private void FixedUpdate()
	{
		if (GetComponent<NetworkIdentify>().isLocal)
		{
			CheckAndSyncPos();
			CheckAndSyncRot();
			CheckAndSyncScale();
		}
	}

	private void CheckAndSyncPos()
	{
		float distance = posAccuracy;
		Vector3? value = pos.Get();

        if (value != null)
		{
            distance = Vector3.Distance(transform.position, (Vector3)value);
        }
		if (distance >= posAccuracy)
		{
			pos.Set(transform.position);
		}
    }
	private void CheckAndSyncRot()
	{
        float distance = rotAccuracy;
        Vector3? value = rot.Get();

        if (value != null)
		{
			distance = Vector3.Distance(transform.eulerAngles, (Vector3)value);
        }
		if (distance >= rotAccuracy)
		{
			rot.Set(transform.eulerAngles);
		}
    }
    private void CheckAndSyncScale()
    {
        float distance = scaleAccuracy;
        Vector3? value = scale.Get();

        if (value != null)
		{
			distance = Vector3.Distance(transform.localScale, (Vector3)value);
        }
        if (distance >= scaleAccuracy)
        {
            scale.Set(transform.localScale); 
        }
    }

}
