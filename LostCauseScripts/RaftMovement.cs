using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * This script controls the movement of the raft to and from its destination
 */ 

public class RaftMovement : Photon.MonoBehaviour {
	
	private Vector3 endPosition;
	public float smooth;
	private GameObject player;
	private GameObject player2;
	public bool p1OnBoard, p2OnBoard = false;
	private bool player1Enabled, player2Enabled = true;
	public bool boatMovingForward = false;
	private float distanceToStart, distanceToEnd;
	private Vector3 start;
	private List<GameObject> buildAreas = new List<GameObject>();
	private Image A_Button;
	private Vector3 nullTest = new Vector3(0,0,0);
	private List<Transform> piers = new List<Transform>();
	
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
		
		start = transform.position;
		A_Button = GameObject.Find ("A_Button").GetComponent<Image>(); //A_Button Icon
		A_Button.enabled = false;
		
		/**
		 * Add build areas to a list. 
		 */ 
		foreach (GameObject pier in GameObject.FindGameObjectsWithTag("buildArea")) {
			piers.Add(pier.transform);
		}
		PickDestination ();

		//endPosition = transform.parent.GetComponent<RaftArea> ().destination;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		
		if (player2 == null) {
			player2 = GameObject.FindGameObjectWithTag ("Player2");
		}
		
		/*
		 * Store distance to the start point and end point
		 */ 
		distanceToStart = Vector3.Distance (transform.position, start);
		distanceToEnd = Vector3.Distance (transform.position, endPosition);
		
		if (boatMovingForward) {

			/*
			 * If the player is on hte raft, and the raft is moving forward, then disable the players character controller.
			 * This prevents the plauyer from moving ehile the boat is moving. It is re-enabled once the raft has stopped.
			 */ 
			if(p1OnBoard)
			{
				player.GetComponent<CharacterController> ().enabled = false;
				player1Enabled = false;
				player.transform.parent = gameObject.transform;
			} else {
				player.GetComponent<CharacterController> ().enabled = true;
				player1Enabled = true;
				player.transform.parent = null;
			}
			
			if(player2 != null)
			{
				if(p2OnBoard)
				{
					player2.GetComponent<CharacterController> ().enabled = false;
					player2Enabled = false;
					player2.transform.parent = gameObject.transform;
				} else {
					player2.GetComponent<CharacterController> ().enabled = true;
					player2Enabled = true;
					player2.transform.parent = null;
				}
			}
			/*
			 * Calls the MoveBoat RPC. This way the boat moves for all players, not just the one on the boat
			 */ 
			photonView.RPC("MoveBoat", PhotonTargets.All, null);

			} else {
			
			if(player != null && !player1Enabled) {
				player.GetComponent<CharacterController> ().enabled = true;
				player.transform.parent = null;
			}
			
			if(player2 != null && !player2Enabled) {
				player2.GetComponent<CharacterController> ().enabled = true;
				player2.transform.parent = null;
			}
			
			
		}

		/*
			 * If the boat has reached its destination, and it is still moving, stop it moving, and call a function that
			 * swaps the starting point and destination. 
			 */ 
		if (distanceToEnd <=3.0f) {
			if(boatMovingForward)
			{
				photonView.RPC("SwapDirection", PhotonTargets.All, start, endPosition);
				//SwapDirection(start, endPosition);
				boatMovingForward = false;
			}
		}
	} 
	
	/*
	 * sWAPS THE starting point and destination. This is called once a raft reaches its destination. So if a raft goes 
	 * from A to B. The next time it goes from B to A.
	 */ 
	[RPC]
	public void SwapDirection(Vector3 comingFrom, Vector3 goingTo)
	{
		start = goingTo;
		endPosition = comingFrom;

	//	GetClosestArea ();
	//	if (p1OnBoard) {
	//		Vector3 newPos = new Vector3(piers[0].transform.position.x, piers[0].transform.position.y + 3, piers[0].transform.position.z);
	//		player.transform.position = newPos;
	//	}
	}
	
	/*
	 * This is called when the boat is moving. This is when a player is on the boat and presses A
	 */ 
	[RPC]
	public void MoveBoat(){
		transform.position = Vector3.MoveTowards (transform.position, endPosition, 15.0f * Time.deltaTime);
	}
	
	/*
	 * If for some reason a raft gets built and doesnt have a destination, this function is called which returns a list of
	 * all destinations and sorts them from closest to the raft, to furthest away. The closest destination is then assigned
	 */ 
	public void GetClosestArea(){
		piers.Sort (delegate( Transform t1, Transform t2) {
			return Vector3.Distance (t1.position, transform.position).CompareTo (Vector3.Distance (t2.position, transform.position));
		});
	}

	private void PickDestination(){
		GetClosestArea();
		endPosition = piers [0].GetComponent<RaftArea> ().destination.position;
		Debug.Log ("In the PickDestination function and endDestination is" + piers [0].GetComponent<RaftArea> ().destination + " :: and endPos is " + endPosition);
	}
	
	
	
}
