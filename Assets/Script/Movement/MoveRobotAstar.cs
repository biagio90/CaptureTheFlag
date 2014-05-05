using UnityEngine;
using System.Collections;

public class MoveRobotAstar : MonoBehaviour {
	private enum Movement {Move, Still, Follow, LeaderFollow};
//	private PlayerController playerController;

	private AstarCreator Astar = new AstarCreator(60, 2.0f);

	public float speed = 10f;
	public float turnSpeed = 2f;
	public bool go = false;

	private Movement movement = Movement.Move;

	//MOVE
	private ArrayList pathAstar;
	private int indexAstar = 0;
	private float mainY = 1;
	private Vector3 destinationGlobal;

	// Stand still movement
	private float angle = 0.0f;
//	private float currentAngle = 0.0f;

	// Follow
	private GameObject enemy;

	// Leader
	private GameObject leader;
	public float followDistance = 10.0f;

	// Debug
	private bool drawPath = true;
	private bool no_free_path = false;
	private Vector3 point1;
	private Vector3 point2;


	private float nextTime = 0.0f;

	void Start () {
		mainY = transform.position.y;
//		playerController = GetComponent<PlayerController> ();
	}

	public void follow(GameObject e) {
		movement = Movement.Follow;

		pathAstar = new ArrayList ();
		indexAstar = 0;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		go = false;

		enemy = e;
	}

	public void leaderFollow(GameObject leader) {
		movement = Movement.LeaderFollow;

		this.leader = leader;
	}

	public void standStill(float a) {
		movement = Movement.Still;

		reset ();

		go = false;
		angle = a;
//		currentAngle = 0.0f;
	}

	public void newDestination(Vector3 destination) {
		reset ();
		movement = Movement.Move;
		destinationGlobal = destination;

		pathAstar = Astar.getPath(transform.position, destination);
		//if (!isFree(pathAstar)) Debug.Log("WRONG");

		indexAstar = 0;
		go = true;
	}

	public void reset() {
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		pathAstar = null;
		indexAstar = 0;

	}

	// Update is called once per frame
	void Update () {
		switch (movement) {
		case Movement.Move: move();
			break;
		case Movement.Still: standStillMovement();
			break;
		case Movement.Follow: followMovement();
			break;
		case Movement.LeaderFollow: leaderFollowMovement();
			break;
		}
	}

	private void leaderFollowMovement() {
		if (Time.time > nextTime) {
			nextTime = Time.time + 0.8f;

			Vector3 dest = leader.transform.position;
			if (Vector3.Distance (transform.position, dest) > followDistance) {
				dest = dest + new Vector3(Random.Range(-followDistance/2, followDistance/2),
				                          0, Random.Range(-followDistance/2, followDistance/2));	
				Vector3 step = (dest - transform.position).normalized + transform.position;
				makeMove(step);
				//newDestination(step);
			} else {
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
	}

	private void followMovement() {
		if(enemy == null) return;

		PlayerController enemyController = enemy.GetComponent<PlayerController> ();
		if (enemyController == null) {
			if (Vector3.Distance(transform.position, 
			     enemy.transform.position) > 0.5) {
				makeMove (enemy.transform.position);
			} else {
				enemy = null;
				newDestination(destinationGlobal);
			}
		} else {
			if(!enemyController.dead){
				makeMove (enemy.transform.position);
			} else {
				enemy = null;
				newDestination(destinationGlobal);
			}
		}
	}

	private void standStillMovement() {
		transform.Rotate(0, angle*Time.deltaTime, 0);
	}

	private void move() {
		if(go && pathAstar != null && pathAstar.Count != 0) {
			Vector3 destination = (Vector3) pathAstar[indexAstar];
			//Debug.Log(indexAstar);
			
			destination.y = mainY;
			float d = Vector3.Distance(transform.position, destination);
			if (d > 0.5f) {
				makeMove(destination);
			} else {
				//Debug.Log(index);
				
				transform.position = destination; //snap to destination
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				
				indexAstar++;
				if(indexAstar == pathAstar.Count) {
					indexAstar = 0;
					go = false;
					//avoidOverlappingPlayers(destination);

				}
			}
		}
	}

	private void avoidOverlappingPlayers(Vector3 destination){
		Vector3 up = new Vector3(0, 0, 1);
		Vector3 down = new Vector3(0, 0, -1);
		Vector3 right = new Vector3(1, 0, 0);
		Vector3 left = new Vector3(-1, 0, 0);

		if (!isFree(destination+up)){
			transform.position = destination+up;
			return;
		}
		if (!isFree(destination+down)){
			transform.position = destination+down;
			return;
		}
		if (!isFree(destination+right)){
			transform.position = destination+right;
			return;
		}
		if (!isFree(destination+left)){
			transform.position = destination+left;
			return;
		}
	}

	private void makeMove(Vector3 destination) {
		Vector3 direction = (destination - transform.position).normalized;
		float sight = (destination - transform.position).magnitude;
		bool collision = false;
		//direction = avoidCollision (direction, sight, ref collision);

		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
		
		rigidbody.velocity = direction * speed;
	}

	void OnDrawGizmos() {
		if (drawPath) {
			Gizmos.color = Color.red;

			if (pathAstar != null) {
				for (int i=0; i< pathAstar.Count-1; i++) {
					Vector3 point = (Vector3)pathAstar[i];
					point.x += 1;
					point.z += 1;
					Gizmos.DrawLine((Vector3)pathAstar[i], point);
					Gizmos.DrawLine((Vector3)pathAstar[i], (Vector3)pathAstar[i+1]);
				}
				/*
				if(no_free_path) {
					Vector3 point = point1;
					point.x += 1;
					point.z += 1;
					Gizmos.DrawLine(point1, point);
					point = point2;
					point.x += 1;
					point.z += 1;
					Gizmos.DrawLine(point2, point);
				}*/
			}
		}
	}

	private bool isFree (Vector3 p) {
		//Vector3 p = new Vector3 (pos.x, 1, pos.y);
		//return !Physics.CheckSphere (p, sight);
		
		Collider[] hitColliders = Physics.OverlapSphere(p, 0.5f);
		int i = 0;
		bool collision = false;
		while (i < hitColliders.Length) {
			//hitColliders[i].SendMessage("AddDamage");
			//			Debug.Log(hitColliders[i].name);
			if (hitColliders[i] != this.collider){ 
			    //(hitColliders[i].tag == "team1" ||
			 	 //hitColliders[i].tag == "team2")) {
				collision = true;
			}
			i++;
		}
		
		return !collision;
	}
	
	private bool isFree (ArrayList path) {
		if (pathAstar != null) {
			for (int i=0; i< pathAstar.Count-1; i++) {
				if (!isFree((Vector3)pathAstar[i], (Vector3)pathAstar[i+1])) {
					Debug.Log("Collision: "+pathAstar[i]+" "+pathAstar[i+1]);
					point1 = (Vector3)pathAstar[i];
					point2 = (Vector3)pathAstar[i+1];
					no_free_path = true;
					return false;
				}
			}
		}
		no_free_path = false;
		return true;// && isFreeRightAndLeft(pos2);
	}

	private bool isFree (Vector2 pos1, Vector2 pos2) {
		Vector3 p = new Vector3 (pos1.x, 1, pos1.y);
		Vector3 p2 = new Vector3 (pos2.x, 1, pos2.y);
		Vector3 dir = (p2 - p).normalized;
		float sight = (p2 - p).magnitude;
		
		Ray ray = new Ray(p, dir);
		RaycastHit hit = new RaycastHit ();
		bool collision = Physics.Raycast (ray, out hit, sight);
		
		//bool capsule = Physics.CheckCapsule(p, p2, step);
		//return (!collision && !capsule);
		
		return !collision;// && isFreeRightAndLeft(pos2);
	}

	Vector3 avoidCollision (Vector3 direction, float sight, ref bool collision) {
		Ray ray = new Ray(transform.position, direction);
		RaycastHit hit = new RaycastHit ();
		float angleR = 0.0f, angleL = 0.0f;
		//Vector3 dir = new Vector3 (direction);
		
		collision = false;
		
		while (Physics.Raycast(ray, out hit, sight) && 
		       hit.collider.tag == "wall")
		{
			collision = true;
			//Debug.Log ( "Collision detected " + hit.collider.name);
			
			angleL -= 20.0f;
			ray.direction = Quaternion.Euler(0, angleL, 0) * direction;
		}
		
		//dir.Set (direction);

		ray.direction = direction;
		while (Physics.Raycast(ray, out hit, sight)&& 
		       hit.collider.tag == "wall")
		{
			collision = true;
			//Debug.Log ( "Collision detected " + hit.collider.name);
			
			angleR += 20.0f;
			ray.direction = Quaternion.Euler(0, angleR, 0) * direction;
		}

		if (collision)
			if (Mathf.Abs(angleL) < Mathf.Abs(angleR+20) )
				direction = Quaternion.Euler(0, angleL-20, 0) * direction;
		else 
			direction = Quaternion.Euler(0, angleR+20, 0) * direction;

		/*
		if (collision)
			direction = Quaternion.Euler(0, angleL-20, 0) * direction;
		*/
		return direction;
	}
}
