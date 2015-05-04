using UnityEngine;
using System.Collections;

public class ButtonTwoSwitches : Photon.MonoBehaviour {

	public GameObject button1;
	public GameObject button2;
	private Vector3 startPos;
	private Vector3 endPos;
	private Vector3 temp;
	public float heightIncrease = 5.0f;
	private GameObject player1;
	private GameObject player2;

	void Start()
	{
		startPos = transform.position;
		temp = new Vector3 (transform.position.x, transform.position.y + heightIncrease, transform.position.z);
		endPos = temp;

		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
	}

	// Update is called once per frame
	void Update () {

		if (player1 == null) {
			player1 = GameObject.FindGameObjectWithTag ("Player");
		} 
		
		if (player2 == null) {
			player2 = GameObject.FindGameObjectWithTag ("Player2");
		}

		if (transform.position.y >= endPos.y) {
			transform.position = endPos;
		}
		
		if (transform.position.y < startPos.y) {
			transform.position = startPos;
		}

		/**
		 * If both buttons are set to true, i.e if someone is starnding on both switches, this object will move upwards.
		 */ 

		if (player1 != null && player2 != null) {
			if (button1.GetComponentInChildren<ButtonScript> ().activated == true && button2.GetComponentInChildren<ButtonScript> ().activated == true) {
				photonView.RPC ("ActivateSwitch", PhotonTargets.All, null);
			} else {
				return;
			}
		} else {
			return;
		}
	}

	[RPC]
	public void ActivateSwitch(){
		Vector3 newPos =  new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z);
		transform.position = newPos;

	}


	[RPC]
	public void DeactivateSwitch()
	{
		Vector3 newPos =  new Vector3 (transform.position.x, transform.position.y - 0.1f, transform.position.z);
		transform.position = newPos;
	}
}
