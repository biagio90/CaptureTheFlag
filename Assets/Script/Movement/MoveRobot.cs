using UnityEngine;
using System.Collections;

public class MoveRobot : MonoBehaviour {
	public GameObject destinationObj;

	public int speed = 10;
	private float sight = 5;
	public float turnSpeed = 4;

	private Vector3 destination = Vector3.zero;

	// Use this for initialization
	void Start () {
		destination = destinationObj.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float d = Vector3.Distance (transform.position, destination);
		if (d < 0.5) {
			transform.position = destination;
			rigidbody.velocity =  Vector3.zero;
		} else {
			Vector3 direction = (destination - transform.position).normalized;
			bool c = false;
			direction = avoidCollision (direction, ref c);

			Quaternion _lookRotation = Quaternion.LookRotation (direction);
			//float angle = Vector3.Angle (direction, transform.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);

			rigidbody.velocity = direction * speed;
		}
	}

	
	Vector3 avoidCollision (Vector3 direction, ref bool collision) {
		Ray ray = new Ray(transform.position, direction);
		RaycastHit hit = new RaycastHit ();
		float angleR = 0.0f, angleL = 0.0f;
		//Vector3 dir = new Vector3 (direction);
		
		collision = false;
		
		while (Physics.Raycast(ray, out hit, sight))
		{
			collision = true;
			//Debug.Log ( "Collision detected " + hit.collider.name);
			
			angleL -= 20.0f;
			ray.direction = Quaternion.Euler(0, angleL, 0) * direction;
		}
		
		//dir.Set (direction);
		ray.direction = direction;
		while (Physics.Raycast(ray, out hit, sight))
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
		
		return direction;
	}
}
