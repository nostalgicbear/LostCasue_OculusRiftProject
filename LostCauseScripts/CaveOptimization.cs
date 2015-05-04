using UnityEngine;
using System.Collections;

public class CaveOptimization : MonoBehaviour {

	void Start()
	{
		foreach (MeshCollider meshCollider in transform.gameObject.GetComponentsInChildren<MeshCollider>()) {
			if (meshCollider != null) {
				meshCollider.enabled = false;
			}
		}
		
		//	foreach(MeshRenderer meshRenderer in transform.gameObject.GetComponentsInChildren<MeshRenderer>())
		//	{
		//		if(meshRenderer != null)
		//		{
		//			meshRenderer.enabled = false;
		//		}
		//	}
		//}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			foreach(MeshCollider meshCollider in transform.gameObject.GetComponentsInChildren<MeshCollider>())
			{
				if(meshCollider != null)
				{
					meshCollider.enabled = true;
				}
			}

		//	foreach(MeshRenderer meshRenderer in transform.gameObject.GetComponentsInChildren<MeshRenderer>())
		//	{
		//		if(meshRenderer != null)
		//		{
		//			meshRenderer.enabled = true;
		//		}
	//		}
		}
	}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
				foreach(MeshCollider meshCollider in transform.gameObject.GetComponentsInChildren<MeshCollider>())
				{
					if(meshCollider != null)
					{
						meshCollider.enabled = false;
					}
				}
				
			//	foreach(MeshRenderer meshRenderer in transform.gameObject.GetComponentsInChildren<MeshRenderer>())
			//	{
			//		if(meshRenderer != null)
			//		{
			//			meshRenderer.enabled = false;
			//		}
			//	}
			//}
		}
	}
}