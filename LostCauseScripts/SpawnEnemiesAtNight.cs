using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This is used to spawn wolves at night. A check is done to pick a random spawnpoint from a list of spawn points. If the 
 * wold is far enough away from hte player, then the wolf is spawned
 */ 

public class SpawnEnemiesAtNight : MonoBehaviour {
	private List<GameObject> waypoints = new List<GameObject>();
	private bool isNight = false;
	private bool hasSpawned = false;
	private GameObject player, player2;

	// Use this for initialization
	void Start () {
		/*
		 * Find all the waypoints
		 */ 
		foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("waypoint")) {
			waypoints.Add(waypoint);
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * If it is not night time, set the hasSpawned value to false. When this is false, and its night, then the
		 * wolves get spawned at random locations. But we only want this to happen at night.
		 */ 
		if (GameObject.FindGameObjectWithTag ("sun").GetComponent<DayNightCycle> ().isNightTime == false) {
			hasSpawned = false;
		}

		/*
		 * If hasSpawned is false, and its night, then SpawnEnemy() is called which spawns more wolves
		 */ 
		if (!hasSpawned) {
			if (GameObject.FindGameObjectWithTag("sun").GetComponent<DayNightCycle> ().isNightTime == true) {
				SpawnEnemy();
			}
		}
	
	}

	/*
	 * Spawn an enemy at any one of the waypoints that exist in the game;
	 */ 
	[RPC]
	void SpawnEnemy()
	{
		Debug.Log("In spawn enemy function");
		for (int i = 0; i < 5; i ++) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
			Vector3 spawnPosition = waypoints [Random.Range (0, waypoints.Count)].transform.position;
			if(Vector3.Distance(spawnPosition, player.transform.position) >= 150 && Vector3.Distance(spawnPosition, player2.transform.position) >= 150)
			{
				Debug.Log("spawned a new wolf");
				PhotonNetwork.Instantiate ("STANDARD_WOLF", spawnPosition, Quaternion.identity, 0);
			} else {
				Debug.Log("spawn was too close to player");
			}
		}

		hasSpawned = true;
	}
}
