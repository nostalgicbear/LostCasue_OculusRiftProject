using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolHealth : Photon.MonoBehaviour {

	public float health = 1.0f; //Health of the tool (destroy when it reaches 0)
	public Slider healthSlider; //Reference to the slider used to display the health in the inventory screen
	public bool healthUpToDate = false; //Use this when damage is delt so not constantly updating the slider

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!healthUpToDate && healthSlider != null) {
			if(healthSlider.value != health){
				healthSlider.value = health;
			}
		}
	}


	//Need to make a function that acts as an rpc call, that will be called when the tool is dropped
	public void reduceHealth(float damage){
		health -= damage;
		healthUpToDate = false;
	}

	public void setHealth(float newHealth){
		photonView.RPC ("setHealthRPC", PhotonTargets.All, newHealth);//new Vector3(resources.x, resources.y, resources.z));

	}

	//RPC Call to set the health so the value stays consistent accross the server
	[RPC]
	void setHealthRPC(float newHealth){
		health = newHealth;
	}
}
