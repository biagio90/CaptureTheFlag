using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	private int scoreTeam1 = 0;
	private int scoreTeam2 = 0;

	// Use this for initialization
	void Start () {
	
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
		return 0;
	}
}
