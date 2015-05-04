using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

	public Transform fwd;
	public Transform rightCamera;

	private Animator anim;
	private float verticalMovement, horizontalMovement;

	private AudioSource walkingAudio;
	private bool audioPlaying = false;

	// Use this for initialization
	void Start () {
		//Access the animator associated with the player rig
		anim = GameObject.Find ("Player_Rig").GetComponent<Animator> ();
		audioPlaying = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (walkingAudio == null) {
			walkingAudio = GetComponent<AudioSource> ();
		} 

		//Rotate the player model to face where the player is looking
		Vector3 relativePos = new Vector3(fwd.position.x, transform.position.y, fwd.position.z ) - transform.position;
		transform.rotation = Quaternion.LookRotation (relativePos);
		transform.position = rightCamera.transform.position;

		verticalMovement = Input.GetAxis("Vertical");
		horizontalMovement = Input.GetAxis("Horizontal");

		if (!audioPlaying && (verticalMovement != 0.0f || horizontalMovement != 0.0f)) {
			walkingAudio.Play();
			audioPlaying = true;
		} else if (audioPlaying && (verticalMovement == 0.0f && horizontalMovement == 0.0f)) {
			walkingAudio.Stop();
			audioPlaying = false;
		}

		anim.SetFloat ("walk", verticalMovement); //Update the walk parameter in the player animator
		anim.SetFloat ("turn", horizontalMovement); //Update the walk parameter in the player animator

	}
}
