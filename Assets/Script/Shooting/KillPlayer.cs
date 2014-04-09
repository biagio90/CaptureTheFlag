using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	public GameObject playerExplosion;
	public Commander commander;
	public GameObject respawn;
	public GameObject flag;

	MoveRobotAstar mover;

	public float timeToRespoun = 5;
	public bool dead = false;

	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
		mover = GetComponent<MoveRobotAstar> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(dead) {
			timer += Time.deltaTime;
			if (timer > timeToRespoun){
				timer = 0;
				dead = false;
			}
		}
	}

	public void killPlayer() {
		Instantiate(playerExplosion, transform.position, transform.rotation);

		if (mover.hasRole) {
			switch(mover.role){
			case "catchFlag": commander.playerAssignedFlag = false;
				break;
			case "helpFlag":  commander.playerAssignedHelp = false;
				break;
			}
			mover.hasRole = false;
			mover.role = "";
		}

		if (mover.hasFlag) {
			Instantiate(flag, transform.position, Quaternion.identity);
			mover.hasFlag = false;
			transform.Find("flag").gameObject.SetActive(false);
		}

		MonoBehaviour[] scripts = GetComponents<MonoBehaviour> ();
		foreach(MonoBehaviour script in scripts) {
			if (script != this) {
				script.enabled = false;
			}
		}
		rigidbody.velocity = Vector3.zero;
		transform.position = respawn.transform.position;
		mover.enabled = true;
		GetComponent<KillPlayer> ().enabled = true;
		GetComponent<Shooting> ().enabled = true;
	}
}
