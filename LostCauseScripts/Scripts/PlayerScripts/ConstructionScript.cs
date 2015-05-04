using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ConstructionScript : MonoBehaviour {
	
	//private GameObject highlight; //This is the green highlight indicating selected object
	private Image highlight_image;
	
	private Canvas constructionCanvas, reticleCanvas;
	
	private bool up_released, down_released; //Used to detect when the dpad is released
	private int selectedIndex = 0; //This is the current inventory slot selected
	
	public bool constructionOpen = false; //Stores if the construction screen is open or not
	private bool placementMode = false;
	
	private float xOrigin, yOrigin; //The original y position of the highlight, eg square 0
	private float downwardBuffer = 	187.0f; //This is the distance the highlight sprite needs to be shifted up and down
	
	private GameObject forwardPoint; //Game object placed in front of the players view
	private int rayLength = 20;
	private RaycastHit hit;
	private float height = 1;
	private float distance = 0.0f;
	
	private LayerMask groundLayerMask = 1 << 8; //Only cast raycast against ground layer
	private GameObject placingObject; //Highlight of the item to be placed on the ground
	private string objectToInstantiate; // Name of the item to be constructed
	//Audio driver
	private AudioDriver audioDriver;
	
	
	// Use this for initialization
	void Start () {
		
		highlight_image = GameObject.Find ("Construction_Highlight").GetComponent<Image> ();
		constructionCanvas = GameObject.Find ("Construction_Canvas").GetComponent<Canvas> (); //Construction Canvas
		reticleCanvas = GameObject.Find ("Reticle_Canvas").GetComponent<Canvas> (); //Reticle Canvas
		
		audioDriver = GetComponent<AudioDriver> ();
		forwardPoint = GameObject.Find("forwardPoint");
		
		if (highlight_image != null) {
			//Save the original positions of the highlight, so we can position based off them later
			xOrigin = highlight_image.transform.localPosition.x;
			yOrigin = highlight_image.transform.localPosition.y;
			
			Debug.Log ("y origin: " + yOrigin);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Update highlight position based on index
		if (highlight_image != null) {
			
			highlight_image.transform.localPosition = new Vector3 (xOrigin, yOrigin - (downwardBuffer*selectedIndex + selectedIndex), highlight_image.transform.localPosition.z);
			
		}
		
		Vector3 fwd = forwardPoint.transform.forward;
		
		if (placementMode && placingObject != null && Physics.Raycast (forwardPoint.transform.position, fwd, out hit, rayLength+distance, groundLayerMask)) {
			if (hit.collider.gameObject.tag == "ground") {
				
				//Check to stop placing the object too close to the place to the player (will need to be updated for larger objects)
				if(Vector3.Distance(hit.point, forwardPoint.transform.position) >= 2.0+distance){
					
					placingObject.transform.position = new Vector3(hit.point.x, hit.point.y + height, hit.point.z);
					
					//Rotate the object to lie fluch with the terrain
					Quaternion rot = Quaternion.FromToRotation(placingObject.transform.up, hit.normal);
					placingObject.transform.rotation *= rot;
				}
			}
		}
		
		Debug.DrawRay (forwardPoint.transform.position, fwd*20, Color.green);
		
		
		
		//When A button is pressed call the use function
		if(Input.GetButtonDown("xbox_A") && constructionOpen) {
			
			if(!placementMode){
				if(audioDriver != null){
					audioDriver.playButtonPress();
				}
				
				StartCoroutine(APressAction());
			} else {
				place();
			}
		} 
		
		if(Input.GetButtonDown("xbox_B") && placementMode) {
			
			if(audioDriver != null){
				audioDriver.playButtonPress();
			}
			
			cancel();
		}
		
		
		//--------------------------------------- Listen for dpad input to move selected index ----------------------------------
		if (DPadButtons.up && up_released && constructionOpen) {
			
			if(selectedIndex > 0){
				selectedIndex -= 1;
			} else {
				selectedIndex = 2;
			}
			
			up_released = false;
		}
		
		if (!DPadButtons.up && !up_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			up_released = true;
		}
		
		if (DPadButtons.down && down_released && constructionOpen) {
			if(selectedIndex < 2){
				selectedIndex += 1;
			} else {
				selectedIndex = 0;
			}
			
			down_released = false;
		}
		
		if (!DPadButtons.down && !down_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			down_released = true;
		}
		
	}
	
	//Code run when the A button is pressed (Using coroutine to highlight the button)
	public IEnumerator APressAction(){
		
		//A_Highlight.enabled = true; //Light up A button
		Debug.Log ("A pressed");
		
		if (selectedIndex == 0) {
			placingObject = (GameObject)Instantiate (Resources.Load ("Hut_Outline"));
			placingObject.transform.position = new Vector3(0,-1000,0); //Place out of the scene untill repositioned
			height = 3.5f;
			distance = 4.0f;
			placementMode = true;
			objectToInstantiate = "Hut_Construction";
			
		} else if (selectedIndex == 1) {
			placingObject = (GameObject)Instantiate (Resources.Load ("FirePit_Outline"));
			placingObject.transform.position = new Vector3(0,-1000,0); //Place out of the scene untill repositioned
			height = 0.1f;
			distance = 0.0f;
			placementMode = true;
			objectToInstantiate = "FirePit_Construction";
			
		} else if (selectedIndex == 2){
			
			placingObject = (GameObject)Instantiate (Resources.Load ("Raft_Outline"));
			placingObject.transform.position = new Vector3(0,-1000,0); //Place out of the scene untill repositioned
			height = 0.5f;
			distance = 1.0f;
			placementMode = true;
			objectToInstantiate = "Raft_Construction";
			
			
		}
		
		yield return new WaitForSeconds(0.15f);
		
		GetComponent<CharacterMotor>().canControl = true;
		constructionCanvas.enabled = false;
		reticleCanvas.enabled = false;
		
		//	A_Highlight.enabled = false;
	}
	
	private void place(){
		if(placingObject.GetComponent<PlaceItem>().canPlace){
			if(audioDriver != null){
				audioDriver.playCraftingSound();
			}
			
			reticleCanvas.enabled = true;
			placementMode = false;
			constructionOpen = false;
			
			PhotonNetwork.Instantiate(objectToInstantiate, placingObject.transform.position, placingObject.transform.rotation, 0);
			
			DestroyObject (placingObject);
			placingObject = null;
			
		} else {
			if(audioDriver != null){
				audioDriver.playButtonPress();
			}
		}
	}
	
	private void cancel(){
		reticleCanvas.enabled = true;
		placementMode = false;
		DestroyObject (placingObject);
		placingObject = null;
		
		//Character motor is a javascript file and is being highlighted incorrectly
		GetComponent<CharacterMotor>().canControl = true;
	}
}
