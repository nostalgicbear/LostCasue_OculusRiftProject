using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoxScript : Photon.MonoBehaviour {
	private NavMeshAgent agent;
	private GameObject player1, player2, closestPlayer;
	private Animator anim;
	private float distanceToPlayer1 = 200, distanceToPlayer2 = 200; //set these to any value just so they are not null
	private Vector3 destination;
	private float distanceToDestination;
	public float rangeOfVision = 80.0f; //how wide the bears cone of vision is
	public float lengthOfVision = 70.0f; //how far the bear can see
	public float inRangeOfAttack = 5.0f; //how close the bear must be before it attacks you
	public float escapeDistance = 80.0f; //how far you must get from the bear before it stops hunting you
	private float health;
	private Vector3 positionAtLastPacket = Vector3.zero;
	//private Vector3 pos;
//	private Quaternion rot;
	public float damage = -0.002f;
	private Vector3 deathPos;
	private bool isDead = false;
	private float stuckTimer = 0.0f;
	private AudioSource audioSource;
	private AudioClip deathClip;
	private RaycastHit hit;
	private int updatedFrames = 0;

	public double interpolationBackTime = 0.05;
	
	internal struct State
	{
		internal double timestamp;
		internal Vector3 pos;
		internal Quaternion rot;
	}
	
	// We store twenty states with "playback" information
	State[] m_BufferedState = new State[20];
	// Keep track of what slots are used
	int m_TimestampCount;


	public enum FOX_STATE{
		WALKING,
		ATTACKING_RABBIT,
		ATTACKING_PLAYER,
		DEAD
	};
	
	public FOX_STATE state;
	private FOX_STATE actualState;

	void Awake()
	{
		if (photonView.isMine)
			this.enabled = false;//Only enable inter/extrapol for remote players

		PhotonNetwork.sendRate = 30;
		PhotonNetwork.sendRateOnSerialize = 30;
	}
	
	// Use this for initialization
	void Start () {
		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		health = GetComponent<EnemyHealth> ().enemyHealth;
		state = FOX_STATE.WALKING;
		audioSource = GetComponent<AudioSource> ();
		//pos = transform.position;
		// rot = transform.rotation;
		
		ChooseWaypoint ();
		
	}
	
	// Update is called once per frame
	void Update () {

			//--------------------------------------------------------------------------------------

			double currentTime = PhotonNetwork.time;
			double interpolationTime = currentTime - interpolationBackTime;
		
			if (m_BufferedState [0].timestamp > interpolationTime) {

				Debug.Log ("Interpolating");


				for (int i = 0; i < m_TimestampCount; i++) {
					// Find the state which matches the interpolation time (time+0.1) or use last state
					if (m_BufferedState [i].timestamp <= interpolationTime || i == m_TimestampCount - 1) {
						// The state one slot newer (<100ms) than the best playback state
						State rhs = m_BufferedState [Mathf.Max (i - 1, 0)];
						// The best playback state (closest to 100 ms old (default time))
						State lhs = m_BufferedState [i];
						
						// Use the time between the two slots to determine if interpolation is necessary
						double length = rhs.timestamp - lhs.timestamp;
						float t = 0.0F;
						// As the time difference gets closer to 100 ms t gets closer to 1 in 
						// which case rhs is only used
						if (length > 0.0001)
							t = (float)((interpolationTime - lhs.timestamp) / length);
						
						// if t=0 => lhs is used directly
						transform.localPosition = Vector3.Lerp (lhs.pos, rhs.pos, t);
						transform.localRotation = Quaternion.Slerp (lhs.rot, rhs.rot, t);
						return;
					}
				}
			}
			// Use extrapolation. Here we do something really simple and just repeat the last
			// received state. You can do clever stuff with predicting what should happen.
			else {
				Debug.Log ("Extrapolating");

				State latest = m_BufferedState [0];

				Debug.Log ("Moving too latest.pos: " + latest.pos + "from local position: " + transform.localPosition );
				
				if(latest.pos != Vector3.zero){

					transform.localPosition = Vector3.Lerp (transform.localPosition, latest.pos, Time.deltaTime * 20);
					transform.localRotation = latest.rot;

				} else {
					transform.localPosition = Vector3.Lerp (transform.localPosition, transform.localPosition, Time.deltaTime * 20);
					transform.localRotation = latest.rot;
				}
			}


		//----------------------------------------------------------------------------------------------------------------------//
		
		distanceToDestination = Vector3.Distance (transform.position, destination); //distance to whatever spot the fox is going to
		health = GetComponent<EnemyHealth> ().enemyHealth;
		/*
		 * If eeither player is null, look for the players
		 */ 
		if (player1 == null || player2 == null) {
			player1 = GameObject.FindGameObjectWithTag("Player");
			player2 = GameObject.FindGameObjectWithTag("Player2");
		}
		
		/*
		 * If the players are not null, then store the distance between this fox and each player
		 */ 
		if(player1 != null && player2 != null)
		{
			distanceToPlayer1 = Vector3.Distance(transform.position, player1.transform.position);
			distanceToPlayer2 = Vector3.Distance(transform.position, player2.transform.position);
		}
		
		/*
		 * If teh fox has reached its destination, then choose a new destination
		 */ 
		if (distanceToDestination <= 9) {
			ChooseWaypoint();
		}
		
		if (health <= 0) {
			deathPos = transform.position;
			state = FOX_STATE.DEAD;
		}
		
		switch (state) {
		case FOX_STATE.WALKING:
			anim.SetBool("isWalking", true);
			agent.speed = 3; //walk at a slow enough speed
			
			if(destination == null)
			{
				ChooseWaypoint();
			}
			
			agent.SetDestination(destination);
			
			if (player1 == null) {
				player1 = GameObject.FindGameObjectWithTag("Player"); 
			}
			
			if (player2 == null) {
				player2 = GameObject.FindGameObjectWithTag("Player2"); 
			}
			
			
			/*
			 * While the fox is walking, it is possible that it will see a player. The fox has a cone of vision, and if the
			 * player walks in to that cone of vision, and is within the foxs range, it has been seen and the fox will
			 * change its state to attack the nearest player
			 */ 
			if (player1 != null) {
				Vector3 betweenPlayer1AndEnemy = player1.transform.position - transform.position;
				
				Vector3 forward = transform.forward;
				float angle = Vector3.Angle (betweenPlayer1AndEnemy, forward);
				
				if (angle < rangeOfVision && distanceToPlayer1 <= lengthOfVision || Vector3.Distance(transform.position, player1.transform.position) <= Random.Range(12,40)) {
					anim.SetBool("isWalking", false);
					state = FOX_STATE.ATTACKING_PLAYER;
					
				}
			} else {
				player1 = GameObject.FindGameObjectWithTag("Player");
			}
			
			if (player2 != null) {
				Vector3 betweenPlayer2AndEnemy = player2.transform.position - transform.position;
				
				Vector3 forward = transform.forward;
				float angle = Vector3.Angle (betweenPlayer2AndEnemy, forward);
				
				if (angle < rangeOfVision && distanceToPlayer2 <=lengthOfVision || Vector3.Distance(transform.position, player2.transform.position) <= Random.Range(12,40)) {
					anim.SetBool("isWalking", false);
					state = FOX_STATE.ATTACKING_PLAYER;
					
				}
			} else {
				player2 = GameObject.FindGameObjectWithTag("Player2");
			}
			
			if(agent.velocity != Vector3.zero)
			{
			} else {
				stuckTimer += Time.deltaTime;
			}
			
			if(stuckTimer>=3.0f)
			{
				Debug.Log("Had to pick another spot");
				ChooseWaypoint();
				stuckTimer = 0.0f;
			}
			
			break;
			
		case FOX_STATE.ATTACKING_PLAYER:
			
			if(!audioSource.isPlaying){
				audioSource.Play();
			}
			
			agent.speed = 8.0f;
			float distanceToClosestFoe = Mathf.Min(distanceToPlayer1, distanceToPlayer2);
			
			
			if(distanceToClosestFoe == distanceToPlayer1 && player1 != null)
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
				anim.SetTrigger("bite");
				
				
				if(Physics.Raycast(raycastStartPosition, raycastToClosestPlayer, out hit))
				{
					if(distanceToClosestFoe == distanceToPlayer1){
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
				
				
			}else{
				anim.SetBool("isRunning", true);
			}
			
			/*
			 * If the player has escaped then go back to walknig
			 */ 
			if(distanceToClosestFoe >= escapeDistance)
			{
				state = FOX_STATE.WALKING;
			}
			
			break;
			
		case FOX_STATE.DEAD:
			if(audioSource != null){
				if(audioSource.isPlaying){
					if(audioSource.clip.name == "FoxAttack")
					{
						audioSource.Stop();
					}
					
					if(!audioSource.isPlaying)
					{
						deathClip = Resources.Load("FoxDeath") as AudioClip;
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
	
	[RPC]
	IEnumerator Die()
	{
		if (!isDead) {
			anim.SetBool("isRunning", false);
			anim.SetBool("isWalking", false);
			anim.SetTrigger("Die");
			isDead = true;
		}
		yield return new WaitForSeconds (5.0f);
		Destroy (gameObject);
	}
	
	public void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 2.0f);
	}
	
	/*
	 * Choose a random destination on the navmesh and then go to that destination
	 */ 
	void ChooseWaypoint()
	{
		float walkRadius = 300;
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
		Vector3 finalPosition = hit.position;
		destination = finalPosition;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{ 
		Debug.Log ("OnPhotonSerializeView");

			// Always send transform (depending on reliability of the network view)
			if (stream.isWriting) {


				Vector3 pos = transform.localPosition;
				Quaternion rot = transform.localRotation;
				stream.Serialize (ref pos);
				Debug.Log ("Stream sending position: " + pos);

				stream.Serialize (ref rot);
			}
			// When receiving, buffer the information
			else {
				// Receive latest state information

				Vector3 pos = Vector3.zero;
				Quaternion rot = Quaternion.identity;
				stream.Serialize (ref pos);
				//Debug.Log("Recieved serialiazed pos: " + pos);
				stream.Serialize (ref rot);
			
				// Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
				for (int i = m_BufferedState.Length - 1; i >= 1; i--) {
					m_BufferedState [i] = m_BufferedState [i - 1];
				}
				
				Debug.Log ("Stream recieve next position = " + pos);
			
				// Save currect received state as 0 in the buffer, safe to overwrite after shifting
				State state;
				state.timestamp = info.timestamp;
				state.pos = pos;
				state.rot = rot;
				m_BufferedState [0] = state;
			
				// Increment state count but never exceed buffer size
				m_TimestampCount = Mathf.Min (m_TimestampCount + 1, m_BufferedState.Length);
			
				// Check integrity, lowest numbered state in the buffer is newest and so on
				for (int i = 0; i < m_TimestampCount - 1; i++) {
					if (m_BufferedState [i].timestamp < m_BufferedState [i + 1].timestamp)
						Debug.Log ("State inconsistent");
				}
			}
		}


}
