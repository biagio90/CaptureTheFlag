using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public string enemyTag;

	public float speed = 10.0f;

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
			//Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			Destroy(other.gameObject);
		}

		if (other.tag != "bullet"){
			Destroy(gameObject);
		}
	}
}
