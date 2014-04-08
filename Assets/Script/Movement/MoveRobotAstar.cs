using UnityEngine;
using System.Collections;

public class MoveRobotAstar : MonoBehaviour {
	public bool hasFlag = false;
	public bool hasRole = false;
	public string role;

	private AstarCreator Astar = new AstarCreator(55, 2);

	public float speed = 10f;
	public float turnSpeed = 2f;
	public bool go = false;
	
	private ArrayList pathAstar;
	private int indexAstar = 0;
	private float mainY = 1;

	// Use this for initialization
	void Start () {
		mainY = transform.position.y;
	}

	public void newDestination(Vector3 destination) {
		pathAstar = Astar.getPath(transform.position, destination);
		pathAstar.Reverse();
		pathAstar.Add(destination);
		indexAstar = 0;
		go = true;
	}

	// Update is called once per frame
	void Update () {
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

}
