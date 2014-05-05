using UnityEngine;
using System.Collections;

public class ShootingRL : MonoBehaviour {
	public string enemyTag = "team2";
	
	public float viewLength = 15.0f;
	public float viewAngle  = 50.0f;
	public int killProbability = 100;
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
			
			for (float angle = -viewAngle; angle < viewAngle; angle += 1.0f) {
				ray.direction = Quaternion.Euler(0, angle, 0) * direction;
				if (Physics.Raycast(ray, out hit, viewLength)) {
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
		//script.speed = 40;
		script.enemyTag = enemyTag;
		script.killProbability = killProbability;
		script.go = true;
		
	}
}
