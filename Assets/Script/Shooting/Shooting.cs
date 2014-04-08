using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	public float sight = 5;
	public GameObject bullet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		Vector3 direction = transform.TransformDirection(Vector3.forward);
		RaycastHit hit = new RaycastHit ();
		Ray ray = new Ray(transform.position, direction);

		for (float angle = -40.0f; angle < 40.0f; angle += 2.0f) {
			ray.direction = Quaternion.Euler(0, angle, 0) * direction;
			if (Physics.Raycast(ray, out hit, sight)) {
				//Debug.Log ( "Collision detected " + hit.collider.tag);
				if (hit.collider.tag == "team2" ) {
					shoot(hit.transform.position);
				}
			}
		}
	}

	private void shoot (Vector3 enemyPos) {
		GameObject shoot = (GameObject) Instantiate(bullet, transform.position, transform.rotation);
		Bullet script = shoot.GetComponent<Bullet> ();
		script.destination = enemyPos;
		script.go = true;

	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
		//Debug.Log (direction);
		//Gizmos.DrawRay(transform.position, direction);

		//Ray ray = new Ray(transform.position, direction);
		for (float angle = -40.0f; angle < 40.0f; angle += 2.0f) {
			//ray.direction = Quaternion.Euler(0, angle, 0) * direction;
			Vector3 d = Quaternion.Euler(0, angle, 0) * direction;
			Gizmos.DrawRay(transform.position, d);
		}

	}
}
