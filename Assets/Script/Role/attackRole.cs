using UnityEngine;
using System.Collections;

public class attackRole : MonoBehaviour {
	private bool go = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(go) {
			float x = Random.Range(-20, 20);
			float z = Random.Range(-20, 20);
			Vector3 dest = new Vector3(x, 1, z);
			Debug.Log(dest);
			GetComponent<MoveRobotAstar>().newDestination(dest);
			go = false;
		}
	}
}
