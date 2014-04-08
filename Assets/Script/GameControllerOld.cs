using UnityEngine;
using System.Collections;

public class GameControllerOld : MonoBehaviour {
	private GameObject[] team1;
	private GameObject[] team2;

	public GameObject flagTeam1;
	public GameObject flagTeam2;

	public GameObject baseTeam1;
	public GameObject baseTeam2;

	private bool cameBack = true;

	// Use this for initialization
	void Start () {
		team1 = GameObject.FindGameObjectsWithTag ("team1");
		team2 = GameObject.FindGameObjectsWithTag ("team2");

		foreach (GameObject player in team1) {
			MoveRobotAstar script = player.GetComponent<MoveRobotAstar>();
			script.newDestination( flagTeam1.transform.position );
		}
		
		foreach (GameObject player in team2) {
			MoveRobotAstar script = player.GetComponent<MoveRobotAstar>();
			script.newDestination( flagTeam2.transform.position );
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!cameBack)
		foreach (GameObject player in team1) {
			MoveRobotAstar script = player.GetComponent<MoveRobotAstar>();
			if (script.hasFlag) {
				script.newDestination( baseTeam1.transform.position );
				cameBack = true;
			}
		}
		/*
		foreach (GameObject player in team2) {
			MoveRobotAstar script = player.GetComponent<MoveRobotAstar>();
			if (script.hasFlag) {
				script.newDestination( baseTeam2.transform.position );
			}
		}*/
	}
}
