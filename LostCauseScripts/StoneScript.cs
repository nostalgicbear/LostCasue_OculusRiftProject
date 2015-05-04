using UnityEngine;
using System.Collections;

public class StoneScript : MonoBehaviour {
	public float health = 1.0f;
	public int numberOfStonesToSpawn = 1;
	private AudioSource audioSource;

	void Start(){
		audioSource = GetComponent<AudioSource> ();
	}


	[RPC]
	public void destroyRock(){
		if (audioSource != null) {
			if(!audioSource.isPlaying){
				audioSource.Play();
			}
		}
			Destroy (gameObject);
		}

	public void spawnStone(){
		Debug.Log ("Spawning stone");
		Vector3 position = new Vector3(Random.Range(-2.0f, 2.0f), 0 , Random.Range(-2.0f, 2.0f));
		Vector3 spawnWoodPosition = new Vector3(transform.position.x + Random.Range(-1.0f, 1.0f), transform.position.y + 1, transform.position.z + Random.Range(-1.0f, 1.0f));


		for (int i = 0; i < numberOfStonesToSpawn; i++) {
			PhotonNetwork.Instantiate("stone",spawnWoodPosition, Quaternion.identity,0);
		}
	}
}
