using UnityEngine;
using System.Collections;

public class ButtonScript : Photon.MonoBehaviour {
	public bool activated = false;
	private AudioSource audioSource;
	private AudioSource leverOn, leverOff;

	void Start(){
		audioSource = GetComponent<AudioSource> ();
		AudioSource[] audios = GetComponents<AudioSource> ();
		leverOn = audios [0];
		leverOff = audios [1];
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			activated = true;
			photonView.RPC("Green", PhotonTargets.All, null);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			activated = false;
			photonView.RPC("Red", PhotonTargets.All, null);
		}
	}

	[RPC]
	public void Red()
	{
		activated = false;
		GetComponent<Renderer> ().material.color = Color.red;
		transform.parent.GetComponent<Animation>().CrossFade("down");
		leverOff.Play ();
	}

	[RPC]
	public void Green()
	{
		activated = true;
		GetComponent<Renderer> ().material.color = Color.green;
		transform.parent.GetComponent<Animation>().CrossFade("up");
		leverOn.Play ();
		
	}
}
