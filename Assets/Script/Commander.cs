using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour {
	public GameController gameController;

	public int team;

	public float respownArea = 3;

	private GameObject[] myTeam;
//	private GameObject[] enemyTeam;
	public string flagTag;
	private Vector3 flagPos;

	public GameObject flagPrefabs;

	public GameObject baseMyTeam;
	public GameObject baseEnemyTeam;

	public bool playerAssignedFlag = false;
	public bool playerAssignedHelp = false;

	//private ArrayList rolesToAssigne = new ArrayList();

	// Use this for initialization
	void Start () {
		if(team == 1){
			myTeam = GameObject.FindGameObjectsWithTag ("team1");
//			enemyTeam = GameObject.FindGameObjectsWithTag ("team2");
		} else {
			myTeam = GameObject.FindGameObjectsWithTag ("team2");
//			enemyTeam = GameObject.FindGameObjectsWithTag ("team1");
		}
		flagPos = getFlagObject ().transform.position;
		/*
		rolesToAssigne.Add ("chatchFlag");
		rolesToAssigne.Add ("helpFlag");
		rolesToAssigne.Add ("attack");
		rolesToAssigne.Add ("attack");
		rolesToAssigne.Add ("attack");
*/
	}

	// Update is called once per frame
	void Update () {
		foreach (GameObject player in myTeam) {
			KillPlayer kill = player.GetComponent<KillPlayer>();
			if(!kill.dead) {
				
				bool isRespawnZone = inRespawnZone(player);
				MoveRobotAstar mover = player.GetComponent<MoveRobotAstar>();

				if(isRespawnZone && mover.hasFlag) {
					gameController.increaseScore(team);
					mover.hasFlag = false;
					player.transform.Find("flag").gameObject.SetActive(false);
					//flagMyTeam.SetActive(true);
					Instantiate(flagPrefabs, flagPos, Quaternion.identity);
				}

				if(!mover.hasRole){
					if (isRespawnZone) {
						if(!playerAssignedFlag) {
							catchFlagRole role = player.GetComponent<catchFlagRole>();
							role.flagTag = flagTag;
							role.basePos = baseMyTeam;
							role.enabled = true;
							playerAssignedFlag = true;
							mover.role = "catchFlag";
							//Debug.Log ("catcher settato");
						} else if(!playerAssignedHelp) {
							playerAssignedHelp = true;
							helpFlagRole role = player.GetComponent<helpFlagRole>();
							role.enabled = true;
							mover.role = "helpFlag";
						} else {
							attackRole role = player.GetComponent<attackRole>();
							role.dest = baseMyTeam.transform.position;
							role.enabled = true;
							mover.role = "attack";
						}
						mover.hasRole = true;
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
	/*
	private string pickRole() {

		foreach(string role in rolesToAssigne) {
			
		}
	}*/
}
