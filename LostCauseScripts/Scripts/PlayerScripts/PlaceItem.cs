using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceItem : MonoBehaviour {

	public bool canPlace, isRaft, inRaftArea = false; //Specific bools to accomodate for placing the raft in a specified zone
	private string colour = "red";

	private int rayLength = 1;
	private RaycastHit hit;

	public List<GameObject> anchors = new List<GameObject> ();
	private List<GameObject> currentlyColliding = new List<GameObject>();

	public Component[] renderers; //Hold child renderers to change colour

	private Color green = new Color(0, 1, 0, 0.4f);
	private Color red = new Color(1, 0, 0, 0.4f);

	// Use this for initialization
	void Start () {
		//GetComponent<Renderer> ().material.color = Color.red;

		renderers = GetComponentsInChildren<Renderer>();
		//Debug.Log ("Getting components to turn red");
		
		foreach (Renderer render in renderers) {
			render.material.color = Color.red;
		}
		
		GetComponent<Collider> ().isTrigger = true;
	}

	
	// Update is called once per frame
	void Update () {

		//If the object isn't coliding with any objects 
		if (!isRaft) {
			if (currentlyColliding.Count <= 0 && !canPlace) {
				canPlace = true;

			} else if (currentlyColliding.Count > 0 && canPlace) {
				canPlace = false;
			}
		} else {
			if (currentlyColliding.Count <= 1 && !canPlace && inRaftArea) {
				canPlace = true;
				
			} else if (currentlyColliding.Count >= 0 && canPlace && !inRaftArea) {
				canPlace = false;
			}
		}


		if (canPlace && colour == "red") {

			foreach (Renderer render in renderers) {
				render.material.color = green;
			}
			
			colour = "green";

		} else if (!canPlace && colour == "green") {
					
			foreach (Renderer render in renderers) {
				render.material.color = red;
			}

			colour = "red";
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag != "Ground") {
			currentlyColliding.Add (other.gameObject);
		}

		if (isRaft && other.tag == "raftArea") {
			inRaftArea = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag != "Ground") {
			currentlyColliding.Remove(other.gameObject);
		}

		if (isRaft && other.tag == "raftArea") {
			inRaftArea = false;
		}
	}
}
