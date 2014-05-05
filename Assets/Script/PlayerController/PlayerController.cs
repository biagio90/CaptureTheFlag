using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public enum Character {Soldier, Sniper, Scout};
	public enum Strategy  {Circle, Half, Dummy, Neighborhood, LeaderFollower};

	public Character character = Character.Soldier;
	public Strategy strategy;

	private float range_action;
	private float prob_go_enemybase;

	//INFO
	public GameObject playerExplosion;
	public Commander commander;
	public GameObject respawn;
	public GameObject flagPrefabs;
	private GameObject[] myTeam;

	//info on enemybase
	public GameObject enemybase;

	//for commander
	public bool hasFlag = false;
	public int role;

	//for respawn
	public float timeToRespoun = 5;
	public bool dead = false;
	private float timer = 0.0f;

	//script
	public MoveRobotAstar mover;
	public Shooting shooting;

	//CATCHER
	private bool cameBack = false;
	private bool goForward = false;
	public string flagTag;
	public string enemyTag;
	public string enemyFlagTag;

	//ATTACKER
	public Vector3 dest;
	private Vector3 neighborDest;
	private Vector3 neighborTarget;
	private bool neighborOn = false;

	//LEADER FOLLOWER
	public GameObject leader;
	public bool isLeader;

	//HELPER
	GameObject catcherOld;

	// some check
	private Vector3 lastPosition;
	private int countLastPosition;

	void Start () {
		range_action = 20;

		mover = GetComponent<MoveRobotAstar> ();
		shooting = GetComponent<Shooting> ();

		dest = transform.position;
		myTeam = GameObject.FindGameObjectsWithTag (tag);

		lastPosition = transform.position;
	}

	void Update () {
		checkLastPosition ();
		checkBoundary ();

		changeCharater (character);

		if (role == Roles.Catcher) 
			transform.Find("EyeBall").gameObject.SetActive(true);
		else 
			transform.Find("EyeBall").gameObject.SetActive(false);

		if (role == Roles.Helper) 
			transform.Find("WheelBall").gameObject.SetActive(true);
		else 
			transform.Find("WheelBall").gameObject.SetActive(false);

		if(dead) {
			timer += Time.deltaTime;
			if (timer > timeToRespoun){
				timer = 0;
				dead = false;
				
				mover.enabled = true;
				GetComponent<Shooting> ().enabled = true;
			}
		} else {
			switch(role) {
			case Roles.Catcher:  catcher();
				break;
			case Roles.Helper:   helper();
				break;
			case Roles.Attacker: attacker();
				break;
			}
		}
	}

	public void setLeaderToFollow(GameObject leader) {
		this.leader = leader;
		mover.leaderFollow (leader);
	}

	public void setAsLeader(Vector3 dest) {
		isLeader = true;
		mover.newDestination (dest);
		Debug.Log (dest);
	}

	private void checkLastPosition () {
		if (!dead && lastPosition == transform.position) {
			countLastPosition++;
			if (countLastPosition >= 10) {
				switch(role){
				case Roles.Catcher: 
					goForward = false;
					cameBack = false;
					catcher();
					break;
				case Roles.Helper: helper();
					break;
				case Roles.Attacker: 
					dest = transform.position;
					attacker();
					break;
				}
			}
		} else {
			countLastPosition = 0;
			lastPosition = transform.position;
		}

	}

	private void checkBoundary (){
		Vector3 pos = transform.position;
		if (pos.x < -27 || pos.x > 27 || pos.z > 20 || pos.z < -20) {
			killPlayer();
		}
	}

	public void changeRole(int newRole) {
		// reset
		role = newRole;
		//mover.reset ();
		switch(role){
		case Roles.Catcher: 
			goForward = false;
			cameBack = false;
			break;
		case Roles.Helper:
			break;
		case Roles.Attacker:
			break;
		}
	}

	private void catcher() {
		if (!hasFlag && !goForward) {
			GameObject flag = getFlagCurrent();
			if(flag == null ) {
				killPlayer();
				return;
			}
			mover.newDestination(flag.transform.position);
			goForward = true;
			cameBack = false;
		} else if(hasFlag && !cameBack) {
			mover.newDestination(respawn.transform.position);
			cameBack = true;
			goForward = false;
		}
	}
	
	private void helper() {
		/*
		GameObject catcher = getCatcherObject ();
		if(catcher!=null){// && catcher != catcherOld){
			catcherOld = catcher;
			//mover.follow (catcher);
			transform.position = catcher.transform.position+new Vector3(-2, 0, 2);
		}*/
		attacker ();
	}

	private void attacker() {
		Vector3 target;
		if (Vector3.Distance (transform.position, dest) < 1) {
			switch(strategy) {
			case Strategy.Circle:
				if(getFlagEnemyCurrent()!=null){
					target = getFlagEnemyCurrent().transform.position;
					range_action = 20;
				} else {
					target = enemybase.transform.position; //go towards the enemy base at half way
					range_action = 10; //I don't want them to be too much away from that point
				}

				bool good = false;
				while(!good){
		            dest = randomfromflag (range_action,target);
					if (dest.x > -27 && dest.x < 27 && dest.z > -20 && dest.z < 20) {
						good = true;
					}
				}     
				//dest = randomfromflag (20,Vector3.zero);
				mover.newDestination (dest);
			break;

			case Strategy.Half:
				if (Vector3.Distance (transform.position, 
				                      enemybase.transform.position) > Vector3.Distance (transform.position, respawn.transform.position)){
					prob_go_enemybase = 80;}
				else {
					prob_go_enemybase = 10;
				}
				bool good2 = false;
				while(!good2){
					dest = getrandomposition (prob_go_enemybase, range_action);
					if (dest.x > -27 && dest.x < 27 && dest.z > -20 && dest.z < 20) {
						good2 = true;
					}
				}
				mover.newDestination (dest);
				break;

			case Strategy.Dummy:
				//PURE SQUARE MATRIX RANDOM MOVEMENT
				float x = Random.Range (-20, 20);
				float z = Random.Range (-19, 19);
				dest = new Vector3 (x, 1, z);
				mover.newDestination (dest);
				break;
			
			
			case Strategy.Neighborhood:
				//Debug.Log("dest: "+dest+" neighbor: "+neighborDest);
				if(!neighborOn){
					x = Random.Range (-20, 20);
					z = Random.Range (-19, 19);
					dest = new Vector3 (x, 1, z);
					mover.newDestination (dest);
				} else {
					if(Vector3.Distance(neighborDest, transform.position)<0.5){
						Vector3 direction = (neighborTarget - transform.position).normalized;
						Quaternion _lookRotation = Quaternion.LookRotation (direction);
						transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 5 * Time.deltaTime);
						//float angle = Vector3.Angle(direction, transform.forward);
						//Quaternion t = Quaternion.Euler(0, angle, 0);
						//transform.rotation = Quaternion.Slerp(transform.rotation, t, Time.deltaTime);

					}
				}
				break;
			case Strategy.LeaderFollower:
				if (!isLeader){
					GameObject newLeader = null;
					foreach(GameObject player in myTeam) {
						PlayerController pc = player.GetComponent<PlayerController>();
						if(pc.strategy == Strategy.LeaderFollower &&
						   pc.isLeader){
							newLeader = player;
							break;
						}
					}
					if (newLeader != leader){
						mover.leaderFollow (newLeader);
					}
				}
				break;
			}
		}
	}

	public void setNeighborDest(Vector3 dest, Vector3 t) {
		neighborDest = dest;
		neighborTarget = t;
		neighborOn = true;
	}

	public void killPlayer() {
		Instantiate(playerExplosion, transform.position, transform.rotation);
		
		if (role != Roles.NoRole) {
			switch(role){
			case Roles.Catcher: commander.playerAssignedFlag = false;
				//commander.decreaseNumCatcher();
				break;
			case Roles.Helper:  commander.playerAssignedHelp = false;
				//commander.decreaseNumHelper();
				break;
			case Roles.Attacker: //commander.decreaseNumAttacker();
				if (isLeader) {
					isLeader = false;
					GameObject newLeader = null;
					foreach(GameObject player in myTeam) {
						PlayerController pc = player.GetComponent<PlayerController>();
						if(pc.strategy == Strategy.LeaderFollower){
							newLeader = player;
							break;
						}
					}
					commander.setNewLeader(newLeader);
				}
				break;
			}
			role = Roles.NoRole;
		}
		
		if (hasFlag) {
			Instantiate(flagPrefabs, transform.position, Quaternion.identity);
			transform.Find("flag").gameObject.SetActive(false);
		}
		hasFlag = false;

		mover.enabled = false;
		shooting.enabled = false;

		transform.position = respawn.transform.position;
		mover.reset ();
		
		dead = true;
	}

	public void changeCharater(Character character) {
		switch (character) {
		case Character.Soldier: 
			shooting.viewAngle = 40;
			shooting.viewLength = 12;
			shooting.killProbability = 80;
			break;
		case Character.Sniper: 
			shooting.viewAngle = 20;
			shooting.viewLength = 20;
			shooting.killProbability = 100;
			break;
		case Character.Scout: 
			shooting.viewAngle = 60;
			shooting.viewLength = 7;
			shooting.killProbability = 60;
			break;

		}
	}

	public void catchTheFLag() {
		hasFlag = true;
		if (role != Roles.Catcher) {
			Debug.Log("change role from "+role+" to "+Roles.Catcher);
			changeRole(Roles.Catcher);
			changeRoleCatcherToAttacker();
		}
	}

	private GameObject getFlagCurrent() {
		return GameObject.FindGameObjectWithTag(flagTag);
	}
	private GameObject getFlagEnemyCurrent() {
		return GameObject.FindGameObjectWithTag(enemyFlagTag);
	}


	private void changeRoleCatcherToAttacker() {
		foreach (GameObject player in myTeam) {
			if(player != this) {
				PlayerController playerController = player.GetComponent<PlayerController>();
				if(playerController.role == Roles.Catcher){
					playerController.changeRole(Roles.Attacker);
					return;
				}
			}
		}
	}

	private GameObject getCatcherObject() {
		foreach (GameObject player in myTeam) {
			if(player != this) {
				PlayerController playerController = player.GetComponent<PlayerController>();
				if(playerController.role == Roles.Catcher){
					return player;
				}
			}
		}
		return null;
	}

	Vector3	randomfromflag (float range_action,Vector3 center){
	//I KNOW THAT THE CENTER OF THE MAP IS AT 0,0,0
		Vector3 enemybasedir = Vector3.Normalize(enemybase.transform.position);
		Quaternion quat;
		float angle = Random.Range(0,360);
		//Debug.Log ("Angle :"+angle);
		float distance = Random.Range(0,range_action);
		quat = Quaternion.AngleAxis(angle,enemybasedir);
		enemybasedir = quat * enemybasedir;
		enemybasedir = enemybasedir * distance;
		//Debug.Log("Destination
		return (enemybasedir);
		
	}

	Vector3	getrandomposition (float prob_go_enemybase,float range_action){
		Vector3 direction = Vector3.Normalize (transform.forward);
//		Vector3 enemybasedir = enemybase.transform.position - transform.position;
		Quaternion quat;
		float angle = Random.Range(0,360);
		//Debug.Log ("Angle :"+angle);
		float distance = Random.Range(0,range_action);
		if(Mathf.FloorToInt(Random.Range(0,100)) < prob_go_enemybase){
			//angle = Vector3.Angle(direction,enemybasedir);
			//quat = Quaternion.AngleAxis(angle,direction);
			direction = (enemybase.transform.position - transform.position).normalized;
			distance = range_action;
		}
		else{
			//angle = Random.Range(0,360);
			quat = Quaternion.AngleAxis(angle,direction);
			direction = quat * direction;
		}
		
		direction = direction * distance;
		//Debug.Log("Destination
		return (direction + transform.position);
		
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		//Gizmos.DrawRay (transform.position, transform.forward);
	}
}
