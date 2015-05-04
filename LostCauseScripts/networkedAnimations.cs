using UnityEngine;
using System.Collections;

public class networkedAnimations : Photon.MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Awake () {
		/*
		 * If the player controls the avatar, then let them access the animator conponenet of that character
		 */ 
		if (photonView.isMine) {
			anim = GetComponent<Animator>();

			if(anim == null)
			{
				Debug.Log("On the Player_Rig component: Cant find animator, trying again");
				anim = GetComponent<Animator>();
			}
		}
	}
	/**
	 * Sends the players animation information across teh network to other players. Without this, otherplayers would
	 * not see animations play on players other than their own. 
	 */ 
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
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
}
