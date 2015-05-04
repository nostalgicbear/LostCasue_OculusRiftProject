using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerActions : MonoBehaviour {
	
	private float damage;
	private float progressModifier; //This is the rate at which resources can be depleted, varies for tree, rocks and grass
	private float distanceBetween;
	private bool isAxisInUse = false;
	public bool playerDead = false , inventoryOpen = false, constructionOpen = false; //track wheter the player is alive or not
	private int rayLength = 3;
	private RaycastHit hit;
	private string lastAttackAnimation = "";
	
	public GameObject parentObj;

	//UI Canvas's
	public Canvas inventoryCanvas, constructionCanvas, reticleCanvas, vitalBarCanvas, deathCanvas;
	
	//Button Iconts
	private Image RT_Button, A_Button;
	
	//private UILabel toolTipText;
	
	private VitalBarDriver vitalDriver; //Allows us to set hunger, thirst, stamina and progress bar
	
	//Audio driver
	private AudioDriver audioDriver;
	
	//Player Animator
	private Animator anim;
	private string action = "";
	private string currentTool = "";
	private float playerDamage = 0.0f;
	
	private InventoryScript playerInv;
	private ConstructionScript playerConstruction;
	
	
	//TODO:: Reimplement or remove tooltips
	
	
	void Awake() {
		
		
		
	}
	
	void Start() {
		
		//	toolTipScreen = GameObject.Find ("ToolTipScreen");
		
		vitalDriver = parentObj.GetComponent<VitalBarDriver> ();
		
		audioDriver = parentObj.GetComponent<AudioDriver> ();
		
		playerInv = parentObj.GetComponent<InventoryScript> ();

		playerConstruction = parentObj.GetComponent<ConstructionScript> ();
		//toolTipBackground = GameObject.Find("Tooltip_Background");
		
		//toolTipText = GameObject.Find ("Tooltip_Text").GetComponent<UILabel> ();
		
		//Find UI Canvas's
		inventoryCanvas = GameObject.Find ("Inventory_Canvas").GetComponent<Canvas> (); //Inventory Canvas
		inventoryCanvas.enabled = false; 

		constructionCanvas = GameObject.Find ("Construction_Canvas").GetComponent<Canvas> (); //Construction Canvas

		reticleCanvas = GameObject.Find ("Reticle_Canvas").GetComponent<Canvas> (); //Reticle Canvas
		deathCanvas = GameObject.Find ("Death_Canvas").GetComponent<Canvas> (); //Reticle Canvas
		vitalBarCanvas = GameObject.Find ("VitalBar_Canvas").GetComponent<Canvas> (); //VitalBar Canvas
		vitalBarCanvas.enabled = false; 
		
		RT_Button = GameObject.Find ("RT_Button").GetComponent<Image>(); //RT_Button Icon
		A_Button = GameObject.Find ("A_Button").GetComponent<Image>(); //A_Button Icon
		A_Button.enabled = false;

		anim = GameObject.Find ("Player_Rig").GetComponent<Animator> ();
		
		damage = 0.0f;
		progressModifier = 1.0f;
		
	}
	
	// Update is called once per frame
	void Update() {
		
		/*If the fireButton is pressed,a raycast is sent out front the front of the player. The raycast is like an invisible
		 * straight line. If this line hits an object, (ie if something is in front of the player), it then checks to
		 * see what that object is. It does this by checking the gameobjects tag. 
		 */
		
		Vector3 fwd = transform.forward;
		
		if (Physics.Raycast (transform.position, fwd, out hit, rayLength)) {
			/*If the raycast hits a tree, the player is within range of the tree. The damage value is then
			* sent to the ApplyDamage function which is in a scipt attached to the tree. 
			*/
			string targetTag = hit.collider.gameObject.tag;
			
			if(playerInv != null){
				if(playerInv.toolEquipped != null){
					currentTool = playerInv.toolEquipped.tag;
				}
			}
			
			//Check if we are looking at something we can mine for resources (make sure there is a tool equiped and we have the right one)
			if ((currentTool != "" && ((targetTag == "tree" && currentTool == "axe") || (targetTag == "rock" && currentTool == "hammer") || (targetTag == "tallgrass"))) || (targetTag == "tallgrass")) {
				float targetHealth = 0.0f; //By getting the resources health we can determine when it is fully depleted
				
				if (targetTag == "tree") {
					targetHealth = hit.collider.gameObject.GetComponent<TreeHealth> ().health;
					action = "chopping";
					progressModifier = 0.5f; //Rate at which trees chop
					
					//Sets tooltip text to the tree message (this may need refactoring for extensibility)
					//if(toolTipText.text == " "){
					//	toolTipText.text = "Hmm... \nlooks like a tree";
					//}
					
				} else if (targetTag == "rock") { // Add these when added to the game
					targetHealth = hit.collider.gameObject.GetComponent<StoneScript>().health;
					action = "mining";
					progressModifier = 0.3f; //Rate at which rock is mined
					
					
				} else if (targetTag == "tallgrass") {
					targetHealth = hit.collider.gameObject.GetComponent<GrassScript>().health;
					action = "action";
					progressModifier = 1.0f; //Rate at which grass is picked
					
				}
				
				//If the resource hasn't been destroyed
				if (targetHealth != 0.0f) {
					
					//Draw right trigger icon on screen
					if (!RT_Button.enabled) {
						RT_Button.enabled = true;
					}
					
					//When the right trigger is held
					if (Input.GetAxis ("RightTrigger") > 0.001 && targetHealth != 0.0f) {
						if (vitalDriver != null && damage <= 1.0f && vitalDriver.GetEnergy () > 0.0f) { //When the damage to the resource is less than 100% (1.0f) increase the damage and reticle by 1%

							if(targetTag == "tree")
							{
								audioDriver.playWoodChopSound();
							}


							 if(targetTag == "rock"){
								Debug.Log("Looking at a rock waiting for sound");
								audioDriver.playMineSound();
							} 


							if(targetTag == "tallgrass"){
								audioDriver.playPullGrassSound();
							}

							if (vitalDriver.resting) {
								vitalDriver.resting = false; //Stop the player resting and regaining stamina while mining resource
							}
							
							damage += (0.01f * progressModifier); //Increase damage done
							vitalDriver.SetReticleProgress (damage);
							vitalDriver.updateEnergy ((-0.002f * progressModifier));
							anim.SetFloat(action,damage);
							
						} else if (vitalDriver != null && vitalDriver.GetEnergy () <= 0.0f) {
							vitalDriver.resting = true; 
							damage = 0.0f; //Reset damage
							vitalDriver.SetReticleProgress (damage);
							
						} else if (damage >= 1.0f) { //When the damage totals 100% (1.0f) send a message to the tree calling the function to chp it down
							
							if (!vitalDriver.resting) {
								vitalDriver.resting = true;
							}
							
							damage = 1.0f;
							
							//Here we need to check again what type of resource we are mining to call the right functions
							if (targetTag == "tree") {
								
								hit.collider.gameObject.GetComponent<TreeHealth> ().health = 0.0f; //Tell the tree its health is now 0
								PhotonView pv = hit.transform.GetComponent<PhotonView> ();
								pv.RPC ("chopDownTree", PhotonTargets.All, null);
								hit.transform.GetComponent<TreeHealth> ().SpawnWoodCall();
								playerInv.damageTool(0.028f); //Apply damage to the axe (roughly works out at 35 trees per axe)
								
							} else if (targetTag == "rock") { // Add these when added to the game
								hit.collider.gameObject.GetComponent<StoneScript>().health = 0.0f;
								PhotonView pv = hit.transform.GetComponent<PhotonView>();
								pv.RPC("destroyRock", PhotonTargets.All, null);
								hit.transform.GetComponent<StoneScript>().spawnStone();
								playerInv.damageTool(0.05f); //Apply damage to the axe (roughly works out at 20 rocks per hammer)

							} else if (targetTag == "tallgrass") {
								hit.collider.gameObject.GetComponent<GrassScript>().health = 0.0f;
								PhotonView pv = hit.transform.GetComponent<PhotonView>();
								pv.RPC("destroyGrass", PhotonTargets.All, null);
								hit.transform.GetComponent<GrassScript>().spawnGrass();
							}



						}
						
						//When the right trigger is released
					} else {
						
						resetProgress ();
					}
					//When the resource is destroyed
				} else {
					//Turn off right trigger icon
					if (RT_Button.enabled) {
						RT_Button.enabled = false;
					}
					
					resetProgress ();
				}
				
				//When not looking at target
			} else {
				//Turn off right trigger icon
				if (RT_Button.enabled) {
					RT_Button.enabled = false;
				}
				
				resetProgress ();
			}

			//Looking for constructable objects
			if(hit.collider.name == "FirePit_Collider" || hit.collider.name == "Raft_Collider" || hit.collider.name == "Hut_Collider") {
				if (!A_Button.enabled) {
					A_Button.enabled = true;
				}

				//When the player interacts with the highlight construction item, attempt to remove the required items from the their inventory
				if(Input.GetButtonDown("xbox_A") && !inventoryOpen && !constructionOpen){
					Vector3 resourcesRequired = hit.collider.transform.parent.GetComponent<ConstructItem>().getResourcesRequired();
					Vector3 resrouceCount = new Vector3 (0,0,0);
						

					if(playerInv.woodCount >= resourcesRequired.x){
						resrouceCount.x = resourcesRequired.x;
					} else {
						resrouceCount.x = playerInv.woodCount;
					}

					if(playerInv.stoneCount >= resourcesRequired.y){
						resrouceCount.y = resourcesRequired.y;
					} else {
						resrouceCount.y = playerInv.stoneCount;
					}

					if(playerInv.grassCount >= resourcesRequired.z){
						resrouceCount.z = resourcesRequired.z;
					} else {
						resrouceCount.z = playerInv.grassCount;
					}

					//If succesfully removed the items from the players inventory 
					if (playerInv.removeResources((int) resrouceCount.x,(int) resrouceCount.y,(int) resrouceCount.z)){

						if(resrouceCount.x > 0 || resrouceCount.y > 0 || resrouceCount.z > 0){
							audioDriver.playCraftingSound();
							hit.collider.transform.parent.GetComponent<ConstructItem>().addResources(resrouceCount);

							//If the nessecary resources have been added
							if(hit.collider.transform.parent.GetComponent<ConstructItem>().canCreate){
								hit.collider.transform.parent.GetComponent<ConstructItem>().createObject();
							}


						} else {
							if(audioDriver != null){
								audioDriver.playButtonPress();
							}
						}


					}
				}
			} 

			/**
			 * Attacking enemies
			 */ 


			/**
			 * Here youre looking at an enemy....
			 */ 
			if( targetTag == "enemy" || targetTag == "wolf") {
				GameObject enemy = hit.collider.gameObject;
				float enemyCurrentHealth = hit.collider.gameObject.GetComponent<EnemyHealth>().enemyHealth;
				
				//If the enemy is not dead....
				if(enemyCurrentHealth != 0)
				{
					//If you have a tool equiped....
					if(playerInv.toolEquipped != null)
					{
						if (!RT_Button.enabled) {
							RT_Button.enabled = true;
						}
						
						currentTool = playerInv.toolEquipped.tag;
						
						if(currentTool == "axe"){
							playerDamage = 2.0f;
							action = "chopping";
						} else if(currentTool == "hammer"){
							playerDamage = 2.0f;
							action = "hammerAttack";
						} else if(currentTool == "spear"){
							playerDamage = 4.0f;
							action = "spearAttack";
						} else if(currentTool == "dagger"){
							playerDamage = 3.0f;
							action = "daggerAttack";
						} else if(currentTool == "bow"){
							playerDamage = 5.0f;
							action = "chopping";
						}
						
						//If you press the right trigger to attack, damage the enemy, and play the attack animation.
						if (Input.GetAxis ("RightTrigger") > 0.001 && enemyCurrentHealth !=0) {
							Debug.Log("attacknig the enemy");
							lastAttackAnimation = action;
							anim.SetFloat(action, playerDamage / 150);
							Debug.Log("Last attack I did was :" + lastAttackAnimation);
							
							if(audioDriver != null){
								audioDriver.playAttackSound();
							}
							enemy.SendMessage("TakeDamage", playerDamage, SendMessageOptions.DontRequireReceiver);
							enemyCurrentHealth -= playerDamage;
							
							//If youre looking at the enemy but not pressing the trigger...
						} else {
							Debug.Log("Looking but not hitting");
						}
					}
					
				} else if(enemyCurrentHealth <=0){
					enemy.SendMessage("TakeDamage", 100.0f, SendMessageOptions.DontRequireReceiver);	
					PhotonView pv = hit.transform.GetComponent<PhotonView>();
					pv.RPC("Die", PhotonTargets.All, null);
					hit.transform.GetComponent<EnemyHealth>().SpawnMeatCall();
					anim.SetFloat(action,0.0f);
					
					if(playerInv.toolEquipped.tag == "dagger" || playerInv.toolEquipped.tag == "spear"){
						playerInv.damageTool(0.1f); //Apply damage to the spear and dagger (%10 per kill)
					} else {
						playerInv.damageTool(0.2f);//Apply damage to the axe or hammer when used in combat, x2 the deteriation (20%) as it is not a weapon
					}
					
				}

			} else {
				if(lastAttackAnimation != "")
				{
					anim.SetFloat(lastAttackAnimation,0.0f);
					lastAttackAnimation = "";
				}


			}


			//When looking at drinkable water source
			if(playerInv != null && targetTag == "freshWater" && playerInv.canteen.tag != "canteenfull"){
				//Draw right trigger icon on screen
				if (!RT_Button.enabled) {
					RT_Button.enabled = true;
				}
				
				//When the right trigger is held
				if (Input.GetAxis ("RightTrigger") > 0.001) {
					playerInv.fillCanteen();
				}
			} 
			//If not looking at anything
		} else {

			if(damage != 0.0f){
				resetProgress ();
			}

			if(A_Button.enabled){
				A_Button.enabled = false;
			}

			if (RT_Button.enabled) {
				RT_Button.enabled = false;
			}

			if(lastAttackAnimation != "")
			{
				anim.SetFloat(lastAttackAnimation,0.0f);
				lastAttackAnimation = "";
			}
			
		}

		if (!playerDead) {
		
			if (Input.GetAxis ("LeftTrigger") != 0) {
				if (isAxisInUse == false) {
					Debug.Log ("Left Trigger Pressed");
					isAxisInUse = true;
				}
			}
		
			if (Input.GetAxisRaw ("LeftTrigger") == 0) {
				isAxisInUse = false;
			
			}
		
			//When the TAB key is being held down, render and display the vital bars
			if ((Input.GetKeyDown (KeyCode.Tab) || Input.GetButtonDown ("LeftBumper")) && !inventoryOpen) {
				if (audioDriver != null) {
					audioDriver.playButtonPress ();
				}
				vitalBarCanvas.enabled = true;
				reticleCanvas.enabled = false;
			}
		
			//When the TAB key is released, stop rendering and hide the vital bars
			if ((Input.GetKeyUp (KeyCode.Tab) || Input.GetButtonUp ("LeftBumper")) && !inventoryOpen) {
				if (audioDriver != null) {
					audioDriver.playButtonPress ();
				}
				vitalBarCanvas.enabled = false;
				reticleCanvas.enabled = true;
			}
		
			if ((Input.GetKeyDown (KeyCode.Backspace) || Input.GetButtonDown ("xbox_Back")) && !inventoryOpen) {
			
				//Only display tooltip if there is text set for it
				//if(toolTipText.text != " "){
				//	toolTipScreen.GetComponent<Renderer>().enabled = true;
				//}
			
			}
		
			if ((Input.GetKeyUp (KeyCode.Backspace) || Input.GetButtonUp ("xbox_Back")) && !inventoryOpen) {
				//	toolTipScreen.GetComponent<Renderer>().enabled = false;
			}
		
			if (Input.GetButtonDown ("RightBumper")) {
			
			}
		

			if (Input.GetButtonDown ("xbox_B") && !inventoryOpen) {
				if (constructionCanvas.enabled == playerConstruction.constructionOpen) {
					constructionCanvas.enabled = !playerConstruction.constructionOpen;

					if (!playerConstruction.constructionOpen) {
						//Character motor is a javascript file and is being highlighted incorrectly
						parentObj.GetComponent<CharacterMotor> ().canControl = false;
						parentObj.GetComponent<CharacterMotor> ().jumping.enabled = false;

						reticleCanvas.enabled = false;
						//vitalBarScreen.GetComponent<Renderer>().enabled = false;
					} else {
						parentObj.GetComponent<CharacterMotor> ().canControl = true;
						parentObj.GetComponent<CharacterMotor> ().jumping.enabled = true;

						reticleCanvas.enabled = true;
					}
				}
			}

			if (Input.GetButtonUp ("xbox_B") && !inventoryOpen) {
			
				playerConstruction.constructionOpen = !playerConstruction.constructionOpen;

				//	if(playerConstruction != null){
				//		playerConstruction.constructionOpen = constructionOpen;
				//	}
			
				if (audioDriver != null) {
					audioDriver.playOpenInventory ();
				}
			}
		
			if (Input.GetButtonDown ("xbox_A") && !inventoryOpen) {
				Debug.Log ("A button on xbox controller pressed");
			}
		
			//The bool inventory open allows us to toggle with each press of x button
			if (Input.GetButtonDown ("xbox_X")) {
			
				if (inventoryCanvas.enabled == inventoryOpen) {
					inventoryCanvas.enabled = !inventoryOpen;
				
					if (!inventoryOpen) {
						//Character motor is a javascript file and is being highlighted incorrectly
						parentObj.GetComponent<CharacterMotor> ().canControl = false;
						parentObj.GetComponent<CharacterMotor> ().jumping.enabled = false;

						reticleCanvas.enabled = false;
						//vitalBarScreen.GetComponent<Renderer>().enabled = false;
					} else {
						parentObj.GetComponent<CharacterMotor> ().canControl = true;
						parentObj.GetComponent<CharacterMotor> ().jumping.enabled = true;

						reticleCanvas.enabled = true;
					}
				}
			}
		
			if (Input.GetButtonUp ("xbox_X")) {
			
				inventoryOpen = !inventoryOpen;
			
				if (playerInv != null) {
					playerInv.inventoryOpen = inventoryOpen;
				}
			
				if (audioDriver != null) {
					audioDriver.playOpenInventory ();
				}
			}
		
			if(Input.GetButtonDown("xbox_Y"))
			{

			}
		
			if (Input.GetButtonDown ("xbox_Start")) {
				Debug.Log ("Start butn pressed");
			}
		
		
		
			if (Input.GetButtonDown ("LeftThumbstickClick")) {
				Debug.Log ("Left thumbstick pressed");
			}
		
			if (Input.GetButtonDown ("RightThumbstickClick")) {
				Debug.Log ("Right thumbstick pressed");
			}
		
		}
	}


	//Called to reset the reticle and progress when a player is finished interacting with a resourcd
	public void resetProgress(){
		
		if(damage != 0.0f){
			damage = 0.0f; //Reset damage
		}
		
		//Reset resting and reticle progress
		if (vitalDriver != null) {
			vitalDriver.resting = true; 
			
			if(vitalDriver.GetReticleProgress() != 0.0f){
				vitalDriver.SetReticleProgress (0.0f);
			}
		}
		
		anim.SetFloat(action,0.0f);
		
	}
	
}
