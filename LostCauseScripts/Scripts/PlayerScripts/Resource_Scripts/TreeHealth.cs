using UnityEngine;
using System.Collections;

public class TreeHealth : Photon.MonoBehaviour {
	
	public float health = 1.0f;
	private int choppedHealth = 100;
	private float fallSpeed = 0.3f;
	public bool isChoppedDown = false;
	private AudioSource audioSource;

	void Start(){
		audioSource = GetComponent<AudioSource> ();
	}



	//This function will get called from the player once they have chopped down the tree
	[RPC]
	IEnumerator chopDownTree(){
		if (!isChoppedDown) {
			GetComponent<Rigidbody> ().isKinematic = false;
			GetComponent<Rigidbody>().AddForce(transform.forward * 10);
			GetComponent<Rigidbody> ().mass = 1000;
			isChoppedDown = true;
			audioSource.Play();

			yield return new WaitForSeconds(10);
			Destroy (gameObject);
		}
	}

	void ApplyDamage(int damage)
	{
		if(isChoppedDown)
		{
			Debug.Log("Tree is now chopped down" + choppedHealth);
			choppedHealth-=damage;
		}
		else if(!isChoppedDown)
		{
			health-=damage;
			Debug.Log("the trees health is now :" + health);
		}
	}

	/**
	 * This calls the spawnWood function that spawns wood when a tree has been chopped. The spawnWood function itself is 
	 * an IEnumerator and so it cant be called from a different script. Instead players can call this function which then
	 * calls the spawnWood function.
	 */ 
	public void SpawnWoodCall()
	{
		StartCoroutine("spawnWood");
	}

	IEnumerator spawnWood(){
		Debug.Log ("Spawning wood");
		yield return new WaitForSeconds(5);
		Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), 0 , Random.Range(-1.0f, 1.0f));
		Vector3 spawnWoodPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

		PhotonNetwork.Instantiate("Wood",spawnWoodPosition, Quaternion.identity,0);
	}
	
}
