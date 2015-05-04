using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RaftManager : MonoBehaviour {
	private bool onRaft = false;
	private Image A_Button;
	private GameObject currentRaft;
	private bool boatMoving;
	
	// Use this for initialization
	void Start () {
		A_Button = GameObject.Find ("A_Button").GetComponent<Image>(); //A_Button Icon
		A_Button.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		/*
		 * If you are on a raft, the boatMoving bool refers to that particular raft.
		 */ 
		if (currentRaft != null) {
			boatMoving = currentRaft.GetComponent<RaftMovement>().boatMovingForward;
		}
		
		/*
		 * If you are on the raft and the boat IS NOT moving, if you press A, the raft is set to moving
		 */ 
		if (onRaft && !boatMoving) {
			if (!A_Button.enabled) {
				A_Button.enabled = true;
			}
			
			if (Input.GetButtonDown ("xbox_A") && !GetComponent<InventoryScript>().inventoryOpen) {
				if (currentRaft != null) {
					currentRaft.GetComponent<RaftMovement> ().boatMovingForward = true;
				}
			}
		}
		
		/**
		 * If you are on raft and it is moving, then disable the A button, disable the character controller, and parent 
		 * the player to the raft so it moves with its transform
		 */ 
		if (onRaft && boatMoving) {
			if (A_Button.enabled) {
				A_Button.enabled = false;
			}
		} 	
	}
	/*
	 * This is triggered when hte player phyically collides with tehe raft object. For this to be activated, the player
	 * must be on top of the raft
	 */ 
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "raft") {
			Debug.Log("Colliding with the raft");
			currentRaft = other.gameObject;
			
			if(gameObject.tag == "Player"){
				currentRaft.GetComponent<RaftMovement>().p1OnBoard = true;
			} else if(gameObject.tag == "Player2") {
				currentRaft.GetComponent<RaftMovement>().p2OnBoard = true;
			}
			
			onRaft = true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "raft") {
			
			if(gameObject.tag == "Player"){
				currentRaft.GetComponent<RaftMovement>().p1OnBoard = false;
			} else if(gameObject.tag == "Player2") {
				currentRaft.GetComponent<RaftMovement>().p2OnBoard = false;
				
			}
			
			currentRaft = null;
			onRaft = false;
		}
	}
}
