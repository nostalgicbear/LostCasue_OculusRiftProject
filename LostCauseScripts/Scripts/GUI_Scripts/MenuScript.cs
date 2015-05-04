using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This script is used to calculate where the player is looking on the menu screen

public class MenuScript : MonoBehaviour {
	private int rayLength = 15;
	private RaycastHit hit;
	public Button startButton;
	public GameObject reticle;

	private Color currentSelectedNormalColor;
	private bool currentSelectedNormalColorValid;
	private Color currentSelectedHighlightedColor;
	public Color selectColor = Color.blue;

	private bool highlighted = false;

	private static float START_TIME = 2.0f;
	private float startButtonTimer = START_TIME; //The length of time the player needs to look at the start button to begin the game

	AsyncOperation async;

	void Start () {
		StartCoroutine("load");
	}


	// Update is called once per frame
	void FixedUpdate () {

		Debug.Log ("In update");
	

		if (highlighted) {


			startButtonTimer -= Time.deltaTime;

			Debug.Log ("Start button time: " + startButtonTimer);


			if (startButtonTimer <= 0) {
				Debug.Log ("StartGame");
				ActivateScene();
			}
		}

		Vector3 fwd = transform.forward;
		if (Physics.Raycast (transform.position, fwd, out hit, rayLength)) {

			if (hit.collider.gameObject.name == "StartButton") {
				if (!highlighted) {
					SetSelectedColor ();
					highlighted = true;

				} 

			} else if (hit.collider.gameObject.name == "Cancel") {
				if (highlighted) {
					RestoreColor ();
					highlighted = false;
					startButtonTimer = START_TIME;
				}

			}
		}
	}

	// sets color of selected UI element and saves current color so it can be restored on deselect
	private void SetSelectedColor() {
	
			if (startButton != null) {
				ColorBlock cb = startButton.colors;
				currentSelectedNormalColor = cb.normalColor;
				currentSelectedNormalColorValid = true;
				currentSelectedHighlightedColor = cb.highlightedColor;
				cb.normalColor = selectColor;
				cb.highlightedColor = selectColor;
				startButton.colors = cb;
			}

	}
	
	// restore color of previously selected UI element
	private void RestoreColor() {
		if (currentSelectedNormalColorValid) {
			if (startButton != null) {
				ColorBlock cb = startButton.colors;
				cb.normalColor = currentSelectedNormalColor;
				cb.highlightedColor = currentSelectedHighlightedColor;
				startButton.colors = cb;
			}
		}
	}

	public void StartLoading() {
		StartCoroutine("load");
	}
	
	IEnumerator load() {
		async = Application.LoadLevelAsync (1);
		async.allowSceneActivation = false;
		yield return async;
		Debug.Log ("Loading complete");
	}
	
	public void ActivateScene() {
		async.allowSceneActivation = true;
	}


}
