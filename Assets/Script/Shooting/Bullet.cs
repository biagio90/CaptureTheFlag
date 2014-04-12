using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
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
				other.gameObject.GetComponent<PlayerController>().killPlayer();
			}
		}

		if (other.tag != "bullet" && other.tag != "flagTeam1" && other.tag != "flagTeam2" ){
			Destroy(gameObject);
		}
	}
}
