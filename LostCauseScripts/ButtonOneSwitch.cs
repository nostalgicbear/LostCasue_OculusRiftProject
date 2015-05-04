using UnityEngine;
using System.Collections;

public class ButtonOneSwitch : Photon.MonoBehaviour {
	
	public GameObject button1;
	private Vector3 startPos;
	private Vector3 endPos;
	private Quaternion endRot;
	private Vector3 temp;
	public float heightIncrease = 5.0f;
	private GameObject player1;
	private GameObject player2;
	private AudioSource audioSource;
	
	void Start()
	{
		startPos = transform.position;
		temp = new Vector3 (transform.position.x, transform.position.y + heightIncrease, transform.position.z);
		endPos = temp;

		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");

		audioSource = GetComponent<AudioSource> ();


	}
	
	// Update is called once per frame
	void Update () {

		if (player1 == null) {
			player1 = GameObject.FindGameObjectWithTag("Player");
		} 

		if (player2 == null) {
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}

		if (transform.position.y >= endPos.y) {
			transform.position = endPos;
		}
		
		if (transform.position.y < startPos.y) {
			transform.position = startPos;
		}

		if (player1 != null && player2 != null) {
			if (Vector3.Distance (button1.transform.position, player1.transform.position) <= 100 || Vector3.Distance (button1.transform.position, player2.transform.position) <= 100) {
				if (button1.GetComponentInChildren<ButtonScript> ().activated == true) {
					photonView.RPC ("ActivateSwitch", PhotonTargets.All, null);
				} else if (!button1.GetComponentInChildren<ButtonScript> ().activated == true) {
					photonView.RPC ("DeactivateSwitch", PhotonTargets.All, null);
				}
			} else {
				//Debug.Log("player not in range");
				return;
			}
		} else {
			return; //player is missing
		}

	}
	
	[RPC]
	public void ActivateSwitch(){
		Vector3 newPos =  new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z);
		transform.position = newPos;

		if (audioSource != null) {
			if (!audioSource.isPlaying) {
				audioSource.Play();
			}
		}

	}
	
	
	[RPC]
	public void DeactivateSwitch()
	{
		Vector3 newPos =  new Vector3 (transform.position.x, transform.position.y - 0.1f, transform.position.z);
		transform.position = newPos;
	}
}
