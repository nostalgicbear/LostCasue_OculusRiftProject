using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PigScript : Photon.MonoBehaviour {
	private NavMeshAgent agent;
	private GameObject player1, player2;
	private Animation anim;
	private Vector3 destination;
	private float range = 100.0f; //used for when the pig is running away from players. If distance is greater than 100 then the player is out of range
	private float distanceToPlayer1 = 50, distanceToPlayer2 = 50; //set these to any value just so they are not null
	private float health;
	private Vector3 deathPos;
	private Vector3 pos;
	private Quaternion rot;
	private bool isDead = false;
	private AudioSource audioSource;
	private AudioClip deathClip;
	private List<AudioClip> pigSounds = new List<AudioClip>();
	private RaycastHit hit;
	private float raycastLength = 10.0f;
	/*
	 * Pigs are the weak creatures that run away from players. They have 3 states
	 * */
	public enum PIG_STATE {
		IDLE,
		RUNNING_AWAY,
		DEAD
	};

	public PIG_STATE state;
	private PIG_STATE actualState;

	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animation> ();
		state = PIG_STATE.IDLE;
		health = GetComponent<EnemyHealth> ().enemyHealth;
		audioSource = GetComponent<AudioSource> ();
		pos = transform.position;
		rot = transform.rotation;

		pigSounds.Add(Resources.Load("PigSqueal") as AudioClip);
		pigSounds.Add(Resources.Load("PigSqueal2") as AudioClip);

		PhotonNetwork.sendRate = 40;
		PhotonNetwork.sendRateOnSerialize = 40;
	}
	
	// Update is called once per frame
	void Update () {

			if (PhotonNetwork.isMasterClient) {
				//Dont do anything cz we are controlling ourselves on our own machine
			} else {
			if(pos != null)
			{
				transform.position = Vector3.Lerp(transform.position, pos, 0.1f);
			}
			if(rot != null)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1.0f);
			}
			if(actualState != null)
			{
				state = actualState;
			}
		}


			health = GetComponent<EnemyHealth> ().enemyHealth;
			/*
		 * Here I do a quick check incase the player wasnt found during the Start() function. If the player exists, the distance
		 * to the player is calculated. This is also calculated for player 2. If the either player is null, I just look for the 
		 * player again
		 */ 
			if (player1 != null) {
				distanceToPlayer1 = Vector3.Distance (transform.position, player1.transform.position);
			} else {
				player1 = GameObject.FindGameObjectWithTag ("Player");
			}

			if (player2 != null) {
				distanceToPlayer2 = Vector3.Distance (transform.position, player2.transform.position);
			} else {
				player2 = GameObject.FindGameObjectWithTag ("Player2");
			}

			if (health <= 0) {
				deathPos = transform.position;
				state = PIG_STATE.DEAD;
			}

			Vector3 fwd = transform.forward;
			Vector3 raycastStartPosition = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);

			if (Physics.Raycast (raycastStartPosition, fwd, out hit, raycastLength)) {
				if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "Player2") {
					RunToNewSpot ();
				}
			}

			switch (state) {
			/*
			 * If the pig is idle, play the idle animation, If either player one or player two is close to the pig, it changes its
			 * state to run away.
			 * */
			case PIG_STATE.IDLE:
				anim.Play ("_idle1");
				agent.SetDestination (transform.position); //set its destination to its current position

			//If either player one or player two is close to the pig, it changes its state to run away.
				if (distanceToPlayer1 != null && distanceToPlayer2 != null) {
					/*
				 * If either player is within range of the pig, it runs away
				 */ 
					if (distanceToPlayer1 <= Random.Range (2, 30) || distanceToPlayer2 <= Random.Range (20, 70)) {
						if (!audioSource.isPlaying) {
							audioSource.clip = pigSounds [Random.Range (0, pigSounds.Count)];
							audioSource.Play ();
						}

						RunToNewSpot ();
						state = PIG_STATE.RUNNING_AWAY;
					}
				} else {
				}
				break;
			/*
			 * For the pig to change to this state, a player must have gotten within range of him. The pig plays the 
			 * animation for running away, and runs to a a random point on the map.
			 */ 
			case PIG_STATE.RUNNING_AWAY:

				anim.Play ("_run");

			/*
			 * When a pig is Idle, and a player approaches, it calls the RunToNewSpot() function and then it changes
			 * its state to RUNNING_AWAY. Here I do a check so if for some reason it hasnt calculated a new spot to run to,
			 * it will do it here.
			 */ 
				if (destination == null) {
					RunToNewSpot ();
				}

				agent.SetDestination (destination); // Set the pigs destination to whatever was returned from the RunToNewSpot() function()

				if (Vector3.Distance (transform.position, destination) <= 5) {
					state = PIG_STATE.IDLE;
				}


			/*
			 * If the pig is far enough away from the players, change the state to idle. 
			 */ 
				if (distanceToPlayer1 >= range && distanceToPlayer2 >= range) {
					state = PIG_STATE.IDLE;
				}

				break;
			case PIG_STATE.DEAD:
				if (audioSource != null) {
					if (!audioSource.isPlaying) {
						deathClip = Resources.Load ("PigDeath") as AudioClip;
						audioSource.clip = deathClip;
						audioSource.Play ();
					}
				}

				agent.SetDestination (deathPos);
				StartCoroutine ("Die");
				break;
			}
	}

	[RPC]
	IEnumerator Die()
	{
		if (!isDead) {
			anim.Play("_death");
			isDead = true;
		}

		yield return new WaitForSeconds (5.0f);
		Destroy (gameObject);
	}

	/*
	 * Thisis called when a player gets close to a pig. This calculates a random point on the nav mesh. It draws a sphere
	 * from the centre of the pig and then picks a random point on that sphere and tells the pig to run to it. 
	 */ 
	void RunToNewSpot()
	{
		float walkRadius = 100;
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
		Vector3 finalPosition = hit.position;
		destination = finalPosition;
	}


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(state);
			
		}
		else {
			pos = (Vector3)stream.ReceiveNext();
			rot = (Quaternion)stream.ReceiveNext();
			actualState = (PIG_STATE)stream.ReceiveNext();
		}
	}

}
