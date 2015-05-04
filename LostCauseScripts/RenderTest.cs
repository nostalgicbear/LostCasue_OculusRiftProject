using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderTest : MonoBehaviour {

	/**
	 * This scripts is attached to empty game objects that have a large radius that take up an entire terrain each. All
	 * the trees on that terrain are added to an array, and then the trees colliders are only enabled when the player
	 * walks onto that terrain. Enemies will continue to avoid those trees even with their coliders turned off.
	 */ 
	private List<GameObject> trees = new List<GameObject>();
	private List<GameObject> treesInZone = new List<GameObject>();
	private List<GameObject> caves = new List<GameObject> ();
	public GameObject zone;
	private GameObject player1;
	private GameObject player2;
	private bool playersFound = false;

	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");

		foreach (GameObject tree in GameObject.FindGameObjectsWithTag("tree")) {
			if(Vector3.Distance(zone.transform.position, tree.transform.position) < zone.gameObject.GetComponent<SphereCollider>().radius)
			{
				treesInZone.Add(tree);
			}
			tree.GetComponent<CapsuleCollider>().enabled = false;
		}
	
	}

	void FixedUpdate()
	{

		if (player1 != null && player2 != null) {
			playersFound = true;
		}

		if (!playersFound) {
			player1 = GameObject.FindGameObjectWithTag("Player");
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			Debug.Log("Player just entered a zone");
			foreach(GameObject tree in treesInZone)
			{
				tree.GetComponent<CapsuleCollider>().enabled = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			Debug.Log("Player just left a zone");
			foreach(GameObject tree in treesInZone)
			{
				tree.GetComponent<CapsuleCollider>().enabled = false;
			}
		}
	}


}
