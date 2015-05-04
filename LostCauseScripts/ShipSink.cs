using UnityEngine;
using System.Collections;

public class ShipSink : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	/*
	 * This script causes the ship at teh start of the game to sink. It slowly rotates it on 2 axis whilst simultaneously
	 * lowering its position, thus giving the illusuion that it is sinking
	 */ 
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate (Time.deltaTime, 1.2f * Time.deltaTime, 0, Space.World);
		transform.position = new Vector3 (transform.position.x, transform.position.y - 0.01f, transform.position.z);

		if (transform.position.y <= -16) {
			Destroy(gameObject);
		}
	}
}
