using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConstructItem : Photon.MonoBehaviour {

	public int woodRequired, stoneRequired, grassRequired;
	public int woodRecieved, stoneRecieved, grassRecieved = 0;
	public string objectName; //This is the name of the object to be constructed (either hut, firepit or raft);
	private bool textUpdated = false;
	public bool canCreate = false;
	public Text resourceUpdate;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!textUpdated) {
			if(resourceUpdate != null){

				resourceUpdate.text = "W o o d :  ' "+woodRecieved+" '  o u t  o f   ' "+woodRequired+" '\n" + 
				"S t o n e :  ' "+stoneRecieved+" '  o u t  o f   ' "+stoneRequired+" '\n" + "G r a s s :  ' "+grassRecieved+" '  o u t  o f   ' "+grassRequired+" '";

				textUpdated = true;
			}
		}

	}

	public void addResources(Vector3 resources){

		photonView.RPC ("updateResources", PhotonTargets.All, resources);//new Vector3(resources.x, resources.y, resources.z));

	}

	public Vector3 getResourcesRequired(){
		return new Vector3 ((woodRequired-woodRecieved),(stoneRequired-stoneRecieved),(grassRequired-grassRecieved));
	}
	

	//Create the finished gameobject in the scene once all the resource requirements have been met
	public void createObject(){
		Debug.Log ("Creating " + objectName);
		PhotonNetwork.Instantiate(objectName, transform.position, Quaternion.identity, 0);

		photonView.RPC ("deleteSelf", PhotonTargets.All, null);
	}

	[RPC]
	void updateResources(Vector3 resources){
		woodRecieved += (int) resources.x;
		stoneRecieved += (int) resources.y;
		grassRecieved += (int) resources.z;

		if ((woodRecieved >= woodRequired) && (stoneRecieved >= stoneRequired) && (grassRecieved >= grassRequired)) {
			
			canCreate = true;
		}

		textUpdated = false;
	}

	[RPC]
	void deleteSelf(){
		PhotonNetwork.Destroy(gameObject);
	}	
}
