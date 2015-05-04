using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StagScript : MonoBehaviour {
	private NavMeshAgent agent;
	private GameObject player1, player2, closestPlayer;
	private Animator anim;
	private float distanceToPlayer1 = 200, distanceToPlayer2 = 200; //set these to any value just so they are not null
	private Vector3 destination; //where the stag is walking to
	private float distanceToDestination; 
	private float distanceToClosestFoe;
	public float rangeOfVision = 100.0f; //how wide the bears cone of vision is
	public float lengthOfVision = 70.0f; //how far the bear can see
	public float inRangeOfAttack = 12.0f; //how close the bear must be before it attacks you
	public float escapeDistance = 50.0f; //how far you must get from the bear before it stops hunting you
	public float startleRange = 20.0f; //If the player doesnt approach from the front, the stag will be alerted once the player is within this range
	public float damage = -0.002f;
	private float health;
	private Vector3 deathPos;
	private bool isDead = false;
	private float stuckTimer = 3.0f;
	private AudioSource audioSource;
	private AudioClip deathClip;
	private bool playingCoroutine = false;
	private RaycastHit hit;
	
	public enum STAG_STATE{
		IDLE,
		WALKING,
		ATTACKING_PLAYER,
		DEAD
	};
	
	public STAG_STATE state;
	
	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		state = STAG_STATE.IDLE;
		health = GetComponent<EnemyHealth> ().enemyHealth;
		audioSource = GetComponent<AudioSource> ();
		
		ChooseWaypoint ();
	}
	
	// Update is called once per frame
	void Update () {
		
		distanceToDestination = Vector3.Distance (transform.position, destination);
		health = GetComponent<EnemyHealth> ().enemyHealth;
		
		/*
		 * If a player is null, find it
		 */ 
		if (player1 == null || player2 == null) {
			player1 = GameObject.FindGameObjectWithTag("Player");
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}
		
		/*
		 * If a reference to both players can be found, then store their distances so they can be used alter if the stag
		 * is attacking
		 */ 
		if(player1 != null && player2 != null)
		{
			distanceToPlayer1 = Vector3.Distance(transform.position, player1.transform.position);
			distanceToPlayer2 = Vector3.Distance(transform.position, player2.transform.position);
		}
		
		if (health <= 0) {
			deathPos = transform.position;
			state = STAG_STATE.DEAD;
		}

		if (state != STAG_STATE.ATTACKING_PLAYER) {
			if(distanceToPlayer1 <= startleRange || distanceToPlayer2 <= startleRange) 
			{
				if(playingCoroutine)
				{
					StopCoroutine("StandIdle");
				}
				state = STAG_STATE.ATTACKING_PLAYER; //If a player gets to close to a stag it attacks
			}
		}
		
		switch (state) {
		case STAG_STATE.IDLE:
			/*
			 * When the stag is IDLE, it calls the function below. This function tells the stag to play the idle animation
			 * for a few seconds, and to then select a point on the navmesh to walk to.
			 */ 
			StartCoroutine("StandIdle");
			

			
			break;
			
		case STAG_STATE.WALKING:
			anim.SetBool("isWalking", true);
			agent.speed = 6;
			
			/*
			 * If for some reason no destination has been set, then set one so the stag has somewhere to walk to
			 */ 
			if(destination == null)
			{
				ChooseWaypoint();
			}
			
			if(agent.velocity != Vector3.zero)
			{
			} else {
				stuckTimer += Time.deltaTime;
			}
			
			if(stuckTimer>=3.0f)
			{
				if(state == STAG_STATE.WALKING){
					Debug.Log("Had to pick another spot");
					ChooseWaypoint();
					stuckTimer = 0.0f;
				}
			}
			
			agent.SetDestination(destination); //walk to the current destination
			
			/*
			 * If the stag has arrived at the destination, then change its state to IDLE. This will cause the stag to 
			 * graze for a few seconds before once again walking to its next destination
			 */ 
			if (distanceToDestination <= inRangeOfAttack) {
				state = STAG_STATE.IDLE;
			}
			
			if (player1 == null) {
				player1 = GameObject.FindGameObjectWithTag("Player"); 
			}
			
			if (player2 == null) {
				player2 = GameObject.FindGameObjectWithTag("Player2"); 
			}
			
			
			/*
		 * If the player is found, calculate the vector between the player and the bear. If this angle is less than the
		 * angle in the 'angle' variable, then the player has been spotted by the bear
		 */ 
			if (player1 != null) {
				Vector3 betweenPlayer1AndEnemy = player1.transform.position - transform.position;
				
				Vector3 forward = transform.forward;
				float angle = Vector3.Angle (betweenPlayer1AndEnemy, forward);
				
				if ((angle < rangeOfVision && distanceToPlayer1 <=lengthOfVision) || (distanceToPlayer1 <= startleRange)) {
					anim.SetBool("isWalking", false);
					state = STAG_STATE.ATTACKING_PLAYER;
					
				}
			} else {
				player1 = GameObject.FindGameObjectWithTag("Player");
			}
			
			if (player2 != null) {
				Vector3 betweenPlayer2AndEnemy = player2.transform.position - transform.position;
				
				Vector3 forward = transform.forward;
				float angle = Vector3.Angle (betweenPlayer2AndEnemy, forward);
				
				if ((angle < rangeOfVision && distanceToPlayer2 <=lengthOfVision) || (distanceToPlayer2 <= startleRange)) {
					anim.SetBool("isWalking", false);
					state = STAG_STATE.ATTACKING_PLAYER;
				}
			} else {
				player2 = GameObject.FindGameObjectWithTag("Player2");
			}
			
			break;
			
		case STAG_STATE.ATTACKING_PLAYER:
			if(!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			
			agent.speed = 8.0f;
			float distanceToClosestFoe = Mathf.Min(distanceToPlayer1, distanceToPlayer2);
			
			if(distanceToClosestFoe == distanceToPlayer1)
			{
				agent.SetDestination(player1.transform.position);
				closestPlayer = player1;
			}
			if(distanceToClosestFoe == distanceToPlayer2)
			{
				agent.SetDestination(player2.transform.position);
				closestPlayer = player2;
			}

			Vector3 raycastToClosestPlayer = closestPlayer.transform.position - transform.position;
			Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
			
			/*
			 * If the nearest player is in range of an attack, then attack the player
			 */ 

			if(distanceToClosestFoe <=inRangeOfAttack)
			{
				if(Physics.Raycast(raycastStartPosition, raycastToClosestPlayer, out hit))
				{
				anim.SetBool("Idle", false);
				anim.SetBool("isRunning", false);
				anim.SetBool("isWalking", false);
				anim.SetTrigger("Attack");
					if(distanceToClosestFoe == distanceToPlayer1){
						if(distanceToClosestFoe >=6){
							RotateTowards(player1.transform);
						}

						if(hit.collider.gameObject.tag == "Player")
						{
							if(player1.GetComponent<VitalBarDriver>() != null)
							{
								player1.GetComponent<VitalBarDriver>().SendMessage("updateHealth", damage, SendMessageOptions.DontRequireReceiver);
							}
						}
					} else {
						if(distanceToClosestFoe >=6){
							RotateTowards(player2.transform);
						}

						if(hit.collider.gameObject.tag == "Player2")
						{
							if(player2.GetComponent<VitalBarDriver>() != null)
							{
								player2.GetComponent<VitalBarDriver>().SendMessage("updateHealth", damage, SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				}
			}

			else{
				anim.SetBool("Idle", false);
				anim.SetBool("isWalking", false);
				anim.SetBool("isRunning", true);
			}
			
			if(distanceToClosestFoe >= escapeDistance)
			{
				anim.SetBool("Idle", true);
				anim.SetBool("isWalking", false);
				anim.SetBool("isRunning", false);
				state = STAG_STATE.IDLE;
			}
			
			break;
			
		case STAG_STATE.DEAD:
			if(audioSource != null){
				if(audioSource.isPlaying){
					if(audioSource.clip.name != "StagDeath")
					{
						audioSource.Stop();
					}
					
					if(!audioSource.isPlaying)
					{
						deathClip = Resources.Load("StagDeath") as AudioClip;
						audioSource.clip = deathClip;
						audioSource.Play();
					}
				}
			}
			
			agent.SetDestination(deathPos);
			StartCoroutine("Die");
			break;
		}
	}
	
	/*
	 * Waits a few seconds so the death animation can complete, and then remove the Stag from the game
	 */ 
	[RPC]
	IEnumerator Die()
	{
		if (!isDead) {
			anim.SetBool("isWalking", false);
			anim.SetBool("isRunning", false);
			anim.SetBool("Idle", false);
			anim.SetTrigger("Die");
			isDead = true;
		}
		yield return new WaitForSeconds (5.2f);
		Destroy (gameObject);
	}
	
	/*
	 * This function tells the stag to choose a waypoint and to then stand idle for a few seconds. After the few seconds 
	 * have passed, the stag will then change its state to WALKING, where it will then proceed to walk to its destination.
	 */
	IEnumerator StandIdle()
	{
		if (!isDead) {
			playingCoroutine = true;
			anim.SetBool ("isWalking", false);
			anim.SetBool ("Idle", true);
			ChooseWaypoint ();
			yield return new WaitForSeconds (7.5f);
			anim.SetBool ("Idle", false);
			anim.SetBool ("isWalking", true);
			state = STAG_STATE.WALKING;
			playingCoroutine = false;
		}
	}
	
	public void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 2.0f);
	}
	
	/*
	 * This tells the stag to calculate a point on the navmesh within the designated radius. It will then proceed to walk
	 * to that point as the result of this calculation is set as its destination
	 */ 
	void ChooseWaypoint()
	{
		float walkRadius = 100;
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
		Vector3 finalPosition = hit.position;
		destination = finalPosition;
	}
}
