using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public bool started = false;

	// MENU
	private float left2 = 350, top2 = 200;
	private float width2 = 250, hight2 = 30;
	private float offset = 40;

	private int select1 = 0;
	private int select2 = 0;

	//
	public int numPlayers;
	public int endScore = 10;
	private int winner = 0;

	private float width = 80, hight = 30;
	private float left = Screen.width-100, top = 10;


	private int scoreTeam1 = 0;
	private int scoreTeam2 = 0;

	public int num_catcher1  = 0;
	public int num_helper1   = 0;
	public int num_attacker1 = 0;

	public int num_catcher2  = 0;
	public int num_helper2   = 0;
	public int num_attacker2 = 0;

	public void startMatch() {
		GameObject team1 = GameObject.FindGameObjectWithTag ("team1");
		GameObject team2 = GameObject.FindGameObjectWithTag ("team2");
		
		for (int i=0; i<numPlayers-1; i++) {
			if(team1!=null) Instantiate(team1, team1.transform.position, team1.transform.rotation);
			if(team2!=null) Instantiate(team2, team2.transform.position, team2.transform.rotation);
			
		}

		started = true;
	}

	// Use this for initialization
	void Start () {

	}

	private void endOfTheMatch(){
		GameObject[] team1 = GameObject.FindGameObjectsWithTag ("team1");
		GameObject[] team2 = GameObject.FindGameObjectsWithTag ("team2");

		foreach (GameObject player in team1) {
			MoveRobotAstar mover = player.GetComponent<MoveRobotAstar>();
			mover.reset();
			MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
			foreach(MonoBehaviour script in scripts){
				script.enabled = false;
			}
		}

		foreach (GameObject player in team2) {
			MoveRobotAstar mover = player.GetComponent<MoveRobotAstar>();
			mover.reset();
			MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
			foreach(MonoBehaviour script in scripts){
				script.enabled = false;
			}
		}

	}
	
	void OnGUI() {
		if (started) {
			displayPoints();
			if (winner != 0) {
				if (GUI.Button (new Rect (left2-offset*2, top2+offset*4, width2, hight2), "Restart")) {
					Application.LoadLevel(Application.loadedLevel);
				}
			}
		} else {
			//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), image);
			string[] strategies = new string[4] {"Dummy", "Circle", "Half", "Neighbor"};

			GUI.TextField(new Rect(left2-offset*2, top2, 60, hight2), "Team1: ");
			select1 = GUI.Toolbar(new Rect(left2, top2, width2*1.5f, hight2), 
			                      select1, strategies);

			GUI.TextField(new Rect(left2-offset*2, top2+offset, 60, hight2), "Team2: ");
			select2 = GUI.Toolbar(new Rect(left2, top2+offset, width2*1.5f, hight2), 
			                      select2, strategies);

			if (GUI.Button (new Rect (left2, top2+offset*2, width2, hight2), "Start")) {
				Debug.Log("Start");
				chooseStrategy(select1, "team1");
				chooseStrategy(select2, "team2");
				startMatch();
			}
		}
	}

	private void chooseStrategy(int selection, string teamTag) {
		PlayerController player = GameObject.FindGameObjectWithTag (teamTag).GetComponent<PlayerController>();
		switch (selection) {
		case 0: player.strategy = PlayerController.Strategy.Dummy;
			break;
		case 1: player.strategy = PlayerController.Strategy.Circle;
			break;
		case 2: player.strategy = PlayerController.Strategy.Half;
			break;
		case 3: player.strategy = PlayerController.Strategy.Neighborhood;
			break;

		}
	}

	private void displayPoints() {
		if (winner != 0) {
			GUIStyle style = new GUIStyle();
			style.fontSize = 50;
			style.normal.textColor = Color.green;
			style.alignment = TextAnchor.MiddleCenter;
			float w = 200, h = 40;
			GUI.Label(new Rect (Screen.width/2-w/2, Screen.height/2-h/2+20, w, h), 
			          "END OF THE MATCH", style);
			if(winner == 1){
				GUI.Label(new Rect (Screen.width/2-w/2, Screen.height/2-h, w, h), 
				          "TEAM 1 WON!!", style);
			} else {
				GUI.Label(new Rect (Screen.width/2-w/2, Screen.height/2-h, w, h), 
				          "TEAM 2 WON!!", style);
			}
			
			endOfTheMatch();
		}
		
		GUI.color = Color.red;
		GUI.Label(new Rect (left, top, width, hight), 
		          "Team1: "+getScore(1));
		GUI.Label(new Rect (left, top+hight, width, hight), 
		          "Team2: "+getScore(2));
		GUI.Label (new Rect (left, top + hight * 2, width, hight), 
		           "catcher1: " + num_catcher1);
		GUI.Label (new Rect (left, top + hight * 3, width, hight), 
		           "helper1: " + num_helper1);
		GUI.Label (new Rect (left, top + hight * 4, width, hight), 
		           "attacker1: " + num_attacker1);
		GUI.Label (new Rect (left, top + hight * 5, width, hight), 
		           "catcher2: " + num_catcher2);
		GUI.Label (new Rect (left, top + hight * 6, width, hight), 
		           "helper2: " + num_helper2);
		GUI.Label (new Rect (left, top + hight * 7, width, hight), 
		           "attacker2: " + num_attacker2);

	}

	// Update is called once per frame
	void Update () {
		if (scoreTeam1 >= endScore) {
			winner = 1;
			return;
		}
		if (scoreTeam2 >= endScore) {
			winner = 2;
			return;
		}
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
