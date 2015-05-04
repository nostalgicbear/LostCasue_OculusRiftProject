using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonEnter : Photon.MonoBehaviour {

	public GameObject DungeonParent;
	private List<GameObject> dungeonChildren = new List<GameObject>();
	private Light sun;
	
	
	// Use this for initialization
	void Start () {


		Transform[] allChildren = DungeonParent.GetComponentsInChildren <Transform>();
		
		foreach (Transform child in allChildren) {
			
			dungeonChildren.Add(child.gameObject);
			
		}

		sun = GameObject.FindGameObjectWithTag ("sun").GetComponent<Light> ();

		/*
		foreach (GameObject child in DungeonParent.GetComponentsInChildren<GameObject>()) {
			
			dungeonChildren.Add(child);
			
		}*/
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider other) {
		
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			foreach (GameObject child in dungeonChildren) {
				
				if (child.GetComponent<MeshRenderer> () != null)
					child.GetComponent<MeshRenderer> ().enabled = true;
				
				if (child.GetComponent<MeshCollider> () != null)
					child.GetComponent<MeshCollider> ().enabled = true;
				
			}

		//	sun.enabled = false;
		}
		
	}
}
