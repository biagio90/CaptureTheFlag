using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	public string enemyTag = "team2";

	public float sight = 15.0f;
	public GameObject bullet;
	
	public float delay = 0.8f;
	
	private float nextTime = 0.0f;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
		if (Time.time > nextTime) {
			nextTime = Time.time + delay;

			Vector3 direction = transform.TransformDirection(Vector3.forward);
			RaycastHit hit = new RaycastHit ();
			Ray ray = new Ray(transform.position, direction);

			for (float angle = -40.0f; angle < 40.0f; angle += 0.5f) {
				ray.direction = Quaternion.Euler(0, angle, 0) * direction;
				if (Physics.Raycast(ray, out hit, sight)) {
					if (hit.collider.tag == enemyTag ) {
						shoot(hit.transform.position);
						return;
					}
				}
			}
		}
	}

	private void shoot (Vector3 enemyPos) {
		Vector3 t = transform.position;
		Vector3 direction = (enemyPos - t).normalized;
		t = t + direction * 1;
		t.y = transform.position.y;
		GameObject shoot = (GameObject) Instantiate(bullet, t, Quaternion.identity);
		Bullet script = shoot.GetComponent<Bullet> ();
		script.destination = enemyPos;
		script.speed = 20;
		script.enemyTag = enemyTag;
		script.go = true;

	}
	/*
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
	*/
}
