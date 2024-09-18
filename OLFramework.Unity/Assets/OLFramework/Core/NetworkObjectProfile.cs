using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkObjectProfile", menuName = "OLFramework/NetworkObjectProfile", order = 1)]
public class NetworkObjectProfile : ScriptableObject
{
	[SerializeField]
	public NetworkObjectBinding[] networkObjectBindings;
}
