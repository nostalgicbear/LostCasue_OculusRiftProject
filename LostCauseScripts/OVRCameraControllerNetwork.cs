using UnityEngine;
using System.Collections;

public class OVRCameraControllerNetwork : Photon.MonoBehaviour {
	
	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			//Dont do anything cz we are controlling ourselves on our own machine
		} else {
			transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y + 1.0f, position.z), 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2.0f);

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
}
