using UnityEngine;
using System.Collections;

public class SpikeTrapSide : Photon.MonoBehaviour {
	public GameObject spikeToActivate;
	private AudioSource spikeAudioSource;

	void Start() {
		spikeAudioSource = spikeToActivate.GetComponent<AudioSource> ();
	}
	
	/**
	 * If a player steps on the tile, then call the RPC that plays the animation that pushes out the spike. Calling this
	 * via an RPC so all players see the animation play
	 */ 
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			photonView.RPC("ActivateSideSpike", PhotonTargets.All, null);
		}
	}

	/**
	 * When someone steps off the tile, call the RPC that makes the spike go back in to the wall. Called as an RPC so all
	 * players see the action
	 */ 
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") {
			photonView.RPC("DeactivateSideSpike", PhotonTargets.All, null);
		}
	}

	/*
	 * An RPC that plays an animation where a spike comes out the side of the wall
	 */
	[RPC]
	void ActivateSideSpike(){
		spikeToActivate.GetComponent<Animation>().CrossFade("Dragon_needlel_Default");
		if (!spikeAudioSource.isPlaying) {
			spikeAudioSource.Play();
		}
	}

	/*
	 * An RPC that plays an animation where a spike goes back in the side of the wall
	 */
	[RPC]
	void DeactivateSideSpike(){
		spikeToActivate.GetComponent<Animation>().CrossFade("Dragon_needlel_Default_back");
	}
}
