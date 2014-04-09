using UnityEngine;
using System.Collections;

public class followEnemy : MonoBehaviour {
	public string enemyTag;
	public string enemyFlagTag;

	private PlayerController playerController;

	public float viewLength = 15.0f;
	public float viewAngle  = 50.0f;
	
	public float delay = 0.1f;
	
	private float nextTime = 0.0f;

	private Vector3 enemyFlagOrigin;

	// Use this for initialization
	void Start () {
		playerController = GetComponent<PlayerController> ();
		enemyFlagOrigin = GameObject.FindGameObjectWithTag (enemyFlagTag).transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextTime) {
			nextTime = Time.time + delay;
			
			Vector3 direction = transform.TransformDirection(Vector3.forward);
			RaycastHit hit = new RaycastHit ();
			Ray ray = new Ray(transform.position, direction);
			
			for (float angle = -viewAngle; angle < viewAngle; angle += 1.0f) {
				ray.direction = Quaternion.Euler(0, angle, 0) * direction;
				if (Physics.Raycast(ray, out hit, viewLength)) {
					if(hit.collider.tag == enemyFlagTag ){
					   if( Vector3.Distance(hit.transform.position, 
						                     enemyFlagOrigin) > 1){
							follow(hit.transform.gameObject);
						}
					}
					else{
						if (hit.collider.tag == enemyTag ) {
							follow(hit.transform.gameObject);
							return;
						}
					}
				}
			}
		}
	}

	private void follow(GameObject enemy) {
		if (!playerController.hasFlag) {
			MoveRobotAstar mover = GetComponent<MoveRobotAstar> ();
			mover.follow (enemy);
		}
	}
}
