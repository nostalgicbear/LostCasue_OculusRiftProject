using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonExit : MonoBehaviour {

	public GameObject DungeonParent;
	private List<GameObject> dungeonChildren = new List<GameObject>();
	private Light sun;


	// Use this for initialization
	void Start () {

		sun = GameObject.FindGameObjectWithTag ("sun").GetComponent<Light> ();

		Transform[] allChildren =  DungeonParent.GetComponentsInChildren <Transform>();

		foreach (Transform child in allChildren) {

		//	Debug.Log("Child name: " + child.name + " added to arraay");

			dungeonChildren.Add(child.gameObject);
			
			if (child.gameObject.GetComponent<MeshRenderer> () != null){
				child.gameObject.GetComponent<MeshRenderer> ().enabled = false;
			} else {
		//		Debug.Log("Cannot find child mesh renderer");
			}
			
			if (child.gameObject.GetComponent<MeshCollider> () != null)
				child.gameObject.GetComponent<MeshCollider> ().enabled = false;

		}

		/*
		foreach (GameObject child in DungeonParent.GetComponentsInChildren<GameObject>()) {

			dungeonChildren.Add(child);

			if (child.GetComponent<MeshRenderer> () != null)
				child.GetComponent<MeshRenderer> ().enabled = false;
			
			if (child.GetComponent<MeshCollider> () != null)
				child.GetComponent<MeshCollider> ().enabled = false;

		}*/

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			foreach (GameObject child in dungeonChildren) {

				if (child.GetComponent<MeshRenderer> () != null)
					child.GetComponent<MeshRenderer> ().enabled = false;

				if (child.GetComponent<MeshCollider> () != null)
					child.GetComponent<MeshCollider> ().enabled = false;
				
			}

			sun.enabled = true;
		}
		
	}

}
