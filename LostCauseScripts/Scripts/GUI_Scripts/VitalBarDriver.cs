using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VitalBarDriver : MonoBehaviour {
	
	private float hunger, thirst, energy, health, progress;
	private Slider hungerSlider, thirstSlider, energySlider, healthSlider, progressBar;
	public bool alive,resting = true; //When the player is not performing any activities recharge energy 

	private DeathScript deathScript;

	//Deplete hunger at 6% of the bar per minute (1% per 10 seconds)
	//Deplete hunger at 8% of the bar per minute (~1.4% per 10 seconds)
	private float vital_time = 10.0f; //Used to keep track of 10 second intervals
	private float timer, staminaTimer;

	
	//Audio driver
	private AudioDriver audioDriver;
	private AudioSource heartBeat;
	private bool heartBeatingSound = false;
	

	void Start (){
		audioDriver = GetComponent<AudioDriver> ();

		
		timer = vital_time;
		staminaTimer = vital_time / 10.0f;
		setupSliders ();

		hunger = 1.0f;
		thirst = 1.0f;
		energy = 1.0f;
		health = 1.0f;
		progress = 0.0f;
		
		SetHunger (hunger);
		SetThirst (thirst);
		SetEnergy (energy);

		InvokeRepeating("updateVitals", 1.0f, 1.0f);

	}

	// Update is called once per frame
	void Update () {

		if (heartBeat == null) {
			heartBeat = GetComponent<AudioSource>();
		}

		//When health gets bellow 0 the player has died
		if (health <= 0.0f && alive) {
			if(deathScript == null){
				deathScript = GetComponent<DeathScript>();
			} 

			deathScript.playerDead();
			alive = false;
		}

		//Play heartbeat sound effect when health is less than a quarter
		if (!heartBeatingSound && (health <= 0.25f && health >= 0.0f)) {
			if (heartBeat != null) {
				heartBeat.Play ();
				heartBeatingSound = true;
			}
		} else if (heartBeatingSound && health > 0.25f) {
			if (heartBeat != null) {
				heartBeat.Stop ();
				heartBeatingSound = false;
			}
		}

		if (!alive && Input.GetButtonDown ("xbox_A")) {
			deathScript.respawnPlayer();

			if (audioDriver != null) {
				audioDriver.playOpenInventory ();
			}

			alive = true;


			SetHunger (1.0f);
			SetThirst (1.0f);
			SetEnergy (1.0f);
		}
	}

	//This function is called every second to gradually reduce the players vitals
	private void updateVitals(){

		updateHunger(-0.001f); //Deplete hunger by 0.1% // every second
		updateThirst(-0.0014f); //Deplete thirst by 0.14% // every second
		
		if (hunger <= 0.0f && thirst <= 0.0f) {
			updateHealth (-0.005f); //Deplete health by 0.5% // every second when fully hungry and thirsty
		} else if (hunger <= 0.0f) {
			updateHealth (-0.002f); //Deplete health by 0.2% // every second when fully hungry
		} else if (thirst <= 0.0f) {
			updateHealth (-0.002f); //Deplete health by 0.2% // every second when fully thirsty
		}  else if(health < 1.0f) {
			updateHealth(0.005f);  //Increase health by 0.5% per second if not thirsty / hungry

		}

		if (resting) {
				updateEnergy(0.03f); //Increase energy at 3% per second when not active
		}

	}

	//Takes in a value between 0-1 (0-100%) and sets the hunger bar to it
	public void SetHunger(float value){
		hunger = value;
		hungerSlider.value = hunger;
	}


	public void updateHunger(float value){
		if ((hunger + value) > 1.0f) {
			hunger = 1;
		} else if((hunger + value) < 0.0f) {
			hunger = 0.0f;
		} else {
			hunger += value;
		}

		hungerSlider.value = hunger;
	}

	//Takes in a value between 0-1 (0-100%) and sets the thirst bar to it
	public void SetThirst(float value){
		thirst = value;
		thirstSlider.value = thirst;	
	}

	public void updateThirst(float value){
		if ((thirst + value) > 1.0f) {
			thirst = 1;
		} else if((thirst + value) < 0.0f) {
			thirst = 0.0f;
		} else {
			thirst += value;
		}
		
		thirstSlider.value = thirst;
	}
	
	//Takes in a value between 0-1 (0-100%) and sets the energy bar to it
	
	public void SetEnergy(float value){
		energy = value;
		energySlider.value = energy;	
	}

	public void updateEnergy(float value){
		if ((energy + value) > 1.0f) {
			energy = 1;
		} else if((energy + value) < 0.0f) {
			energy = 0.0f;
		} else {
			energy += value;
		}
		
		energySlider.value = energy;
	}

	//Takes in a value between 0-1 (0-100%) and sets the health bar to it
	
	public void SetHealth(float value){
		health = value;
		healthSlider.value = health;	
	}
	
	public void updateHealth(float value){
		if ((health + value) > 1.0f) {
			health = 1;
		} else if((health + value) < 0.0f) {
			health = 0.0f;
		} else {
			health += value;
		}
		
		healthSlider.value = health;
	}


	public void SetReticleProgress(float value){
		progress = value;
		if (progressBar != null) {
			progressBar.value = progress;
		}
	}
	
	//Getters
	public float GetHunger(){
		return hunger;
	}

	public float GetThirst(){
		return thirst;
	}
	
	public float GetEnergy(){
		return energy;
	}

	public float GetHealth(){
		return health;
	}
	
	public float GetReticleProgress(){
		return progress;
	}

	void setupSliders(){
		hungerSlider = GameObject.Find ("HungerSlider").GetComponent<Slider> ();
		thirstSlider = GameObject.Find ("ThirstSlider").GetComponent<Slider> ();
		energySlider = GameObject.Find ("EnergySlider").GetComponent<Slider> ();
		healthSlider = GameObject.Find ("HealthSlider").GetComponent<Slider> ();

		progressBar = GameObject.Find ("Progress_Slider").GetComponent<Slider> ();
	}

}
