using UnityEngine;
using System.Collections;

public class MoveRobotAstar : MonoBehaviour {
	private enum Movement {Move, Still, Follow};
//	private PlayerController playerController;

	private AstarCreator Astar = new AstarCreator(55, 2);

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

	// Debug
	private bool drawPath = true;

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

	public void standStill(float a) {
		movement = Movement.Still;

		pathAstar = new ArrayList ();
		indexAstar = 0;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;

		go = false;
		angle = a;
//		currentAngle = 0.0f;
	}

	public void newDestination(Vector3 destination) {
		movement = Movement.Move;
		destinationGlobal = destination;

		pathAstar = Astar.getPath(transform.position, destination);

		indexAstar = 0;
		go = true;
	}

	public void reset() {
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		pathAstar = new ArrayList ();
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
				}
			}
		}
	}

	private void makeMove(Vector3 destination) {
		Vector3 direction = (destination - transform.position).normalized;
		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
		
		rigidbody.velocity = direction * speed;
	}

	void OnDrawGizmos() {
		if (drawPath) {
			Gizmos.color = Color.green;

			if (pathAstar != null) {
				for (int i=0; i< pathAstar.Count-1; i++) {
					Gizmos.DrawLine((Vector3)pathAstar[i], (Vector3)pathAstar[i+1]);
				}
			}
		}
	}
}
