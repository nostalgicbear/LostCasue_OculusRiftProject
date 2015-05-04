using UnityEngine;
using System.Collections;

/*
 * This script is placed on teh tall grass object that players cut to get grass.
 */ 

public class GrassScript : MonoBehaviour {
	public float health = 1.0f;

	/*
	 * When grass has been destroyed, all players call this RPC to remove the grass locally 
	 */ 
	[RPC]
	public void destroyGrass(){
		Destroy (gameObject);
	}

	/*
	 * Only the person who cut that grass then calls theis function, ehich causes a collectable piece of grass to be 
	 * spawned
	 */ 
	public void spawnGrass(){
		Debug.Log ("Spawning collectable grass binding");
		Vector3 spawnWoodPosition = new Vector3(transform.position.x + Random.Range(-1.0f, 1.0f), transform.position.y + 1, transform.position.z + Random.Range(-1.0f, 1.0f));
		PhotonNetwork.Instantiate("grass",spawnWoodPosition, Quaternion.identity,0);
	}
}
