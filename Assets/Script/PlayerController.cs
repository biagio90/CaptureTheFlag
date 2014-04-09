using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	//INFO
	public GameObject playerExplosion;
	public Commander commander;
	public GameObject respawn;
	public GameObject flagPrefabs;

	//for commander
	public bool hasFlag = false;
	public int role;

	//for respawn
	public float timeToRespoun = 5;
	public bool dead = false;
	private float timer = 0.0f;

	//script
	MoveRobotAstar mover;
	Shooting shooting;

	//CATCHER
	private bool cameBack = false;
	private bool goForward = false;
	public string flagTag;

	//ATTACKER
	public Vector3 dest;

	//HELPER


	void Start () {
		mover = GetComponent<MoveRobotAstar> ();
		shooting = GetComponent<Shooting> ();

		dest = transform.position;
	}

	void Update () {
		if(dead) {
			timer += Time.deltaTime;
			if (timer > timeToRespoun){
				timer = 0;
				dead = false;
				
				mover.enabled = true;
				GetComponent<Shooting> ().enabled = true;
			}
		} else {
			switch(role) {
			case Roles.Catcher: catcher();
				break;
			case Roles.Helper: helper();
				break;
			case Roles.Attacker: attacker();
				break;
			}
		}
	}

	private void catcher() {
		if (!hasFlag && !goForward) {
			GameObject flag = getFlagCurrent();
			if(flag == null ) {
				killPlayer();
				return;
			}
			mover.newDestination(flag.transform.position);
			goForward = true;
			cameBack = false;
		} else if(hasFlag && !cameBack) {
			mover.newDestination(respawn.transform.position);
			cameBack = true;
			goForward = false;
		}
	}
	
	private void helper() {
		
	}

	private void attacker() {
		if(Vector3.Distance(transform.position, dest) < 1) {
			float x = Random.Range(-20, 20);
			float z = Random.Range(-20, 20);
			dest = new Vector3(x, 1, z);
			mover.newDestination(dest);
		}
	}

	public void killPlayer() {
		Instantiate(playerExplosion, transform.position, transform.rotation);
		
		if (role != Roles.NoRole) {
			switch(role){
			case Roles.Catcher: commander.playerAssignedFlag = false;
				break;
			case Roles.Helper:  commander.playerAssignedHelp = false;
				break;
			}
			role = Roles.NoRole;
		}
		
		if (hasFlag) {
			Instantiate(flagPrefabs, transform.position, Quaternion.identity);
			hasFlag = false;
			transform.Find("flag").gameObject.SetActive(false);
		}

		mover.enabled = false;
		shooting.enabled = false;

		rigidbody.velocity = Vector3.zero;
		transform.position = respawn.transform.position;
		mover.newDestination (respawn.transform.position);
		
		dead = true;
	}

	private GameObject getFlagCurrent() {
		return GameObject.FindGameObjectWithTag(flagTag);
	}
}
