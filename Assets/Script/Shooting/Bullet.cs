using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 destination;
	public bool go = false;

	void FixedUpdate () {
		if (go) {
			go = false;
			Vector3 direction = destination - transform.position;
			rigidbody.velocity = direction * 10;
		}
	}
}
