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

	void Start () {
		if(team == 1){
			myTeam = GameObject.FindGameObjectsWithTag ("team1");
		} else {
			myTeam = GameObject.FindGameObjectsWithTag ("team2");
		}
		flagPos = getFlagObject ().transform.position;
	}

	void Update () {
		foreach (GameObject player in myTeam) {
			PlayerController playerController = player.GetComponent<PlayerController>();
			if(!playerController.dead) {
				
				bool isRespawnZone = inRespawnZone(player);
//				MoveRobotAstar mover = player.GetComponent<MoveRobotAstar>();

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

	private bool inRespawnZone(GameObject player) {
		return Vector3.Distance (player.transform.position, 
		                         baseMyTeam.transform.position) < respownArea;
	}

	private GameObject getFlagObject() {
		return GameObject.FindGameObjectWithTag(flagTag);
	}
}
