using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {
	
	private bool isAlive = true;
	private Vector3 position;
	private Quaternion rotation;
	float lerpSmoothing = 5f;
	//public GameObject vitalbarCanvas;
	public GameObject cameraLeft;
	public GameObject cameraRight;
	//public GameObject inventoryCanvas;
	
	// Use this for initialization
	void Awake () {
		/*
		 * Plyer controlled locally
		 */ 
		if(photonView.isMine)
		{
			//GetComponent<OVRCameraController>().enabled = true;
			GetComponent<MouseLook>().enabled = true;

			cameraLeft.SetActive(true);
			cameraRight.SetActive(true);

			GetComponent<CharacterMotor>().enabled = true;
			GetComponentInChildren<OVRCameraController>().enabled = true;
			GetComponent<FPSInputController>().enabled = true;
			GetComponent<PlayerActions>().enabled = true;
			GetComponent<VitalBarDriver>().enabled = true;
			GetComponent<InventoryScript>().enabled = true;
			GetComponent<DPadButtons>().enabled = true;

			
		}
		else{
			StartCoroutine("Alive");
		}
		
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();
		}
	}
	
	//while alive, so this state machine
	IEnumerator Alive()
	{
		while(isAlive)
		{
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSmoothing);
			
			yield return null;
		}
	}
	
}
