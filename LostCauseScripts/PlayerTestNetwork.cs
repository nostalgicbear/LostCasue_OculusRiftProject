using UnityEngine;
using System.Collections;

public class PlayerTestNetwork : Photon.MonoBehaviour {

	private bool isAlive = true;
	private Vector3 position;
	private Quaternion rotation;
	float lerpSmoothing = 0.1f;
	public GameObject cameraLeft;
	public GameObject cameraRight;
//	public GameObject inventoryCanvas;
//	public GameObject vitalBarCanvas;
	public Animator anim;
	//private float walkSpeed, turnSpeed, choppingSpeed, mineSpeed, actionSpeed = 0.0f;
//	public GameObject head;


	// Use this for initialization
	void Awake () {
		if(photonView.isMine)
		{
//			
			//		GetComponent<OVRCameraController>().enabled = true;
			GetComponent<MouseLook>().enabled = true;
			GetComponent<CharacterMotor>().enabled = true;
			GetComponent<FPSInputController>().enabled = true;

			cameraRight.SetActive(true);
			cameraLeft.SetActive(true);


			GetComponent<AudioDriver>().enabled = true;
			GetComponent<VitalBarDriver>().enabled = true;

			GetComponent<InventoryScript>().enabled = true;
			GetComponent<ConstructionScript>().enabled = true;
			GetComponent<RaftManager>().enabled = true;
			GetComponent<DeathScript>().enabled = true;


			cameraRight.GetComponent<PlayerActions>().enabled = true;
			GetComponent<CharacterController>().enabled = true;
			GetComponent<DPadButtons>().enabled = true;

			GetComponent<AudioSource>().enabled = true;

			//transform.Find("OVRCameraController").localPosition = new Vector3(0,-0.8f,0);


			anim.enabled = true;

			if(anim == null){
				Debug.Log("The animator component is null leaving the Awake function");
			}

		}
		else{
			anim.enabled = true;
			StartCoroutine("Alive");
		}
	
	}
	
	 
	void Update(){

		if (photonView.isMine) {
			//nothing
		} else {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSmoothing);
		}
	}



	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			if(anim !=null)
			{
				stream.SendNext(anim.GetFloat("walk"));
				stream.SendNext(anim.GetFloat("turn"));
				stream.SendNext(anim.GetFloat("chopping"));
				stream.SendNext(anim.GetFloat("mining"));
				stream.SendNext(anim.GetFloat("action"));
			} else {
				Debug.Log("no animator found");
			}
		}
		else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();
			if(anim != null)
			{
				anim.SetFloat("walk", (float)stream.ReceiveNext());
				anim.SetFloat("turn", (float)stream.ReceiveNext());
				anim.SetFloat("chopping", (float)stream.ReceiveNext());
				anim.SetFloat("mining", (float)stream.ReceiveNext());
				anim.SetFloat("action", (float)stream.ReceiveNext());
			} else {
				Debug.Log("no animator found");
			}
		}
	}

	
	//while alive, so this state machine
	IEnumerator Alive()
	{
		while(isAlive)
		{
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSmoothing);
		//	Mathf.Lerp(anim.GetFloat("walk"), walkSpeed, 0.1f);
			
			yield return null;
		}
	}
}
