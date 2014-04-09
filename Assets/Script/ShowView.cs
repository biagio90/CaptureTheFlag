using UnityEngine;
using System.Collections;

public class ShowView : MonoBehaviour {
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
