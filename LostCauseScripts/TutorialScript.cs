using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class TutorialScript : MonoBehaviour {
	//This script is used at the start of the game to introduce the player to the game, the idea is that they are given an instruction and cannot progress until
	// the instruction has been met

	public Canvas tutorialCanvas, backgroundCanvas, reticleCanvas;
	private bool instructionsFollowed = false;
	private bool playerInsideTutorialArea = false;
	private GameObject playerReference;

	public bool x_Button = false;
	public bool lb_Button = false;
	public bool b_Button = false;

	private AudioDriver audioDriver;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (instructionsFollowed && playerReference != null) {
			//Allow the player to move again
			playerReference.GetComponent<CharacterMotor> ().canControl = true;
			if(reticleCanvas != null && !b_Button && !x_Button){
				reticleCanvas.enabled = true;
			}

			if( audioDriver != null){
				audioDriver.playOpenBook();
			}

			Destroy(gameObject);

		}

		if (x_Button) {
			//The bool inventory open allows us to toggle with each press of x button
			if (Input.GetButtonDown ("xbox_X") && playerInsideTutorialArea) {
				instructionsFollowed = true;
			}
		} else if (lb_Button) {
			if (Input.GetButtonDown ("LeftBumper") && playerInsideTutorialArea) {
				instructionsFollowed = true;
			}
		} else if (b_Button) {
			if (Input.GetButtonDown ("xbox_B") && playerInsideTutorialArea) {
				instructionsFollowed = true;
			}
		}
	}

	void OnTriggerEnter(Collider item) {
		if (item.gameObject.tag == "Player" || item.gameObject.tag == "Player2") {
			playerReference = item.gameObject;

			reticleCanvas = playerReference.transform.Find("OVRCameraController/CameraRight/Reticle_Canvas").GetComponent<Canvas>();

			if(audioDriver == null){
				audioDriver = playerReference.GetComponent<AudioDriver>();
			}
			
			if( audioDriver != null){
				audioDriver.playOpenBook();
			}

			if(reticleCanvas != null){
				reticleCanvas.enabled = false;
			}

			tutorialCanvas.enabled = true;
			backgroundCanvas.enabled = true;

			playerInsideTutorialArea = true;

			//Character motor is a javascript file and is being highlighted incorrectly
			playerReference.GetComponent<CharacterMotor> ().canControl = false;
		
			//if(audioDriver == null){
			//	audioDriver = item.gameObject.GetComponent<AudioDriver>();
			//}
			
			//if(playerCount == 0 && audioDriver != null){
			//	audioDriver.playOpenBook();
			//	}

		}
	}
}
