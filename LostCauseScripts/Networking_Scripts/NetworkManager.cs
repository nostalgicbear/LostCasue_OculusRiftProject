using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {
	const string VERSION = "v0.0.1";
	public string roomName = "LostCause";
	public string playerPrefabName = "Player_New_GUI_Network";
	public Transform spawnPoint;
	private float players = 0;
	
	// Use this for initialization
	void Awake () { //Remember to change back to start()
		PhotonNetwork.ConnectUsingSettings(VERSION);
		
	}
	
	void OnJoinedLobby()
	{
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2};
		PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
	}
	
	void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
		players+=1;
		
		
	}
	
	void Update()
	{
		Debug.Log("There are now " + players + " in the game");
	}
	
}
