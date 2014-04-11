﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public enum Character {Soldier, Sniper, Scout};
	public enum Strategy  {Circle, Half, Dummy};

	public Character character = Character.Soldier;
	public Strategy strategy;

	private float range_action;
	private float prob_go_enemybase;

	//INFO
	public GameObject playerExplosion;
	public Commander commander;
	public GameObject respawn;
	public GameObject flagPrefabs;
	private GameObject[] myTeam;

	//info on enemybase
	public GameObject enemybase;

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
	public string enemyTag;
	public string enemyFlagTag;

	//ATTACKER
	public Vector3 dest;

	//HELPER

	// some check
	private Vector3 lastPosition;
	private int countLastPosition;

	void Start () {
		range_action = 20;

		mover = GetComponent<MoveRobotAstar> ();
		shooting = GetComponent<Shooting> ();

		dest = transform.position;
		myTeam = GameObject.FindGameObjectsWithTag (tag);

		lastPosition = transform.position;
	}

	void Update () {
		checkLastPosition ();
		checkBoundary ();

		changeCharater (character);

		if (role == Roles.Catcher) 
			transform.Find("EyeBall").gameObject.SetActive(true);
		else 
			transform.Find("EyeBall").gameObject.SetActive(false);

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
			case Roles.Catcher:  catcher();
				break;
			case Roles.Helper:   helper();
				break;
			case Roles.Attacker: attacker();
				break;
			}
		}
	}

	private void checkLastPosition () {
		if (!dead && lastPosition == transform.position) {
			countLastPosition++;
			if (countLastPosition >= 10) {
				switch(role){
				case Roles.Catcher: 
					goForward = false;
					cameBack = false;
					catcher();
					break;
				case Roles.Helper: helper();
					break;
				case Roles.Attacker: 
					dest = transform.position;
					attacker();
					break;
				}
			}
		} else {
			countLastPosition = 0;
			lastPosition = transform.position;
		}

	}

	private void checkBoundary (){
		Vector3 pos = transform.position;
		if (pos.x < -27 || pos.x > 27 || pos.z > 20 || pos.z < -20) {
			killPlayer();
		}
	}

	public void changeRole(int newRole) {
		// reset
		role = newRole;
		//mover.reset ();
		switch(role){
		case Roles.Catcher: 
			goForward = false;
			cameBack = false;
			break;
		case Roles.Helper:
			break;
		case Roles.Attacker:
			break;
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
		attacker ();
	}

	private void attacker() {
		Vector3 target;
		if (Vector3.Distance (transform.position, dest) < 1) {
			switch(strategy) {
			case Strategy.Circle:
				if(getFlagEnemyCurrent()!=null){
					target = getFlagEnemyCurrent().transform.position;
					range_action = 20;
				} else {
					target = enemybase.transform.position; //go towards the enemy base at half way
					range_action = 10; //I don't want them to be too much away from that point
				}

				bool good = false;
				while(!good){
		            dest = randomfromflag (range_action,target);
					if (dest.x > -27 && dest.x < 27 && dest.z > -20 && dest.z < 20) {
						good = true;
					}
				}     
				//dest = randomfromflag (20,Vector3.zero);
				mover.newDestination (dest);
			break;
			case Strategy.Half:
				if (Vector3.Distance (transform.position, 
				                      enemybase.transform.position) > Vector3.Distance (transform.position, respawn.transform.position)){
					prob_go_enemybase = 80;}
				else {
					prob_go_enemybase = 10;
				}
				bool good2 = false;
				while(!good2){
					dest = getrandomposition (prob_go_enemybase, range_action);
					if (dest.x > -27 && dest.x < 27 && dest.z > -20 && dest.z < 20) {
						good2 = true;
					}
				}
				mover.newDestination (dest);
				break;
			case Strategy.Dummy:
				//PURE SQUARE MATRIX RANDOM MOVEMENT
				float x = Random.Range (-20, 20);
				float z = Random.Range (-20, 20);
				dest = new Vector3 (x, 1, z);
				mover.newDestination (dest);
				break;

			}
		}
	}

	public void killPlayer() {
		Instantiate(playerExplosion, transform.position, transform.rotation);
		
		if (role != Roles.NoRole) {
			switch(role){
			case Roles.Catcher: commander.playerAssignedFlag = false;
				//commander.decreaseNumCatcher();
				break;
			case Roles.Helper:  commander.playerAssignedHelp = false;
				//commander.decreaseNumHelper();
				break;
			case Roles.Attacker: //commander.decreaseNumAttacker();
				break;
			}
			role = Roles.NoRole;
		}
		
		if (hasFlag) {
			Instantiate(flagPrefabs, transform.position, Quaternion.identity);
			transform.Find("flag").gameObject.SetActive(false);
		}
		hasFlag = false;

		mover.enabled = false;
		shooting.enabled = false;

		transform.position = respawn.transform.position;
		mover.reset ();
		
		dead = true;
	}

	public void changeCharater(Character character) {
		switch (character) {
		case Character.Soldier: 
			shooting.viewAngle = 40;
			shooting.viewLength = 12;
			break;
		case Character.Sniper: 
			shooting.viewAngle = 20;
			shooting.viewLength = 20;
			break;
		case Character.Scout: 
			shooting.viewAngle = 60;
			shooting.viewLength = 7;
			break;

		}
	}

	public void catchTheFLag() {
		hasFlag = true;
		if (role != Roles.Catcher) {
			Debug.Log("change role from "+role+" to "+Roles.Catcher);
			changeRole(Roles.Catcher);
			changeRoleCatcherToAttacker();
		}
	}

	private GameObject getFlagCurrent() {
		return GameObject.FindGameObjectWithTag(flagTag);
	}
	private GameObject getFlagEnemyCurrent() {
		return GameObject.FindGameObjectWithTag(enemyFlagTag);
	}


	private void changeRoleCatcherToAttacker() {
		foreach (GameObject player in myTeam) {
			if(player != this) {
				PlayerController playerController = player.GetComponent<PlayerController>();
				if(playerController.role == Roles.Catcher){
					playerController.changeRole(Roles.Attacker);
					return;
				}
			}
		}
	}

	Vector3	randomfromflag (float range_action,Vector3 center){
	//I KNOW THAT THE CENTER OF THE MAP IS AT 0,0,0
		Vector3 enemybasedir = Vector3.Normalize(enemybase.transform.position);
		Quaternion quat;
		float angle = Random.Range(0,360);
		//Debug.Log ("Angle :"+angle);
		float distance = Random.Range(0,range_action);
		quat = Quaternion.AngleAxis(angle,enemybasedir);
		enemybasedir = quat * enemybasedir;
		enemybasedir = enemybasedir * distance;
		//Debug.Log("Destination
		return (enemybasedir);
		
	}

	Vector3	getrandomposition (float prob_go_enemybase,float range_action){
		Vector3 direction = Vector3.Normalize (transform.forward);
//		Vector3 enemybasedir = enemybase.transform.position - transform.position;
		Quaternion quat;
		float angle = Random.Range(0,360);
		//Debug.Log ("Angle :"+angle);
		float distance = Random.Range(0,range_action);
		if(Mathf.FloorToInt(Random.Range(0,100)) < prob_go_enemybase){
			//angle = Vector3.Angle(direction,enemybasedir);
			//quat = Quaternion.AngleAxis(angle,direction);
			direction = (enemybase.transform.position - transform.position).normalized;
			distance = range_action;
		}
		else{
			//angle = Random.Range(0,360);
			quat = Quaternion.AngleAxis(angle,direction);
			direction = quat * direction;
		}
		
		direction = direction * distance;
		//Debug.Log("Destination
		return (direction + transform.position);
		
	}
}
