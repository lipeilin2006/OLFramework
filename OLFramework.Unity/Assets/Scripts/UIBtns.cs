using OLFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtns : MonoBehaviour
{
    public Text text;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public async void Match()
    {
        Message response = await OLClient.Send(
            new Message("JoinRoom", MessageType.Request, new Dictionary<int, object>() { { "UID".Hash(), 0 } }), 5000);

        if (response.isSucceed())
        {
            Debug.Log($"Joined room ID : {(uint)response.Values["RoomID".Hash()]}");
            text.text = ((uint)response.Values["RoomID".Hash()]).ToString();
        }
        else
        {
            Debug.Log("Join room failed");
        }
	}
#nullable enable
    public async void AddObject()
    {
        await NetworkManager.Spawn("Cube");
    }
}
