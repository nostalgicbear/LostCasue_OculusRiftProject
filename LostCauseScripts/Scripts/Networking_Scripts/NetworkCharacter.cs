using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour
{
	Vector3 actualPosition = Vector3.zero;
	Quaternion actualRotation = Quaternion.identity;

	void Awake(){
		if (photonView.isMine) {
			
			GetComponentInChildren<OVRCameraController>	().enabled = true;
		}

	}

	void Start()
	{
		if (photonView.isMine) {

			//GetComponent<OVRCameraController>().enabled = true;
			GetComponent<MouseLook>().enabled = true;
			
			GetComponent<CharacterMotor>().enabled = true;
			GetComponent<FPSInputController>().enabled = true;
			GetComponent<PlayerActions>().enabled = true;
			GetComponent<VitalBarDriver>().enabled = true;
			GetComponent<InventoryScript>().enabled = true;
			GetComponent<DPadButtons>().enabled = true;

		}

	}
	
	void Update()
	{
		if(photonView.isMine)
		{
			//Do nothing. THe character is being moved by us anyway
		}
		else{
			transform.position = Vector3.Lerp(transform.position, actualPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, actualRotation, 0.1f);
		}
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			//This is sending our own player info to other players on the network. Eg our position
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else{
			//receive data related to other players, such as their exact position
			transform.position = (Vector3)stream.ReceiveNext();
			transform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}
}