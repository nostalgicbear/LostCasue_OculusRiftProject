using UnityEngine;
using System.Collections;

public class ResourceUtil : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void removeItemFromGame(){
		photonView.RPC ("deleteSelf", PhotonTargets.All, null);
	}

	[RPC]
	void deleteSelf(){
		PhotonNetwork.Destroy(gameObject);
	}
}
