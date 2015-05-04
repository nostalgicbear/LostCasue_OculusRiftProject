using UnityEngine;
using System.Collections;

public class DeathScript : Photon.MonoBehaviour {

	private Vector3 deathSpawn; //This is the posisiton the player will be moved to when they die
	private Light sunLight; //The games sun, turn off the light when player is dead
	public GameObject playerModel, joints; //Hide player body

	private Canvas inventoryCanvas, constructionCanvas, reticleCanvas, vitalBarCanvas, deathCanvas;
	public GameObject cameraRight;
	private PlayerActions playerActions; //Access point to playerActions
	public Vector3 respawnPoint;
	private InventoryScript playerInv;

	AudioDriver audioDriver;

	
	// Use this for initialization
	void Start () {
		deathSpawn = GameObject.Find ("Death_Room").transform.position;
		respawnPoint = GameObject.Find ("SpawnSpot").transform.position;
		sunLight = GameObject.Find ("Sun").GetComponent<Light> ();
		playerInv = gameObject.GetComponent<InventoryScript> ();
		audioDriver = GetComponent<AudioDriver> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(sunLight == null){
			sunLight = GameObject.Find ("Sun").GetComponent<Light> ();
		}
	}
	
	//TODO:: Delete all items from inventory on death
	//Display respawn text
	//Listen for button press and respawn
	public void playerDead(){
		transform.position = deathSpawn;

		if (sunLight != null) {
			sunLight.enabled = false;
		}
	
		//Set up player actions and canvas's if they haven't been already
		if (playerActions == null) {
			playerActions = cameraRight.GetComponent<PlayerActions>();
			inventoryCanvas = playerActions.inventoryCanvas;
			constructionCanvas = playerActions.inventoryCanvas;
			reticleCanvas = playerActions.reticleCanvas;
			vitalBarCanvas = playerActions.vitalBarCanvas;
			deathCanvas = playerActions.deathCanvas;
		}

		playerInv.resetInventory ();

		playerModel.SetActive (false);
		joints.SetActive(false);
		organiseCanvas (false);
	}

	public void respawnPlayer(){

		transform.position = respawnPoint;

		sunLight.enabled = true;

		if (reticleCanvas != null) {
			reticleCanvas.enabled = true;
		}

		if (deathCanvas != null) {
			deathCanvas.enabled = false;
		}
		
		if (playerActions != null) {
			playerActions.playerDead = false;
		}

		playerModel.SetActive (true);
		joints.SetActive(true);


	}

	//Function to turn on or off all the canvas (wrapped here to save from duplicate code)
	private void organiseCanvas(bool state){

		if (reticleCanvas != null) {
			reticleCanvas.enabled = state;
		}

		if (inventoryCanvas != null) {
			inventoryCanvas.enabled = state;
		}

		if (constructionCanvas != null) {
			constructionCanvas.enabled = state;
		}

		if (vitalBarCanvas != null) {
			vitalBarCanvas.enabled = state;
		}

		if (deathCanvas != null) {
			deathCanvas.enabled = !state;
		}

		if (playerActions != null) {
			playerActions.playerDead = !state;
		}

		if (audioDriver != null) {
			audioDriver.playDeathSound();
		}

		playerActions.inventoryOpen = false;
		playerActions.constructionOpen = false;

	}

	public void updateRespawnLocation(Vector3 pos){
		respawnPoint = pos;
		Debug.Log ("Respawn position updated: " + respawnPoint);
	}
	
}
