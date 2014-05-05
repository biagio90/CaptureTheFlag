using UnityEngine;
using System.Collections;

public class GameControllerRL : MonoBehaviour {
	public enum States {flagOnGround, flagCarried, madePoint};
	private States team1State = States.flagOnGround;
	private States team2State = States.flagOnGround;

	private GameObject[] team1;
	private GameObject[] team2;

	public int score1 = 0;
	public int score2 = 0;

	// Use this for initialization
	void Start () {
		team1 = GameObject.FindGameObjectsWithTag ("team1");
		team2 = GameObject.FindGameObjectsWithTag ("team2");

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void team1Scoring() {
		score1 ++;
	}

	public void team2Scoring() {
		score2 ++;
		foreach(GameObject player in team1) {
			PlayerRL rl = player.GetComponent<PlayerRL>();
			rl.updateState(constantRL.enemyMakeScore);
		}
	}

}
