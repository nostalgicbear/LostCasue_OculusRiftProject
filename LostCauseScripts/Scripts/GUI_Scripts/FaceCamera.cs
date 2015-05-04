using UnityEngine;
using System.Collections;

//Rotate the image canvas to face the camera
public class FaceCamera : MonoBehaviour {

	private GameObject CameraRight;

	// Use this for initialization
	void Start () {
		CameraRight = GameObject.Find ("CameraRight");
	}
	
	// Update is called once per frame
	void Update () {

		//If the right camera object is null try and locate it
		if (CameraRight == null) {
			CameraRight = GameObject.Find ("CameraRight");
		} else {
			//Otherwise rotate the text to face the player
			transform.rotation = Quaternion.LookRotation (CameraRight.transform.forward, CameraRight.transform.up);
		}
	}
}
