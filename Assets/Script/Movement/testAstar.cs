using UnityEngine;
using System.Collections;

public class testAstar : MonoBehaviour {
	private MoveRobotAstar mover;
	private Vector3 destination;
	private bool go = false;

	// Use this for initialization
	void Start () {
		mover = GetComponent<MoveRobotAstar> ();
		destination = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			destination = getMouseDirection ();
			destination.y = transform.position.y;
			go = true;
		}

		float d = Vector3.Distance(transform.position, destination);
		if (d > 0.1f) {
			if (go) {
				go = false;
				//Debug.Log(destination);
				mover.newDestination(destination);
			}
		} else {
			rigidbody.velocity=Vector3.zero;
			transform.position = destination;
			go = false;
		}
	}

	public static Vector3 getMouseDirection () {
		Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition)); //create the ray
		RaycastHit hit; //create the var that will hold the information of where the ray hit
		
		if (Physics.Raycast(ray, out hit)){ //did we hit something?
			if (hit.transform.tag == "sourface"){
				return hit.point; //set the destinatin to the vector3 where the ground was contacted
			}
		}
		return Vector3.zero;
	}
}
