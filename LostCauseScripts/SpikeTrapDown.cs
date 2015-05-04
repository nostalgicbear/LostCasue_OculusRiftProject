using UnityEngine;
using System.Collections;

/*
 * This is placed on whatever tile activates the spike. The spike that drops down can then be added as a public variable
 */ 

public class SpikeTrapDown : Photon.MonoBehaviour {
	public GameObject objectToBeAffected;
	private Vector3 startPos;
	private Vector3 endPos;
	private Vector3 temp;
	public float dropHeight = 15.0f;
	private bool dropSpikes = false;
	private float resetTimer = 0.0f;
	private GameObject player1;
	private GameObject player2;
	private AudioSource spikesAudioSource;

	void Start(){
		startPos = objectToBeAffected.transform.position;
		temp = new Vector3 (objectToBeAffected.transform.position.x, objectToBeAffected.transform.position.y - dropHeight, objectToBeAffected.transform.position.z);
		endPos = temp;

		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");

		spikesAudioSource = objectToBeAffected.GetComponent<AudioSource> ();
	}

	/**
	 * If a player steps on the tile to activate the spike, then an RPC is called that tells all players that the spike is
	 * moving down.
	 */ 
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			dropSpikes = true;
		}
	}

	/**
	 * If a player steps ff the tile to activate the spike, then an RPC is called that tells all players that the spike is
	 * moving back up.
	 */ 
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
		}
	}

	void Update()
	{
		if (player1 == null) {
			player1 = GameObject.FindGameObjectWithTag("Player");
		} 
		
		if (player2 == null) {
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}

		if (player1 != null && player2 != null) {
			if (dropSpikes) {
				photonView.RPC ("SpikesComeDown", PhotonTargets.All, null);
				resetTimer += Time.deltaTime;
				Debug.Log ("rESET TIMER IS " + resetTimer);
				if (resetTimer >= 5) {
					dropSpikes = false;
				}
			} else {
				photonView.RPC("SpikesGoUp", PhotonTargets.All, null);
				resetTimer = 0.0f;
			}
		}

		if (objectToBeAffected.transform.position.y >= startPos.y) {
			objectToBeAffected.transform.position = startPos;
		}

		if (objectToBeAffected.transform.position.y <= endPos.y) {
			objectToBeAffected.transform.position = endPos;
		}

	}

	/*
	 * Move the spike downwards
	 */ 
	[RPC]
	public void SpikesComeDown(){
		Vector3 newPos =  new Vector3 (objectToBeAffected.transform.position.x, objectToBeAffected.transform.position.y - 0.3f, objectToBeAffected.transform.position.z);
		objectToBeAffected.transform.position = newPos;
		spikesAudioSource.Play ();
	}
	
	/*
	 * Move the spike back up
	 */ 
	[RPC]
	public void SpikesGoUp()
	{
		Vector3 newPos =  new Vector3 (objectToBeAffected.transform.position.x, objectToBeAffected.transform.position.y + 0.3f, objectToBeAffected.transform.position.z);
		objectToBeAffected.transform.position = newPos;
	}
}
