using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour {
	public GameController gameController;

	public int team;

	public float respownArea = 5;

	private GameObject[] myTeam;
	private GameObject[] enemyTeam;
	
	public GameObject flagMyTeam;
	public GameObject flagEnemyTeam;
	
	public GameObject baseMyTeam;
	public GameObject baseEnemyTeam;

	public bool playerAssignedFlag = false;
	public bool playerAssignedHelp = false;

	//private ArrayList rolesToAssigne = new ArrayList();

	// Use this for initialization
	void Start () {
		if(team == 1){
			myTeam = GameObject.FindGameObjectsWithTag ("team1");
			enemyTeam = GameObject.FindGameObjectsWithTag ("team2");
		} else {
			myTeam = GameObject.FindGameObjectsWithTag ("team2");
			enemyTeam = GameObject.FindGameObjectsWithTag ("team1");
		}
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
			bool isRespawnZone = inRespawnZone(player);
			MoveRobotAstar mover = player.GetComponent<MoveRobotAstar>();

			if(isRespawnZone && mover.hasFlag) {
				gameController.increaseScore(team);
				mover.hasFlag = false;
				player.transform.Find("flag").gameObject.SetActive(false);
				flagMyTeam.SetActive(true);
			}

			if(!mover.hasRole){
				if (isRespawnZone) {
					if(!playerAssignedFlag) {
						catchFlagRole role = player.GetComponent<catchFlagRole>();
						role.flag = flagMyTeam;
						role.basePos = baseMyTeam;
						role.enabled = true;
						playerAssignedFlag = true;
						mover.role = "catchFlag";
						Debug.Log ("catcher settato");
					} else if(!playerAssignedHelp) {
						playerAssignedHelp = true;
						helpFlagRole role = player.GetComponent<helpFlagRole>();
						role.enabled = true;
						mover.role = "helpFlag";
					} else {
						player.GetComponent<attackRole>().enabled = true;
						mover.role = "attack";
					}
					mover.hasRole = true;
				}
			}
		}
	}

	private bool inRespawnZone(GameObject player) {
		return Vector3.Distance (player.transform.position, 
		                         baseMyTeam.transform.position) < respownArea;
	}
	/*
	private string pickRole() {

		foreach(string role in rolesToAssigne) {
			
		}
	}*/
}
