using UnityEngine;
using System.Collections;

public class catchFlagRole : MonoBehaviour {
	public GameObject flag;
	private Vector3 flagPos;
	public GameObject basePos;

	private MoveRobotAstar mover;
	private bool cameBack = false;
	private bool goForward = false;

	// Use this for initialization
	void Start () {
		mover = GetComponent<MoveRobotAstar> ();
		flagPos = flag.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (!mover.hasFlag && !goForward) {
			mover.newDestination(flagPos);
			//Debug.Log("Va verso la bandiera");
			goForward = true;
			cameBack = false;
		} else if(mover.hasFlag && !cameBack) {
			mover.newDestination(basePos.transform.position);
			cameBack = true;
			goForward = false;
		}
	}
}
