using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public GameObject playerShooter;
	public string enemyTag;
	public int killProbability;

	public float speed;

	public Vector3 destination;
	public bool go = false;

	void FixedUpdate () {
		if (go) {
			go = false;
			Vector3 direction = (destination - transform.position).normalized;
			rigidbody.velocity = direction * speed;
		}
	}

	
	void OnTriggerEnter(Collider other) 
	{
		//	Instantiate(explosion, transform.position, transform.rotation);
		if (other.tag == enemyTag)
		{
			int probability = Random.Range(0, 100);
			if(probability < killProbability){
				PlayerController pc = other.gameObject.GetComponent<PlayerController>();
				if (pc != null) {
					pc.killPlayer();
				} else {
					PlayerRL rl = other.gameObject.GetComponent<PlayerRL>();
					rl.killPlayer();
					PlayerRL rlShooter = playerShooter.GetComponent<PlayerRL>();
					rlShooter.killedEnemy(rl.hasFlag);
				}
			}
		}

		if (other.tag != "bullet" && other.tag != "flagTeam1" && other.tag != "flagTeam2" ){
			Destroy(gameObject);
		}
	}
}
