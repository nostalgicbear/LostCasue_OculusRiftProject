
using UnityEngine;
using System.Collections;

// This class maps the DPad axis to buttons.
public class DPadButtons : MonoBehaviour {
	public static bool up;
	public static bool down;
	public static bool left;
	public static bool right;
	
	float lastX;
	float lastY;
	
	void Start() {
		up = down = left = right = false;
		lastX = Input.GetAxis("DpadX");
		lastY = Input.GetAxis("DpadY");
	}
	
	void Update() {
		if(Input.GetAxis ("DpadX") == 1 && lastX != 1) { right = true; } else { right = false; }
		if(Input.GetAxis ("DpadX") == -1 && lastX != -1) { left = true; } else { left = false; }
		if(Input.GetAxis ("DpadY") == 1 && lastY != 1) { up = true; } else { up = false; }
		if(Input.GetAxis ("DpadY") == -1 && lastY != -1) { down = true; } else { down = false; }
	}
}