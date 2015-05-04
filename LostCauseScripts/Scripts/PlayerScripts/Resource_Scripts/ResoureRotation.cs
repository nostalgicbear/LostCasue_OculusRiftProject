using UnityEngine;
using System.Collections;

public class ResoureRotation : MonoBehaviour {

	public float rotateSpeed = 10f;
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	}
}
