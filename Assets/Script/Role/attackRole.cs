using UnityEngine;
using System.Collections;

public class attackRole : MonoBehaviour {
//	private bool go = true;
	public Vector3 dest;

	// Use this for initialization
	void Start () {
		dest = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, dest) < 1) {
			float x = Random.Range(-20, 20);
			float z = Random.Range(-20, 20);
			dest = new Vector3(x, 1, z);
			//Debug.Log(dest);
			GetComponent<MoveRobotAstar>().newDestination(dest);
//			go = false;
		}
	}
}
