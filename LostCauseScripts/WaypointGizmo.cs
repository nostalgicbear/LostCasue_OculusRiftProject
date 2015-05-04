using UnityEngine;
using System.Collections;

public class WaypointGizmo : MonoBehaviour {
	/*
	 * Draws a representation of where the waypoint is. Makes it easy to see in the scene
	 */ 
	void OnDrawGizmosSelected(){
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.position, 1);
	}
}
