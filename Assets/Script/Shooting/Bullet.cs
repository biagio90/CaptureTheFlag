using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float speed = 10.0f;

	public Vector3 destination;
	public bool go = false;

	void FixedUpdate () {
		if (go) {
			go = false;
			Vector3 direction = destination - transform.position;
			rigidbody.velocity = direction * speed;
		}
	}

	
	void OnTriggerEnter(Collider other) 
	{
		//	Instantiate(explosion, transform.position, transform.rotation);
		if (other.tag == "team2")
		{
			//Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			Destroy(other.gameObject);
		}
		Destroy(gameObject);
	}
}
