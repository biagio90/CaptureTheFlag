using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour {
	public GameController gameController;

	public int team;

	public float respownArea = 5;

	private GameObject[] myTeam;

	public string flagTag;
	private Vector3 flagPos;

	public GameObject flagPrefabs;

	public GameObject baseMyTeam;
	public GameObject baseEnemyTeam;

	public bool playerAssignedFlag = false;
	public bool playerAssignedHelp = false;

	private bool firstUpdate = true;

	void Start () {/*
		if(team == 1){
			myTeam = GameObject.FindGameObjectsWithTag ("team1");
		} else {
			myTeam = GameObject.FindGameObjectsWithTag ("team2");
		}
		flagPos = getFlagObject ().transform.position;
		*/
	}

	void Update () {
		if (firstUpdate) {
			firstUpdate = false;
			if(team == 1){
				myTeam = GameObject.FindGameObjectsWithTag ("team1");
			} else {
				myTeam = GameObject.FindGameObjectsWithTag ("team2");
			}
			flagPos = getFlagObject ().transform.position;
		}

		if(!checkCatcher()) {
			GameObject closest = findClosestToFlag();
			if(closest != null ) {
				PlayerController playerController = closest.GetComponent<PlayerController>();
				playerController.changeRole(Roles.Catcher);
				playerAssignedFlag = true;
			}
		}

		foreach (GameObject player in myTeam) {
			PlayerController playerController = player.GetComponent<PlayerController>();
			if(!playerController.dead) {
				
				bool isRespawnZone = inRespawnZone(player);

				if(isRespawnZone && playerController.hasFlag) {
					gameController.increaseScore(team);
					playerController.hasFlag = false;
					player.transform.Find("flag").gameObject.SetActive(false);
					Instantiate(flagPrefabs, flagPos, Quaternion.identity);
				}

				if(playerController.role == Roles.NoRole){
					if (isRespawnZone) {
						if(!playerAssignedFlag) {
							playerController.role = Roles.Catcher;
							playerAssignedFlag = true;
						} else if(!playerAssignedHelp) {
							playerAssignedHelp = true;
							playerController.role = Roles.Helper;
						} else {
							playerController.dest = player.transform.position;
							playerController.role = Roles.Attacker;
						}
					}
				}
			}
		}
	}

	private bool checkCatcher() {
		foreach (GameObject player in myTeam) {
			PlayerController playerController = player.GetComponent<PlayerController>();
			if (playerController.role == Roles.Catcher) return true;
		}
		return false;
	}

	private bool inRespawnZone(GameObject player) {
		return Vector3.Distance (player.transform.position, 
		                         baseMyTeam.transform.position) < respownArea;
	}

	private GameObject findClosestToFlag() {
		Vector3 flagPos = getFlagObject ().transform.position;
		GameObject closest = null;
		float minDistance = 10000;
		foreach (GameObject player in myTeam) {
			PlayerController playerController = player.GetComponent<PlayerController>();
			if (!playerController.dead) {
				float dist = Vector3.Distance(player.transform.position,
				                              flagPos);
				if (dist < minDistance){
					minDistance = dist;
					closest = player;
				}
			}
		}
		return closest;
	}
	
	private GameObject getFlagObject() {
		return GameObject.FindGameObjectWithTag(flagTag);
	}
}
