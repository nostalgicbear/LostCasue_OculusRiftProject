using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WolfScript : MonoBehaviour {
	private GameObject player1;
	private GameObject player2, closestPlayer;
	private float distanceToPlayer1 = 500;
	private float distanceToPlayer2 = 500;
	private Animator anim;
	private NavMeshAgent agent;
	private Vector3 destination;
	private float distanceToDestination;
	//private GameObject healthManager;
	private float health;
	public float damage = -0.002f;
	public float rangeOfView = 50.0f;
	public float escapeDistance = 70.0f;
	public float inRangeOfDestination = 12.0f;
	public float rangeOfCall = 200.0f;
	private List<GameObject> otherWolves = new List<GameObject>();
	private Vector3 deathPos;
	private bool isDead = false;
	float stuckTimer = 0.0f;
	private AudioSource audioSource;
	private AudioClip deathSound;
	private RaycastHit hit;

	public enum WOLF_STATE {
		WALKING,
		ATTACKING,
		DEAD
	};
	
	public WOLF_STATE state;
	
	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		health = GetComponent<EnemyHealth>().enemyHealth; //gets the enemies health from the ENEMYHEALTH script
		state = WOLF_STATE.WALKING;
		audioSource = GetComponent<AudioSource> ();
		//	healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");
		
		/*
		 * Here I add all the WOLVES in the level to a list. This is used to call other wolves when a wolf sees the player
		 */ 
		foreach(GameObject otherWolf in GameObject.FindGameObjectsWithTag("wolf"))
		{
			otherWolves.Add(otherWolf);
		}
		
		ChooseWaypoint (); //Picks a random destination for the wolf to travel to
		
	}
	
	// Update is called once per frame
	void Update () {

		
		distanceToDestination = Vector3.Distance (transform.position, destination);

		health = GetComponent<EnemyHealth>().enemyHealth;
		
		/*
		 * If the players are not null, then store the distance between this fox and each player
		 */
		if (player1 != null && player2 != null) {
			distanceToPlayer1 = Vector3.Distance (transform.position, player1.transform.position);
			distanceToPlayer2 = Vector3.Distance (transform.position, player2.transform.position);
		} else {
			player1 = GameObject.FindGameObjectWithTag("Player");
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}
		
		float distanceToClosestFoe = Mathf.Min(distanceToPlayer2, distanceToPlayer1); //calculates which player is closer


		if (health <= 0) {
			deathPos = transform.position;
			state = WOLF_STATE.DEAD;
		}
		
		switch(state) {
		case WOLF_STATE.WALKING:
			
			/*
			 * When WALKING the wolf is traveling to its destination. If for some reason it doesnt have one, then I tell it
			 * to pick one
			 */ 
			if(destination == null)
			{
				ChooseWaypoint();
			}
			
			/*
			 * If the wolf reaches its destination, then pick another one 
			 */ 
			if (distanceToDestination <= inRangeOfDestination) {
				ChooseWaypoint();
			}
			
			agent.SetDestination(destination); //go to the chosen destination


			if(agent.velocity != Vector3.zero) {

			} else {
				stuckTimer += Time.deltaTime;
			}

			if(stuckTimer>=3.0f)
			{
				Debug.Log("Had to pick another spot");
				ChooseWaypoint();
				stuckTimer = 0.0f;
			}
			
			/*
			 * If a wplayer gets too close to a wolf, then thewolf will attack. It will notify all other wolves of where 
			 * the player was sighted, and then it changes its state to attack.
			 */ 
			if(distanceToClosestFoe <= rangeOfView)
			{
				CallOtherWolves();
				state = WOLF_STATE.ATTACKING;
			}
			
			break;
			
			
		case WOLF_STATE.ATTACKING:

			if(audioSource != null){
				if(!audioSource.isPlaying)
				{
					audioSource.Play();
				}
			}

			/*
			 * If the wolf enters the ATTACKING state then it has spotted a player. The wolf will determine which player
			 * is closer and then go for that player
			 */ 
			if(distanceToPlayer1 == distanceToClosestFoe)
			{
				destination = player1.transform.position;
				closestPlayer = player1;
			}
			
			if(distanceToPlayer2 == distanceToClosestFoe)
			{
				destination = player2.transform.position;
				closestPlayer = player2;
			}

			Vector3 raycastToClosestPlayer = closestPlayer.transform.position - transform.position;
			Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

			if(Physics.Raycast(raycastStartPosition, raycastToClosestPlayer, out hit))
			{
				if(distanceToClosestFoe <= inRangeOfDestination)
				{
					anim.SetTrigger("Bite");
					if(distanceToClosestFoe == distanceToPlayer1)
					{
						if(distanceToClosestFoe >=5){
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
						if(distanceToClosestFoe >=5){
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
			
			
			agent.SetDestination(destination);
			
			if(distanceToClosestFoe >= escapeDistance)
			{
				state = WOLF_STATE.WALKING;
			}
			
			break;
			
			/*
			 * If this state has been triggered, the plant has been killed.
			 */ 
		case WOLF_STATE.DEAD:
			if(audioSource != null){
				if(audioSource.isPlaying){
					if(audioSource.clip.name != "WolfDeath")
					{
						audioSource.Stop();
					}

					if(!audioSource.isPlaying)
					{
						deathSound = Resources.Load("wolfDeath") as AudioClip;
						audioSource.clip = deathSound;
						audioSource.Play();
					}
				}
			}
			agent.SetDestination(deathPos);
			//GetComponent<Collider>().enabled = false;
			StartCoroutine("Die");
			break;
		}
	}
	
	/*
	 * This waits for 3 seconds and then destroys the enemy.
	 */ 
	[RPC]
	IEnumerator Die()
	{
		if (!isDead) {
			anim.SetBool("IDLE", false);
			anim.SetBool("Walking", false);
			anim.SetBool("AttackFoe", false);
			anim.SetTrigger("Die");
			isDead = true;
		}
		yield return new WaitForSeconds(5.01f);
		Destroy(gameObject);
	}
	
	/*
	 * A wolf has spotted the player. He calls the other wolves. If the other wolves are not already attaking the player and 
	 * are within range, then they are told to go to where the player is spotted.
	 */ 
	void CallOtherWolves()
	{
		Vector3 sightedPlayer = transform.position;
		foreach (GameObject wolf in otherWolves) {
			if(wolf.gameObject != null)
			{
				if(wolf.GetComponent<WolfScript>().state != WOLF_STATE.ATTACKING && Vector3.Distance(transform.position, wolf.transform.position) <=rangeOfCall)
				{
					wolf.GetComponent<WolfScript>().state = WOLF_STATE.WALKING;
					wolf.GetComponent<WolfScript>().destination = sightedPlayer;
					wolf.GetComponent<NavMeshAgent>().SetDestination(sightedPlayer);
				}
			}
		}
	}

	public void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 2.0f);
	}
	
	/*
	 * Choose a random point on the navmesh and set that point as your destination
	 */ 
	void ChooseWaypoint()
	{
		float walkRadius = Random.Range(100, 1000);
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
		Vector3 finalPosition = hit.position;
		
		destination = finalPosition;
	}
	
}
