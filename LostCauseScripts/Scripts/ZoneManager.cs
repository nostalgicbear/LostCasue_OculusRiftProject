using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneManager : Photon.MonoBehaviour  {

	public int playersInZone = 0;
	private BoxCollider zoneCollider;
	private List<GameObject> treesInZone = new List<GameObject>();	
	private List<GameObject> totalTrees = new List<GameObject>();
	private List<GameObject> totalStone = new List<GameObject>();
	private List<GameObject> totalGrass = new List<GameObject>();
	private List<GameObject> rocksInZone = new List<GameObject>();
	private List<GameObject> grassInZone = new List<GameObject>();
	private List<GameObject> waterBarriers = new List<GameObject>();
	private Vector3 playerPosition;
	private int treeCount = 0;
	private int rockCount = 0;
	private int grassCount = 0;

	// Use this for initialization
	void Start () {

		zoneCollider = GetComponent<BoxCollider> ();

		foreach (GameObject tree in GameObject.FindGameObjectsWithTag("tree")) {
			if(zoneCollider.bounds.Contains(tree.transform.position)){
				treesInZone.Add(tree);
				treeCount ++;
			}
		}

		foreach (GameObject rock in GameObject.FindGameObjectsWithTag("rock")) {
			if(zoneCollider.bounds.Contains(rock.transform.position)){
				rocksInZone.Add(rock);
				rockCount ++;
			}
		}

		foreach (GameObject grass in GameObject.FindGameObjectsWithTag("tallgrass")) {
			if(zoneCollider.bounds.Contains(grass.transform.position)){
				rocksInZone.Add(grass);
				grassCount ++;
			}
		}

	//	Debug.Log ("Tree Count is: " + treeCount + " Rock Count is: " + rockCount + " Grass Count is: " + grassCount);

		foreach (GameObject waterBarrier in GameObject.FindGameObjectsWithTag("waterBarrier")) {
			if(zoneCollider.bounds.Contains(waterBarrier.transform.position)){
				waterBarriers.Add(waterBarrier);
			}
		}

		
		foreach (GameObject waterBarrier in GameObject.FindGameObjectsWithTag("freshWater")) {
			if(zoneCollider.bounds.Contains(waterBarrier.transform.position)){
				waterBarriers.Add(waterBarrier);
			}
		}

		foreach (GameObject tree in GameObject.FindGameObjectsWithTag("tree")) {
			totalTrees.Add(tree);
		}

		foreach (GameObject grass in GameObject.FindGameObjectsWithTag("tallgrass")) {
			totalGrass.Add(grass);
		}

		foreach (GameObject rock in GameObject.FindGameObjectsWithTag("rock")) {
			totalStone.Add(rock);
		}

		Debug.Log ("T:" + totalTrees.Count + ": s " + totalStone.Count + " :g :" + totalGrass.Count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			Debug.Log("Player just entered a zone");
			photonView.RPC("playerEnteredZone", PhotonTargets.AllBuffered, null);
		}


	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			Debug.Log("Player just left a zone");
			playerPosition = other.gameObject.transform.position;
			photonView.RPC("playerExitedZone", PhotonTargets.AllBuffered, null);
		}
	}

	[RPC]
	void playerEnteredZone(){
		if (playersInZone < 2) {
			playersInZone += 1;
		}

		if (playersInZone == 1) {

			foreach (GameObject tree in treesInZone) {
				tree.GetComponent<CapsuleCollider>().enabled = true;
			}

			foreach (GameObject rock in rocksInZone) {
				rock.GetComponent<MeshCollider>().enabled = true;
			}

			foreach (GameObject grass in grassInZone) {
				grass.GetComponent<BoxCollider>().enabled = true;
			}

			foreach (GameObject waterBarrier in waterBarriers) {
				waterBarrier.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	[RPC]
	void playerExitedZone(){
		if (playersInZone > 0) {
			playersInZone -= 1;
		}

		if (playersInZone == 0) {
			foreach (GameObject tree in treesInZone) {
				if(Vector3.Distance(playerPosition, tree.transform.position) > 50.0f){
					tree.GetComponent<CapsuleCollider>().enabled = false;
				}
			}

			foreach (GameObject rock in rocksInZone) {
				if(Vector3.Distance(playerPosition, rock.transform.position) > 50.0f){
					rock.GetComponent<MeshCollider>().enabled = false;
				}
			}

			foreach (GameObject grass in grassInZone) {
				if(Vector3.Distance(playerPosition, grass.transform.position) > 50.0f){
					grass.GetComponent<BoxCollider>().enabled = false;
				}
			}

			foreach (GameObject waterBarrier in waterBarriers) {
				if(Vector3.Distance(playerPosition, waterBarrier.transform.position) > 50.0f){
					waterBarrier.GetComponent<BoxCollider>().enabled = false;
				}
			}
		}
	}




}
