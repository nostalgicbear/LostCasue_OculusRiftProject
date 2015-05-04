using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoarScript : Photon.MonoBehaviour {
	private NavMeshAgent agent;
	private GameObject player1;
	private Animation anim;
	private float distanceToPlayer1 = 300; //set these to any value just so they are not null at startup
	private List<GameObject> waypoints = new List<GameObject>();
	private List<AudioClip> boarSounds = new List<AudioClip>();
	private GameObject destination;
	private float distanceToDestination;
	public float rangeOfVision = 50.0f; //how wide the bears cone of vision is
	public float lengthOfVision = 150.0f; //how far the bear can see
	public float inRangeOfAttack = 8.0f; //how close the bear must be before it attacks you
	public float escapeDistance = 160.0f; //how far you must get from the bear before it stops hunting you
	private float health;
	public float damage = -0.002f;
	private Vector3 deathPos;
	Vector3 realPos;
	Quaternion realRot = Quaternion.identity;
	private bool isDead = false;
	private AudioSource audioSource;
	private AudioClip deathClip;
	private RaycastHit hit;
	
	public enum BOAR_STATE{
		WALKING,
		ATTACKING,
		DEAD
	};
	
	public BOAR_STATE state;
	
	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animation> ();
		state = BOAR_STATE.WALKING;
		health = GetComponent<EnemyHealth> ().enemyHealth;
		audioSource = GetComponent<AudioSource> ();
		realPos = transform.position;

		boarSounds.Add(Resources.Load("PigSqueal") as AudioClip);
		boarSounds.Add(Resources.Load("boar") as AudioClip);
		
		/*
		 * Add all waypoints to a List so the bear can then randomly choose a destination
		 */ 
		foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("waypoint")) {
			waypoints.Add(waypoint);
		}
		
		// The bear calls this function to find its  next location to walk to
		ChooseWaypoint ();
	}
	
	// Update is called once per frame
	void Update () {
		
		
		distanceToDestination = Vector3.Distance (transform.position, destination.transform.position); //distance to waypoint
		health = GetComponent<EnemyHealth> ().enemyHealth;
		/*
		 * If player1 and player2 are not null, then we can store the distance to them so the bear can later calculate
		 * which player is closer to him
		 */ 
		if(player1 != null)
		{
			distanceToPlayer1 = Vector3.Distance(transform.position, player1.transform.position);
		}
		
		/*
		 * If the bear has reached his destination, then it can choose another destination. This keeps him moving from
		 * point to point
		 */ 
		if (distanceToDestination <= 3) {
			ChooseWaypoint();
		}
		
		if (health <= 0) {
			deathPos = transform.position;
			state = BOAR_STATE.DEAD;
		}
		
		/*
		 * A bear can be either WALKING, ATTACKING, or DEAD.
		 */ 
		switch (state) {
		case BOAR_STATE.WALKING:
			anim.Play("walk");
			agent.speed = 5; //Set the speed at which the bear walks
			
			/*
			 * If for whatewver reason the destination may be null, I just put in a check here so it will then choose a
			 * new destination. This prevents the bear from being stuck with no destination
			 */ 
			if(destination == null)
			{
				ChooseWaypoint();
			}
			
			agent.SetDestination(destination.transform.position); //Walk to whatever destination was returned from ChooseWaypoint()
			
			if (player1 == null) {
				player1 = GameObject.FindGameObjectWithTag("Player"); 
			}
			
			/*
			 * While the bear is walking, it is possible that it will see a player. The bear has a cone of vision, and if the
			 * player walks in to that cone of vision, and is within the bears range, it has been seen and the bear will
			 * change its state to attack the nearest player
			 */ 
			if (player1 != null) {
				Vector3 betweenPlayer1AndEnemy = player1.transform.position - transform.position;
				
				Vector3 forward = transform.forward;
				float angle = Vector3.Angle (betweenPlayer1AndEnemy, forward);
				
				if (angle < rangeOfVision && distanceToPlayer1 <= lengthOfVision || Vector3.Distance(transform.position, player1.transform.position) <= Random.Range(15,40)) {
					anim.Stop("walk");
					state = BOAR_STATE.ATTACKING;
				}
			} else {
				player1 = GameObject.FindGameObjectWithTag("Player");
			}
			
			
			break;
			
		case BOAR_STATE.ATTACKING:
			agent.speed = 8.0f; //Make the bear faster than when it is walking
			
			if(audioSource != null)
			{
				if(!audioSource.isPlaying){
					audioSource.clip = boarSounds [Random.Range (0, boarSounds.Count)];
					audioSource.Play();
				}
			}
			
			/*
			 * We already store the distance to both players. Here it compares those distances to see which player is 
			 * closer. The bear then runs for the closer player.
			 */ 
			
			agent.SetDestination(player1.transform.position);
			
			Vector3 raycastToClosestPlayer = player1.transform.position - transform.position;
			Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
			/*
			 * If the nearest player is in range of an attack, then attack the player
			 */ 
			if(distanceToPlayer1 <=inRangeOfAttack)
			{
				anim.Stop("run");
				anim.Play("attack1");
				
				if(Physics.Raycast(raycastStartPosition, raycastToClosestPlayer, out hit))
				{
					if(distanceToPlayer1 == distanceToPlayer1)
					{
						if(hit.collider.gameObject.tag == "Player")
						{
							if(player1.GetComponent<VitalBarDriver>() != null)
							{
								player1.GetComponent<VitalBarDriver>().SendMessage("updateHealth", damage, SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				}
				
			}else {
				anim.Stop("4LegsBiteAttack");
				anim.Play("run");
			}
			
			if(distanceToPlayer1 >= escapeDistance) //if the nearest player is after escaping, then go back to walking
			{
				state = BOAR_STATE.WALKING;
			}
			
			
			break;
			
		case BOAR_STATE.DEAD:
			if(audioSource != null){
				if(audioSource.isPlaying){
					if(audioSource.clip.name == "BearGrowl")
					{
						audioSource.Stop();
					}
					
					if(!audioSource.isPlaying)
					{
						deathClip = Resources.Load("BearDeath") as AudioClip;
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
	 * Called to delete the bear once it has been killed
	 */ 
	[RPC]
	IEnumerator Die()
	{
		if (!isDead) {
			anim.Stop("run");
			anim.Stop("walk");
			anim.Stop("4LegsBiteAttack");
			anim.Play("4LegsDeath");
			isDead = true;
		}
		yield return new WaitForSeconds (5.0f);
		Destroy (gameObject);
	}
	
	
	/*
	 * Choose a destination from the waypoint array
	 */ 
	void ChooseWaypoint()
	{
		destination = waypoints [Random.Range (0, waypoints.Count - 1)];
	}
	
	public void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 2.0f);
	}

}
