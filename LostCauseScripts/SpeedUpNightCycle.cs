using UnityEngine;
using System.Collections;

public class SpeedUpNightCycle : Photon.MonoBehaviour {
	public int playersInside; //How many players are in a given hut. If this is 2 and its night, the cycle speeds up
	private bool speedIncreased = false;
	private int changeCount = 0;

	void Update(){
		if (!speedIncreased && playersInside >= 2) {
			GameObject.FindGameObjectWithTag ("sun").GetComponent<DayNightCycle> ().increaseNightSpeed = true;
			speedIncreased = true;
		} else if (speedIncreased && playersInside <= 2) {
			GameObject.FindGameObjectWithTag("sun").GetComponent<DayNightCycle>().increaseNightSpeed = false;
			speedIncreased = false;
		}
	}

	/*
	 * If a player walks into a hut, increase the amount of people in the hut by 1. If 2 players are in the same hut,
	 * then send a message to the Sun script to tell it to speed up the rate at which night passes. That script will 
	 * only speed this up if its night time
	 */ 
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			other.gameObject.GetComponent<DeathScript>().updateRespawnLocation(transform.position);
			changeCount = 1;
			photonView.RPC("ChangeCount", PhotonTargets.All, changeCount);
		}
	}

	/*
	 * If someone leaves the hut, then decrease the amount of people in the hut by 1
	 */
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			changeCount = -1;
			photonView.RPC("ChangeCount", PhotonTargets.All, changeCount);
		}
	}

	[RPC]
	public void ChangeCount(int playersIn)
	{
		Debug.Log("RPC called. Players inside:  " + playersInside );
		playersInside += playersIn;
	}
}
