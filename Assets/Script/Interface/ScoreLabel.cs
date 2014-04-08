using UnityEngine;
using System.Collections;

public class ScoreLabel : MonoBehaviour {
	private GameController gameController;

	private float width = 80, hight = 30;
	private float left = Screen.width-100, top = 10;


	// Use this for initialization
	void Start () {
		gameController = GetComponent<GameController> ();
	}

	void OnGUI() {
		GUI.color = Color.red;
		GUI.Label(new Rect (left, top, width, hight), 
		          "Team1: "+gameController.getScore(1));
		GUI.Label(new Rect (left, top+hight, width, hight), 
		          "Team2: "+gameController.getScore(2));

	}
}
