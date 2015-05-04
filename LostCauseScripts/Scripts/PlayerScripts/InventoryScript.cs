using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryScript : MonoBehaviour {
	
	//private GameObject highlight; //This is the green highlight indicating selected object
	private Image highlight_image;
	private Image equipped_Icon, torch_Equipped_Icon; //Used to highlight equiiped item
	
	private float sidewaysBuffer = 95.0f; //This is the distance the highlight sprite needs to be shifted left and right
	private float downwardBuffer = 	95.0f; //This is the distance the highlight sprite needs to be shifted up and down
	
	private float xOrigin; //The original x position of the highlight, eg square 0
	private float yOrigin; //The original y position of the highlight, eg square 0
	
	private float craft_xOrigin = -196.0f; //xOrigin of the first crafting slot
	private float craft_yOrigin = -328.0f; //yOrigin of the first crafting slot
	
	private bool up_released, down_released, right_released, left_released; //Used to detect when the dpad is released
	
	private int maxRow = 3, maxCol = 4, row = 0, col = 0; //this represents how many slots in the inventory
	private int selectedIndex = 0; //This is the current inventory slot selected
	
	private Image meat_Icon, wood_Icon, stone_Icon, grass_Icon, axe_Icon, hammer_Icon, dagger_Icon, spear_Icon, torch_Icon, canteen_full_Icon, canteen_halfFull_Icon, canteen_empty_Icon; //Icons for displaying items in the inventory
	private Image Axe_Cover, Hammer_Cover, Dagger_Cover, Spear_Cover, Torch_Cover, A_Highlight, B_Highlight, Y_Highlight; //Cover to darken uncraftable items and highlight to light up button presses
	private GameObject axe_Equipped, hammer_Equipped, dagger_Equipped, spear_Equipped, torch_Equipped; //Weapon and tool prefabs to enable
	public GameObject axe_Icon_Object;
	private int totalItems = 0;
	private static int inventorySize = 20;
	
	public GameObject[] inventoryObjects = new GameObject[inventorySize]; //This array holds the objects in our inventory, public for testing
	public Image[] inventoryIcons = new Image[inventorySize]; //This array holds the icons to be displayed for each item in the inventory
	public GameObject canteen;
	public GameObject toolEquipped;
	private int toolEquippedRow, toolEquippedCol, toolEquippedIndex; //This is the inventory index the equipped tool is at

	private GameObject currentTorchEquipped;
	private bool torchIsEquipped = false; //Check if the torch is equiped
	public bool besideFire = false; //Determine if the player is next to fire and able to craft torches
	private int torchEquippedRow, torchEquippedCol, torchEquippedIndex; //This is the inventory index the equipped tool is at
	private static float TORCH_LIFE = 10.0f; //Timer used to decrease torch when it is active
	private float torchTimer = TORCH_LIFE;

	public bool inventoryOpen = false; //Stores if the inventory is open or not
	public bool inventoryUpToDate = false; //This is to keep track of wheter the gui is displaying the up to date contents of the inventory
	public bool craftingMode = false; //Determines if the player is in crafting mode
	
	//Store item resource quantity for quick access
	public int woodCount = 0, stoneCount = 0, grassCount = 0;
	
	private bool canCraft_Hammer, canCraft_Axe, canCraft_Spear, canCraft_Dagger, canCraft_Torch = false;
	
	//Requirements to craft each item (x = wood required, y = stone and z = grass);-------------------------------------------------------------
	//Note grass is 0 for all now as we don't have the resource in, will need to change this at a later date
	private Vector3 hammerRequirements = new Vector3(2,3,2);
	private Vector3 axeRequirements = new Vector3(3,2,2);
	private Vector3 spearRequirements = new Vector3(4,1,4);
	private Vector3 daggerRequirements = new Vector3(1,1,2);
	private Vector3 torchRequirements = new Vector3(2,0,4);
	
	//Audio driver
	private AudioDriver audioDriver;
	
	
	// Use this for initialization
	void Start () {
		
		up_released = down_released = right_released = left_released = true;
		
		findGuiItems ();
		
		if (highlight_image != null) {
			//Save the original positions of the highlight, so we can position based off them later
			
			xOrigin = highlight_image.transform.localPosition.x;
			yOrigin = highlight_image.transform.localPosition.y;
			
			Debug.Log ("x origin: " + xOrigin);
			
		}
		
		audioDriver = GetComponent<AudioDriver> ();
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//Update highlight position based on index
		if (highlight_image != null) {
			if(!craftingMode){
				selectedIndex = row*maxCol + row + col;
				highlight_image.transform.localPosition = new Vector3 (xOrigin + (sidewaysBuffer*col), yOrigin - (downwardBuffer*row), highlight_image.transform.localPosition.z);
			} else {
				highlight_image.transform.localPosition = new Vector3 (craft_xOrigin + (sidewaysBuffer*col), craft_yOrigin, highlight_image.transform.localPosition.z);
			}
		}
		
		//Listen for dpad input to move selected index ----------------------------------
		if (DPadButtons.up && up_released && inventoryOpen) {
			
			if(row > 0){
				row -= 1;
			} else {
				row = maxRow;
			}
			
			up_released = false;
		}
		
		if (!DPadButtons.up && !up_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			up_released = true;
		}
		
		if (DPadButtons.down && down_released && inventoryOpen) {
			if(row < maxRow){
				row += 1;
			} else {
				row = 0;
			}
			
			down_released = false;
		}
		
		if (!DPadButtons.down && !down_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			down_released = true;
		}
		
		
		if (DPadButtons.left && left_released && inventoryOpen) {
			if(col > 0){
				col -= 1;
			} else {
				col = maxCol;
				
				if(row > 0){
					row -= 1;
				} else {
					row = maxRow;
				}
			}
			
			left_released = false;
		}
		
		if (!DPadButtons.left && !left_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			left_released = true;
		}
		
		
		if (DPadButtons.right && right_released && inventoryOpen) {
			
			if(col < maxCol){
				col += 1;
			} else {
				col = 0;
				
				if(row < maxRow){
					row += 1;
				} else {
					row = 0;
				}
			}
			
			right_released = false;
		}
		
		if (!DPadButtons.right && !right_released) {
			if(audioDriver != null){
				audioDriver.playDpadClick();
			}
			
			right_released = true;
		}
		
		// ---------------------------------------------------------------------------
		
		//Check if inventory gui needs to be updated
		if (!inventoryUpToDate) {
			updateInventoryDisplay();		
		}
		
		//When A button is pressed call the use function
		if(Input.GetButtonDown("xbox_A") && inventoryOpen && (craftingMode || inventoryObjects[selectedIndex] != null)) {
			Debug.Log ("Pressing A while in use mode, on collumn: " + col);

			if(audioDriver != null){
				audioDriver.playButtonPress();
			}
			
			StartCoroutine(APressAction());
		} 
		
		//When B button is pressed call the use function
		if(Input.GetButtonDown("xbox_B") && inventoryOpen && inventoryObjects[selectedIndex] != null) {
			if(inventoryObjects[selectedIndex].name != "CanteenState"){ //Can't drop canteen
				
				if(audioDriver != null){
					audioDriver.playButtonPress();
				}
				
				StartCoroutine(BPressAction());
			}
		}
		
		//When Y button is pressed call the use function
		if(Input.GetButtonDown("xbox_Y") && inventoryOpen) {
			if(audioDriver != null){
				audioDriver.playButtonPress();
			}
			
			StartCoroutine(YPressAction());
		}
		
		if (Input.GetButtonDown ("xbox_X") && inventoryOpen) {
			if(audioDriver != null){
				audioDriver.playButtonPress();
			}
			
			row = col = 0; //Reset position
			craftingMode = false;
		}

		if (torchIsEquipped) {
			torchTimer -= Time.deltaTime;
			//Every 10 seconds decrement the torch health
			if(torchTimer <= 0){
				if(currentTorchEquipped != null){
					currentTorchEquipped.GetComponent<ToolHealth>().reduceHealth(0.04f); //Called 6 x per min, torch lasts 5 minutes, meaning 30 calls, 1.0/30 = 0.0333... so round to 0.034
					torchTimer = TORCH_LIFE; //Reset the 10 second count

					//When torch is depleted, delete the object from the inventory
					if(currentTorchEquipped.GetComponent<ToolHealth>().health <= 0.0f){
						
						audioDriver.playCraftingSound();
						Destroy(inventoryObjects[torchEquippedIndex]);
						inventoryObjects[torchEquippedIndex] = null;
						Destroy(inventoryIcons[torchEquippedIndex].transform.gameObject);
						inventoryIcons[torchEquippedIndex] = null;


						//Unequip the torch --------------
						torch_Equipped.transform.localPosition = new Vector3 (axe_Equipped.transform.localPosition.x, -10000.0f, axe_Equipped.transform.localPosition.z);
						currentTorchEquipped = null;
						torchIsEquipped = false;

						totalItems -= 1;
						inventoryUpToDate = false;
						
					}
				}
			}
		}
		
	}
	
	//Code run when the A button is pressed (Using coroutine to highlight the button)
	public IEnumerator APressAction(){
		
		A_Highlight.enabled = true; //Light up A button
		
		if(!craftingMode){
			Debug.Log ("Pressing A while in use mode, on collumn: " + col);

			use();
		} else {
			Debug.Log ("Pressing A while in crafting mode, on collumn: " + col);
			//Checks if currently able to craft any tool and if so call craft function
			if((col == 0 && canCraft_Axe) || (col == 1 && canCraft_Hammer) || (col == 2 && canCraft_Spear) || (col == 3 && canCraft_Dagger) || (col == 4 && canCraft_Torch)){
				craftTool();
			}
		}
		
		yield return new WaitForSeconds(0.15f);
		
		A_Highlight.enabled = false;
	}
	
	//Code run when the B button is pressed (Using coroutine to highlight the button)
	public IEnumerator BPressAction(){
		
		B_Highlight.enabled = true; //Light up B button
		
		if(!craftingMode){
			drop();
		} else {
			row = col = 0; //Reset position
			craftingMode = false;
		}
		
		yield return new WaitForSeconds(0.15f);
		
		B_Highlight.enabled = false;
		
	}
	
	//Code run when the Y button is pressed (Using coroutine to highlight the button)
	public IEnumerator YPressAction(){
		
		Y_Highlight.enabled = true; //Light up Y button
		
		//When crafting mode is enabled move the highlight down to the crafting tab
		if(!craftingMode){
			highlight_image.transform.localPosition = new Vector3(craft_xOrigin,craft_yOrigin,0);
			craftingMode = true;
			col = 0; //Reset position
		} else { //When the crafting mode is disabled move the highlight back to the regular inventory
			row = col = 0; //Reset position
			craftingMode = false;
		}
		
		yield return new WaitForSeconds(0.15f);
		
		Y_Highlight.enabled = false;
		
		
	}
	
	//if player collides with an item on the ground, add it to inventory


	void OnTriggerEnter(Collider item) {
		string colliderTag = item.gameObject.tag;
		
		if (colliderTag == "wood" || colliderTag == "stone" || colliderTag == "meat" || colliderTag == "grass") {
			
			GameObject newItem = new GameObject ();
			newItem.tag = colliderTag;
			newItem.name = item.gameObject.name;
			
			//If the item is collectible add to inventory and hide from the world
			if (addItemToInventory (newItem)) {
				if (audioDriver != null) {
					audioDriver.playPickUpSound ();
				}

				//Remove the item from the game over the network
				item.GetComponent<ResourceUtil> ().removeItemFromGame ();
				
				//Update item count (This can possibly be refactored to somewhere else)
				if (colliderTag == "wood") {
					woodCount += 1;
				} else if (colliderTag == "stone") {
					stoneCount += 1;

				} else if (colliderTag == "grass") {
					grassCount += 1;
				} 
				
				Debug.Log ("Item Count =  Wood: " + woodCount + ", Stone = " + stoneCount + ", Grass  = " + grassCount);
			}
		} else if (colliderTag == "axe" || colliderTag == "hammer" || colliderTag == "dagger" || colliderTag == "spear" || colliderTag == "torch") {

			GameObject newItem = new GameObject ();
			newItem.tag = colliderTag;
			newItem.name = item.gameObject.name;

			//Add tool health script and copy over the health from the newly collected item
			newItem.AddComponent<ToolHealth>();
			newItem.GetComponent<ToolHealth>().health = item.gameObject.GetComponent<ToolHealth>().health;
			newItem.GetComponent<ToolHealth>().healthUpToDate = false; 

			//If the item is collectible add to inventory and hide from the world
			if (addItemToInventory (newItem)) {
				if (audioDriver != null) {
					audioDriver.playPickUpSound ();
				}

				//Remove the item from the game over the network
				item.GetComponent<ResourceUtil> ().removeItemFromGame ();

			}
		} else if (colliderTag == "constructionObject") {
			
			item.transform.Find ("InstructionCanvas").GetComponent<Canvas> ().enabled = true;

		} else if (colliderTag == "firepit") {
			besideFire = true;
			updateCraftRequirements();
		}
		
	}


	void OnTriggerExit(Collider item) {
		string colliderTag = item.gameObject.tag;

		if (colliderTag == "constructionObject") {
			item.transform.Find ("InstructionCanvas").GetComponent<Canvas>().enabled = false;

		} else if (colliderTag == "firepit") {
			besideFire = false;
			updateCraftRequirements();
		}
	}
	
	
	//Update this function to add items at the beginning of the inventory once an item is removed
	bool addItemToInventory(GameObject item){
		if (totalItems < inventorySize){ //&& !ArrayUtility.Contains(inventoryObjects, item)) { //Check there is space and it is not a duplicate item
			Debug.Log ("collected: " + item.tag);
			
			for(int i = 0; i <= totalItems; i++){
				if(inventoryObjects [i] == null){
					inventoryObjects [i] = item;
					break;
				}
			}
			totalItems++;
			inventoryUpToDate = false;
			
			return true;
			
		} else {
			Debug.Log("Inventory full, cannot pick up item");	
			
			return false;
		}
	}
	
	//This function is called to ensure that every item in the inventory is displayed graphically
	void updateInventoryDisplay(){
		Debug.Log("Updating inventory");
		
		for (int r = 0; r <= maxRow; r++) {
			for(int c = 0; c <= maxCol; c++){
				
				int index = r*maxCol + r + c;
				
				if(inventoryObjects[index] != null){
					
					if(inventoryIcons[index] == null){
						
						//Create an icon corresponding to the inventory object if not already in place
						if(inventoryObjects[index].tag == "meat"){
							createNewIcon(r, c, index, meat_Icon);
							
						} else if(inventoryObjects[index].tag == "wood"){
							createNewIcon(r, c, index, wood_Icon);
							
						} else if(inventoryObjects[index].tag == "stone"){
							createNewIcon(r, c, index, stone_Icon);
							
						} else if(inventoryObjects[index].tag == "axe"){
							createNewIcon(r, c, index, axe_Icon);
							
						} else if(inventoryObjects[index].tag == "hammer"){
							createNewIcon(r, c, index, hammer_Icon);
							
						} else if(inventoryObjects[index].tag == "spear"){
							createNewIcon(r, c, index, spear_Icon);
							
						} else if(inventoryObjects[index].tag == "dagger"){
							createNewIcon(r, c, index, dagger_Icon);

						} else if(inventoryObjects[index].tag == "canteenfull"){
							createNewIcon(r, c, index, canteen_full_Icon);
							
						} else if(inventoryObjects[index].tag == "canteenhalf"){
							createNewIcon(r, c, index, canteen_halfFull_Icon);
							
						} 	else if(inventoryObjects[index].tag == "canteenempty"){
							createNewIcon(r, c, index, canteen_empty_Icon);
							
						} 	else if(inventoryObjects[index].tag == "grass"){
							createNewIcon(r, c, index, grass_Icon);

						} 	else if(inventoryObjects[index].tag == "torch"){
							createNewIcon(r, c, index, torch_Icon);
						} 	
						
						
					}
				} 
			}		
		}

		if (equipped_Icon != null) {

			//Move the equipped icon
			if (toolEquipped != null && !equipped_Icon.enabled) {
				equipped_Icon.enabled = true;
			} else if (toolEquipped == null && equipped_Icon.enabled) {
				equipped_Icon.enabled = false;
			}
		
			equipped_Icon.transform.localPosition = new Vector3 (xOrigin + (sidewaysBuffer * toolEquippedCol) - 45, yOrigin - (downwardBuffer * toolEquippedRow) + 45, equipped_Icon.transform.localPosition.z);
		}

		if (torch_Equipped_Icon != null) {
			
			//Move the equipped icon
			if (currentTorchEquipped != null && !torch_Equipped_Icon.enabled) {
				torch_Equipped_Icon.enabled = true;
			} else if (currentTorchEquipped == null && torch_Equipped_Icon.enabled) {
				torch_Equipped_Icon.enabled = false;
			}
			
			torch_Equipped_Icon.transform.localPosition = new Vector3 (xOrigin + (sidewaysBuffer * torchEquippedCol) - 45, yOrigin - (downwardBuffer * torchEquippedRow) + 45, equipped_Icon.transform.localPosition.z);
		}

		updateCraftRequirements ();
		
		inventoryUpToDate = true;
	}
	
	//Used to create a new image on the inventory for an item
	void createNewIcon(int r, int c, int index, Image icon_prefab){
		
		Image itemIcon = (Image) Instantiate(icon_prefab, new Vector3(0,0,0), icon_prefab.transform.rotation);
		
		//Position the new icon based on its index and store in array
		itemIcon.enabled = true;
		
		itemIcon.transform.parent = GameObject.Find("InventoryIcons").transform;
		
		itemIcon.transform.localPosition =  new Vector3 (xOrigin + (sidewaysBuffer*c), yOrigin - (downwardBuffer*r), 0);
		itemIcon.transform.localScale = new Vector3 (1, 1, 1);


		if (itemIcon != null) {

			string iconType = inventoryObjects[index].tag;

			//If the icon is for a hammer, axe, spear, torch or dagger turn on the slider for the tools health
			if(iconType == "hammer" || iconType == "axe" || iconType == "spear" || iconType == "torch" || iconType == "dagger"){
				itemIcon.transform.Find("Health_Slider/Fill Area/Fill").GetComponent<Image>().enabled = true;

				if(inventoryObjects[index] != null){ //If the objec isnt null assign the slider to the particular tool
					inventoryObjects[index].GetComponent<ToolHealth>().healthSlider = itemIcon.transform.Find("Health_Slider").GetComponent<Slider>();
				}
			}

		}

		inventoryIcons[index] = itemIcon;
	}
	
	//This function is the general "use" action, eg to eat meat or equip item
	//Current state only eats meat
	void use(){
		
		if(inventoryObjects[selectedIndex] != null){
			if(inventoryObjects[selectedIndex].tag == "meat"){
				gameObject.GetComponent<VitalBarDriver> ().updateHunger (0.25f); //0.25 is the amount of hunger eating the meat cures this to be decided
				
				if(audioDriver != null) {
					audioDriver.playEatingSound();
				}

				removeItemAtIndex(selectedIndex);
				
			}else if(inventoryObjects[selectedIndex].tag == "canteenfull" || inventoryObjects[selectedIndex].tag == "canteenhalf"){
				gameObject.GetComponent<VitalBarDriver> ().updateThirst (0.3f); //0.3 is the amount of thirst drinking water restores (value to be decided)
				
				
				if(audioDriver != null) {
					audioDriver.playDrinkSound();
				}
				
				if(inventoryObjects[selectedIndex].tag == "canteenfull"){
					inventoryObjects[selectedIndex].tag = "canteenhalf";
				} else {
					inventoryObjects[selectedIndex].tag = "canteenempty";
				}
				
				Destroy(inventoryIcons[selectedIndex]);
				inventoryIcons[selectedIndex] = null;
				
				inventoryUpToDate = false;

			//If player selects a tool or weapon call the equip function
			} else if(inventoryObjects[selectedIndex].tag == "axe" || inventoryObjects[selectedIndex].tag == "hammer" || inventoryObjects[selectedIndex].tag == "dagger" || inventoryObjects[selectedIndex].tag == "spear"){
				inventoryUpToDate = false;
				//If the tool selected is different to the current one equiped, reset the hand and equip the new item
				if(toolEquipped == null || toolEquipped != inventoryObjects[selectedIndex]){
					unEquipAll();
					equipItem();
				} else {
					unEquipAll(); //Else unequip the current item
				}

			} else if(inventoryObjects[selectedIndex].tag == "torch"){
				inventoryUpToDate = false;
				
				equipTorch();
			} 
		}
	}
	
	//Function used to drop items fromt the players inventory onto the ground
	void drop(){
		float distance = 5.0f; //Distance to drop the item away from the player
		
		if(inventoryObjects[selectedIndex] != null){
			/*
			 * When droping an item from the inventory, it gets reinstantiated into the game world. To do this, we store
			 * the tag of the object (wood, stone, or meat) in a variable called objectToInstantiate. We then pass that 
			 * string into the Instantiate method. Photon then instantiates the corresponding resource into the world.
			 */ 

			//If the item the player wishes to drop is currently equipped, then unequip it before dropping
			if(inventoryObjects[selectedIndex] == toolEquipped){
				unEquipAll();
			} else if(inventoryObjects[selectedIndex] == currentTorchEquipped){
				equipTorch();
			}

			string objectToInstantiate = inventoryObjects[selectedIndex].tag;
			GameObject droppedItem = PhotonNetwork.Instantiate(objectToInstantiate, transform.position + (transform.forward)* distance, Quaternion.identity, 0);

			if(inventoryObjects[selectedIndex].tag == "wood"){
				woodCount -= 1;
			} else if (inventoryObjects[selectedIndex].tag == "stone") {
				stoneCount -= 1;
				
			} else if (inventoryObjects[selectedIndex].tag == "grass") {
				grassCount -= 1;
			}  else {
				//If the item is a tool or weapon update its health
				droppedItem.GetComponent<ToolHealth>().setHealth(inventoryObjects[selectedIndex].GetComponent<ToolHealth>().health);
			}

			totalItems-=1;
			Destroy(inventoryObjects[selectedIndex]);
			inventoryObjects[selectedIndex] = null;
			Destroy(inventoryIcons[selectedIndex].transform.gameObject);
			inventoryIcons[selectedIndex] = null;


			inventoryUpToDate = false;

		}
		
	}

	//Removes item from inventory at specified index

	void removeItemAtIndex(int index){

		//Destroy (inventoryObjects [index]);
	//	inventoryObjects[index] = null;
		//Destroy(inventoryIcons[index]);
		//inventoryIcons[index] = null;

		Destroy(inventoryObjects[index]);
		inventoryObjects[index] = null;
		Destroy(inventoryIcons[index].transform.gameObject);
		inventoryIcons[index] = null;

		totalItems -= 1;
		
	}
	
	
	//Checks if the player has enough resources to craft each item
	void updateCraftRequirements(){
		//Check which items can be crafted and display accordingly on the GUI
		
		if (woodCount >= hammerRequirements.x && stoneCount >= hammerRequirements.y && grassCount >= hammerRequirements.z) {
			canCraft_Hammer = true;
			Hammer_Cover.enabled = false;
		} else if(canCraft_Hammer){
			canCraft_Hammer = false;
			Hammer_Cover.enabled = true;
		}
		
		if (woodCount >= axeRequirements.x && stoneCount >= axeRequirements.y && grassCount >= axeRequirements.z) {
			canCraft_Axe = true;
			Axe_Cover.enabled = false;
		} else if(canCraft_Axe){
			canCraft_Axe = false;
			Axe_Cover.enabled = true;
		}
		
		if (woodCount >= daggerRequirements.x && stoneCount >= daggerRequirements.y && grassCount >= daggerRequirements.z) {
			canCraft_Dagger = true;
			Dagger_Cover.enabled = false;
		} else if(canCraft_Dagger){
			canCraft_Dagger = false;
			Dagger_Cover.enabled = true;
		}
		
		if (woodCount >= spearRequirements.x && stoneCount >= spearRequirements.y && grassCount >= spearRequirements.z) {
			canCraft_Spear = true;
			Spear_Cover.enabled = false;
		} else if(canCraft_Spear){
			canCraft_Spear = false;
			Spear_Cover.enabled = true;
		}
		
		if (woodCount >= torchRequirements.x && stoneCount >= torchRequirements.y && grassCount >= torchRequirements.z && besideFire) {
			canCraft_Torch = true;
			Torch_Cover.enabled = false;
		} else if(canCraft_Torch){
			canCraft_Torch = false;
			Torch_Cover.enabled = true;
		}
	}
	
	
	//Remove when finished
	void craftTool() {
		Debug.Log ("Crafting tool at column = " + col);
		
		string name = "";
		Vector3 resourcesRequired = new Vector3(0,0,0);
		
		//Depending on the column selected set the name and requirements to the corresponding item
		if (col == 0) {
			name = "axe";
			resourcesRequired = axeRequirements;
		} else if (col == 1) {
			name = "hammer";
			resourcesRequired = hammerRequirements;
		} else if (col == 2) {
			name = "spear";
			resourcesRequired = spearRequirements;
		} else if (col == 3) {
			name = "dagger";
			resourcesRequired = daggerRequirements;
		} else if (col == 4) {
			name = "torch";
			resourcesRequired = torchRequirements;
		}
		
		float woodRequired = resourcesRequired.x, stoneRequired = resourcesRequired.y, grassRequired = resourcesRequired.z;
		
		//itterate through every item in the inventory, and remove the required resources until the correct amount has been used
		for(int i = inventorySize-1; i >= 0; i--){
			
			if(inventoryObjects[i] != null){
				if(inventoryObjects[i].tag == "wood" && woodRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					woodCount -= 1;
					woodRequired -= 1;
				} else if (inventoryObjects[i].tag == "stone" && stoneRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					stoneCount -= 1;
					stoneRequired -= 1;
				} else if (inventoryObjects[i].tag == "grass" && grassRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					grassCount -= 1;
					grassRequired -= 1;
				}
				
				if(woodRequired == 0 && stoneRequired == 0 && grassRequired == 0){
				
					//Create axe and add it to inventory (empty game object is a placeholder for the time being)
					GameObject tool = new GameObject();
					tool.name = name;
					tool.tag = name;
					tool.AddComponent<ToolHealth>();
					
					addItemToInventory (tool);
					
					if(audioDriver != null){
						audioDriver.playCraftingSound();
					}
					break;
				}
			}
		} 
		
	}

	public bool removeResources(int wood, int stone, int grass){
		float woodRequired = wood, stoneRequired = stone, grassRequired = grass;
		
		//itterate through every item in the inventory, and remove the required resources until the correct amount has been used
		for(int i = totalItems-1; i >= 0; i--){
			
			if(inventoryObjects[i] != null){
				if(inventoryObjects[i].tag == "wood" && woodRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					woodCount -= 1;
					woodRequired -= 1;
				} else if (inventoryObjects[i].tag == "stone" && stoneRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					stoneCount -= 1;
					stoneRequired -= 1;
				} else if (inventoryObjects[i].tag == "grass" && grassRequired != 0){
					Debug.Log("removing item at index: " + i);
					removeItemAtIndex(i);
					grassCount -= 1;
					grassRequired -= 1;
				}
				
				if(woodRequired == 0 && stoneRequired == 0 && grassRequired == 0){
					
					return true;
				}
			}
		} 

		return false;
	}

	public void equipItem(){

		Debug.Log ("Equipping item");

		string itemTag = inventoryObjects [selectedIndex].tag;

		Debug.Log ("Equipping item with tag " + itemTag );

		if(inventoryObjects[selectedIndex] != null){
			toolEquipped = inventoryObjects[selectedIndex];
			toolEquippedRow = row;
			toolEquippedCol = col;
			toolEquippedIndex = selectedIndex;

			//Move the tool into the players hand (moving as opposed to disabling and enabling due to network constraints)
			if(itemTag == "axe" && axe_Equipped != null){
				axe_Equipped.transform.localPosition = new Vector3 (axe_Equipped.transform.localPosition.x, 0.145f, axe_Equipped.transform.localPosition.z);

			} else if(itemTag == "hammer" && hammer_Equipped != null ){
				hammer_Equipped.transform.localPosition = new Vector3 (hammer_Equipped.transform.localPosition.x, 0.145f, hammer_Equipped.transform.localPosition.z);

			} else if(itemTag == "dagger" && dagger_Equipped != null){
				dagger_Equipped.transform.localPosition = new Vector3 (dagger_Equipped.transform.localPosition.x, 0.145f, dagger_Equipped.transform.localPosition.z);

			} else if(itemTag == "spear" && spear_Equipped != null){
				spear_Equipped.transform.localPosition = new Vector3 (spear_Equipped.transform.localPosition.x, 0.145f, spear_Equipped.transform.localPosition.z);
			
			} 
		}
	}

	//Unequip all items and tools
	public void unEquipAll(){
		Debug.Log ("unEquipAll called");

		if (axe_Equipped != null)
			axe_Equipped.transform.localPosition = new Vector3 (axe_Equipped.transform.localPosition.x, -10000.0f, axe_Equipped.transform.localPosition.z);

		if(hammer_Equipped!=null)
			hammer_Equipped.transform.localPosition = new Vector3 (hammer_Equipped.transform.localPosition.x, -10000.0f, hammer_Equipped.transform.localPosition.z);

		if(dagger_Equipped!=null)
			dagger_Equipped.transform.localPosition = new Vector3 (dagger_Equipped.transform.localPosition.x, -10000.0f, dagger_Equipped.transform.localPosition.z);

		if(spear_Equipped!=null)
			spear_Equipped.transform.localPosition = new Vector3 (spear_Equipped.transform.localPosition.x, -10000.0f, spear_Equipped.transform.localPosition.z);

		toolEquipped = null;
	}

	public void equipTorch(){
		
		Debug.Log ("Equipping torch");

		if (torch_Equipped != null) {

			//if there is no torch equipped or it is a different torch then equip the torch
			if(!torchIsEquipped || (torchIsEquipped && currentTorchEquipped != inventoryObjects[selectedIndex])){

				currentTorchEquipped = inventoryObjects[selectedIndex];
				torch_Equipped.transform.localPosition = new Vector3 (spear_Equipped.transform.localPosition.x, 0.169f, spear_Equipped.transform.localPosition.z);
				torchEquippedRow = row;
				torchEquippedCol = col;
				torchEquippedIndex = selectedIndex;

				torchIsEquipped = true;
				torchTimer = TORCH_LIFE; //Reset the 10 second count

			//If clicking the same torch then unequip
			} else if((torchIsEquipped && currentTorchEquipped == inventoryObjects[selectedIndex]) || (!torchIsEquipped)){
				torch_Equipped.transform.localPosition = new Vector3 (axe_Equipped.transform.localPosition.x, -10000.0f, axe_Equipped.transform.localPosition.z);
				currentTorchEquipped = null;
				torchIsEquipped = false;
			
			} 
			
		}
	}

	//Do damage to the tool equipped
	public void damageTool(float damage){
		if (toolEquipped != null) {
			toolEquipped.GetComponent<ToolHealth>().reduceHealth(damage);
		

			if(toolEquipped.GetComponent<ToolHealth>().health <= 0.0f){
						
				Debug.Log ("Removing tool at index : " + toolEquippedIndex);

				unEquipAll();
				audioDriver.playCraftingSound();
				Destroy(inventoryObjects[toolEquippedIndex]);
				inventoryObjects[toolEquippedIndex] = null;
				Destroy(inventoryIcons[toolEquippedIndex].transform.gameObject);
				inventoryIcons[toolEquippedIndex] = null;

				totalItems -= 1;

				inventoryUpToDate = false;

			}

		}
	}

	public void fillCanteen(){
		canteen.tag = "canteenfull"; //Set the canteen tag to represent it being full
		inventoryIcons [0] = null; //Reset the icon for the canteen
		inventoryUpToDate = false;
		
		audioDriver.playDrinkSound();
	}

	//This function removes every item from the players inventory and refills the canteen (called on repspawn)
	public void resetInventory(){
		unEquipAll();
		currentTorchEquipped = null;

		for (int i = 1; i < inventorySize; i++) {

			if(inventoryObjects[i] != null){
				removeItemAtIndex(i);
			}
		}

		//Remove canteen inventory icon so it can be refreshed
		Destroy(inventoryIcons[0].transform.gameObject);
		inventoryIcons[0] = null;

		inventoryObjects [0].tag = "canteenfull";
		inventoryUpToDate = false;

		woodCount = stoneCount = grassCount = 0;


		GetComponent<VitalBarDriver> ().SetHealth (1.0f); 
		Debug.Log ("Inventory reset total item count is now: " + totalItems);
	}

	//This functions finds and assigns required GUI items, such as icons, backgrounds etc.. (Only called once during setup)
	void findGuiItems(){
		highlight_image = GameObject.Find ("Highlight_Sprite").GetComponent<Image> ();
		equipped_Icon = GameObject.Find ("Equipped_Icon").GetComponent<Image> ();
		torch_Equipped_Icon = GameObject.Find ("Torch_Equipped_Icon").GetComponent<Image> ();

		meat_Icon = GameObject.Find("Meat_Icon").GetComponent<Image>();
		wood_Icon = GameObject.Find("Wood_Icon").GetComponent<Image>();
		stone_Icon = GameObject.Find("Stone_Icon").GetComponent<Image>();
		grass_Icon = GameObject.Find("Grass_Icon").GetComponent<Image>();
		axe_Icon = GameObject.Find("Axe_Icon").GetComponent<Image>();


		hammer_Icon = GameObject.Find("Hammer_Icon").GetComponent<Image>();
		dagger_Icon = GameObject.Find("Dagger_Icon").GetComponent<Image>();
		spear_Icon = GameObject.Find("Spear_Icon").GetComponent<Image>();
		torch_Icon = GameObject.Find("Torch_Icon").GetComponent<Image>();
		canteen_full_Icon = GameObject.Find("Canteen_Full_Icon").GetComponent<Image>();
		canteen_halfFull_Icon = GameObject.Find("Canteen_HalfFull_Icon").GetComponent<Image>();
		canteen_empty_Icon = GameObject.Find("Canteen_Empty_Icon").GetComponent<Image>();
		canteen = GameObject.Find ("CanteenState");

		Hammer_Cover = GameObject.Find("Hammer_Cover").GetComponent<Image>();
		Axe_Cover = GameObject.Find("Axe_Cover").GetComponent<Image>();
		Spear_Cover = GameObject.Find("Spear_Cover").GetComponent<Image>();
		Dagger_Cover = GameObject.Find("Dagger_Cover").GetComponent<Image>();
		Torch_Cover = GameObject.Find("Bow_Cover").GetComponent<Image>();
		A_Highlight = GameObject.Find("A_Highlight").GetComponent<Image>();
		B_Highlight = GameObject.Find("B_Highlight").GetComponent<Image>();
		Y_Highlight = GameObject.Find("Y_Highlight").GetComponent<Image>();

		axe_Equipped = GameObject.Find("axe_Equipped");
		hammer_Equipped = GameObject.Find("hammer_Equipped");
		dagger_Equipped = GameObject.Find("dagger_Equipped");
		spear_Equipped = GameObject.Find("spear_Equipped");
		torch_Equipped = GameObject.Find("torch_Equipped");

		
		unEquipAll ();
		addItemToInventory (canteen);
		torch_Equipped.transform.localPosition = new Vector3 (axe_Equipped.transform.localPosition.x, -10000.0f, axe_Equipped.transform.localPosition.z);

		inventoryUpToDate = false;
		
	}
	

}