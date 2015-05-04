using UnityEngine;
using System.Collections;

public class OnRaftTest : MonoBehaviour {
	public bool playerOnBoard = false;
	public bool player2OnBoard = false;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			playerOnBoard = true;

		}

		if (other.gameObject.tag == "Player2"){
			player2OnBoard = true;
		}
		//other.transform.parent = gameObject.transform;
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			playerOnBoard = false;
		}
		
		if (other.gameObject.tag == "Player2"){
			player2OnBoard = false;
		}
		//other.transform.parent = null;
	}
}
