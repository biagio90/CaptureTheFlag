using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public int numPlayers;

	private float width = 80, hight = 30;
	private float left = Screen.width-100, top = 10;


	private int scoreTeam1 = 0;
	private int scoreTeam2 = 0;

	public int num_catcher = 0;
	public int num_helper = 0;
	public int num_attacker = 0;

	// Use this for initialization
	void Start () {
		GameObject team1 = GameObject.FindGameObjectWithTag ("team1");
		GameObject team2 = GameObject.FindGameObjectWithTag ("team2");

		for (int i=0; i<numPlayers-1; i++) {
			Instantiate(team1, team1.transform.position, team1.transform.rotation);
			Instantiate(team2, team2.transform.position, team2.transform.rotation);

		}
	}

	
	void OnGUI() {
		GUI.color = Color.red;
		GUI.Label(new Rect (left, top, width, hight), 
		          "Team1: "+getScore(1));
		GUI.Label(new Rect (left, top+hight, width, hight), 
		          "Team2: "+getScore(2));
		GUI.Label (new Rect (left, top + hight * 2, width, hight), 
		          "catcher: " + num_catcher);
		GUI.Label (new Rect (left, top + hight * 3, width, hight), 
		           "catcher: " + num_helper);
		GUI.Label (new Rect (left, top + hight * 4, width, hight), 
		           "catcher: " + num_attacker);

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
