using UnityEngine;
using System.Collections;

/*
 * This is placed on the spike that will damage the player / enemy if it hits them
 */ 
public class SpikeDamage : MonoBehaviour {
	private float damage = -0.04f;

	/*
	 * If a player gets hit with a spike, then reduce the players health by the amount specified.
	 * If an enemy gets hit by a spike, kill them instantly
	 */ 
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			other.gameObject.GetComponent<VitalBarDriver> ().updateHealth (damage);

		} else if (other.gameObject.tag == "enemy" || other.gameObject.tag == "wolf") {
			other.gameObject.GetComponent<EnemyHealth>().enemyHealth -= 110;
		}
	}
}
