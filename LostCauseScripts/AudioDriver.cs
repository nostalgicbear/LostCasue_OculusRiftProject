using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioDriver : MonoBehaviour {

	private GameObject audioManager;
	private AudioSource inventoryOpen, inventoryCraft, buttonPress, dpadClick, drinkSound, eatingSound, pickUpSound, pageOpenSound, attackSound, treeChop1, mineSound, pickGrass, deathSound;
	private List<AudioClip> woodChopSounds = new List<AudioClip>();
	private List<AudioClip> weaponSounds = new List<AudioClip>();
	private List<AudioClip> miningSounds = new List<AudioClip>();


	// Use this for initialization
	void Start () {
		audioManager = GameObject.Find ("_audio");

		AudioSource[] audios = audioManager.GetComponents<AudioSource> ();
		AudioSource[] audios2 = audioManager.GetComponentsInChildren<AudioSource> ();
		inventoryOpen = audios [0];
		inventoryCraft = audios [1];
		buttonPress = audios [2];
		dpadClick = audios [3];
		drinkSound = audios [4];
		eatingSound = audios [5];
		pickUpSound = audios [6];

		//on child component
		attackSound = audios2 [7];
		treeChop1 = audios2 [8];
		mineSound = audios2 [9];
		pickGrass = audios2 [10];
		pageOpenSound = audios2 [11];
		deathSound = audios2 [12];


		woodChopSounds.Add (Resources.Load ("TreeChop4") as AudioClip);
		woodChopSounds.Add (Resources.Load ("TreeChop5") as AudioClip);
		woodChopSounds.Add (Resources.Load ("TreeChop6") as AudioClip);

		weaponSounds.Add (Resources.Load ("WeaponSwing4") as AudioClip);
		weaponSounds.Add (Resources.Load ("WeaponSwing2") as AudioClip);
		weaponSounds.Add (Resources.Load ("WeaponSwing3") as AudioClip);

		miningSounds.Add (Resources.Load ("Mining") as AudioClip);
		miningSounds.Add (Resources.Load ("Mining1") as AudioClip);
		miningSounds.Add (Resources.Load ("Mining2") as AudioClip);
		miningSounds.Add (Resources.Load ("Mining3") as AudioClip);

		Debug.Log ("audio length is " + audios.Length);
		Debug.Log ("audio2 length is " + audios2.Length);
	}
	
	// Update is called once per frame
	void Update () {


		
	}

	public void playOpenInventory(){
		if (inventoryOpen != null) {
			inventoryOpen.Play();
		}
	}

	public void playCraftInventory(){
		if (inventoryCraft != null) {
			inventoryCraft.Play();
		}
	}

	public void playButtonPress(){
		if (buttonPress != null) {
			buttonPress.Play();
		}
	}

	public void playDpadClick(){
		if (dpadClick != null) {
			dpadClick.Play();
		}
	}

	public void playDrinkSound(){
		if (drinkSound != null) {
			drinkSound.Play();
		}
	}

	public void playEatingSound(){
		if (eatingSound != null) {
			eatingSound.Play();
		}
	}

	public void playCraftingSound(){
		if (inventoryCraft != null) {
			inventoryCraft.Play();
		}
	}

	public void playPickUpSound(){
		if (pickUpSound != null) {
			pickUpSound.Play();
		}
	}

	/**
	public void playLeverOn(){
		if (leverOn != null) {
			leverOn.Play();
		}
	}
	*/



	public void playWoodChopSound(){
		if (treeChop1 != null) {
			if(!treeChop1.isPlaying){
				treeChop1.clip = woodChopSounds[Random.Range(0, woodChopSounds.Count)];
				treeChop1.Play();
			}
		}
	}



	public void playAttackSound(){
		if (attackSound != null) {
			if(!attackSound.isPlaying)
			{
				attackSound.clip = weaponSounds[Random.Range(0, weaponSounds.Count)];
				attackSound.Play();
			}
		}
	}

	public void playMineSound(){
		if (mineSound != null) {
			if(!mineSound.isPlaying)
			{
				mineSound.clip = miningSounds[Random.Range(0, miningSounds.Count)];
				mineSound.Play();
			}
		}
	}

	public void playPullGrassSound(){
		if (pickGrass != null) {
			if(!pickGrass.isPlaying)
			{
				pickGrass.Play();
			}
		}
	}

	public void playOpenBook(){

		if (pageOpenSound != null) {
			pageOpenSound.Play();
		}
	}

	public void playDeathSound(){
		
		if (deathSound != null) {
			deathSound.Play();
		}
	}

}
