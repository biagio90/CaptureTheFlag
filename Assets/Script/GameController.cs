using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public int numPlayers;

	private int scoreTeam1 = 0;
	private int scoreTeam2 = 0;


	// Use this for initialization
	void Start () {
		GameObject team1 = GameObject.FindGameObjectWithTag ("team1");
		GameObject team2 = GameObject.FindGameObjectWithTag ("team2");

		for (int i=0; i<numPlayers-1; i++) {
			Instantiate(team1, team1.transform.position, team1.transform.rotation);
			Instantiate(team2, team2.transform.position, team2.transform.rotation);

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void increaseScore(int team) {
		if(team == 1) scoreTeam1++;
		else 		  scoreTeam2++;
	}

	public int getScore(int team){
		if(team == 1) return scoreTeam1;
		else 		  return scoreTeam2;
	}
}
