using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLFramework.Core
{
	public class NetworkObject
	{
		public int ID;
		public GameObject gameObject;
		public NetworkObject(int id, GameObject gameObject)
		{
			ID = id;
			this.gameObject = gameObject;
		}
	}
}
