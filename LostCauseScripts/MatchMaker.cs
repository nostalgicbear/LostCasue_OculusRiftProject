using UnityEngine;
using System.Collections;

public class MatchMaker : Photon.MonoBehaviour {

	private const string roomName = "room1";
	private Camera[] cameras;
	public GameObject spawnPosition;
	public string playerControllerName;
	public int playerCount = 0;
	private bool gameFull = false;
	
	// Use this for initialization
	void Start () 
	{

		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	/**
	void Update(){

		if (!gameFull) {

			if(playerCount >= 1){
				Debug.Log("Going to call RPC");
				photonView.RPC ("updatePlayerTag", PhotonTargets.AllBuffered, null);
				gameFull = true;

			}

		}
	}
	*/

	void OnGUI () 
	{
		GUILayout.Label("Lost Cause");
		
	}
	
	void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}
	
	
	void OnPhotonRandomJoinFailed()
	{
		Debug.Log("Couldnt join room");
		PhotonNetwork.CreateRoom(null);
	}
	
	void OnJoinedRoom()
	{
		//playerCount = PhotonNetwork.countOfPlayersInRooms;
		playerCount += 1;
		Debug.Log ("PLAYERS IN ROOM" + playerCount);
		GameObject player = PhotonNetwork.Instantiate(playerControllerName, spawnPosition.transform.position, Quaternion.Euler(0,0,0),0);

		if (playerCount == 1) {
			player.gameObject.tag = "Player";
		} else {
			player.gameObject.tag = "Player2";
		}



		/**
		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("I am the master");
		} else {
			Debug.Log("I am not the master");
		}

		if (playerCount == 0 && player.tag != "Player") {
			Debug.Log("Setting tag to Player");
			player.tag = "Player";
		} else if (playerCount == 1 && player.tag != "Player") {
			Debug.Log("Setting tag to Player 2");
			player.tag = "Player2";
		}
		*/

	}
	/**
	[RPC]
	void updatePlayerTag(){

		Debug.Log ("Updating player tag rpc function called");

		if (GameObject.FindWithTag ("Player")) {
				
			if(GameObject.FindWithTag("Player_Untaged")){
				Debug.Log ("I'm player 1 and player untagged found");

				GameObject.FindWithTag("Player_Untaged").tag = "Player2";
				gameFull = true;

			} else {
				Debug.Log ("I'm player 1 and no player untagged");
			}

		} else if (GameObject.FindWithTag ("Player2")) {

			if(GameObject.FindWithTag("Player_Untaged")){
				Debug.Log ("I'm player 2 and player untagged found");

				GameObject.FindWithTag("Player_Untaged").tag = "Player";
				gameFull = true;
			} else {

				Debug.Log ("I'm player 2 and no player untagged");
			}
		}
	}
	*/
	
}
