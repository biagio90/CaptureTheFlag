using UnityEngine;
using System.Collections;

public class PlayerRL : MonoBehaviour {
	// RL data
	private int state = constantRL.flag1Ground_flag2Ground;
	private int action = constantRL.GET_FLAG;
	private int stateOfAction = constantRL.flag1Ground_flag2Ground;

	public bool hasFlag = false;
	public bool dead = false;
	public int team;

	// INFO
	public GameObject playerExplosion;
	public GameObject flagPrefabs;
	public GameObject myBase;
	public GameControllerRL gameController;

	private GameObject[] myTeam;
	//private GameObject[] enemyTeam;

	// Use this for initialization
	void Start () {
		if(team == 1){
			myTeam = GameObject.FindGameObjectsWithTag ("team1");
			//enemyTeam = GameObject.FindGameObjectsWithTag ("team2");
		} else {
			myTeam = GameObject.FindGameObjectsWithTag ("team2");
			//enemyTeam = GameObject.FindGameObjectsWithTag ("team1");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (hasFlag) checkFlagToBase();

		switch (action) {
		case constantRL.DEFENSE:
			break;
		case constantRL.GET_ENEMY_FLAG:
			break;
		case constantRL.GET_FLAG:
			break;
		case constantRL.KILL_ENEMY_FLAG_CARRIER:
			break;
		case constantRL.RESTORE_FLAG:
			break;
		case constantRL.RETURN_TO_BASE:
			break;
		case constantRL.SUPPORT_FLAG_CARRIER:
			break;
		case constantRL.WAIT_AT_ENEMY_BASE:
			break;

		}
	}

	public void catchTheFLag() {
		hasFlag = true;
		gameObject.transform.Find("flag").gameObject.SetActive(true);

		// update state
		updateState(constantRL.tookFlag);
		foreach(GameObject player in myTeam) {
			PlayerRL rl = player.GetComponent<PlayerRL>();
			rl.updateState(constantRL.teammateTookFlag);
		}
		/*
		foreach(GameObject player in enemyTeam) {
			PlayerRL rl = player.GetComponent<PlayerRL>();
			rl.updateState(constantRL.enemysTooksEnemysFlag);
		}*/
	}

	public void catchEnemysFLag() {
		// update state
		updateState(constantRL.tookEnemysFlag);
		foreach(GameObject player in myTeam) {
			PlayerRL rl = player.GetComponent<PlayerRL>();
			rl.updateState(constantRL.teammateTookEnemysFlag);
		}
		/*
		foreach(GameObject player in enemyTeam) {
			PlayerRL rl = player.GetComponent<PlayerRL>();
			rl.updateState(constantRL.enemysTooksOurFlag);
		}*/
	}

	private void checkFlagToBase() {
		if (Vector3.Distance (transform.position, 
		                      myBase.transform.position) < 3.0f) {
			// leave flag
			gameObject.transform.Find("flag").gameObject.SetActive(false);
			hasFlag = false;

			// update score
			gameController.team1Scoring();

			// update state
			updateState(constantRL.makeScore);
			foreach(GameObject player in myTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.teammateMakeScore);
			}
			/*
			foreach(GameObject player in enemyTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.enemyMakeScore);
			}*/
		}
	}

	public void updateState(int eventHappened) {
		state = constantRL.selectNextState [state, eventHappened];
		terminateAction (state, eventHappened);
	}

	private void terminateAction(int newState, int eventHappened) {
		int r = calculateReward (action, stateOfAction, newState, eventHappened);
		updateQ (action, stateOfAction, newState, eventHappened, r);

		int prob = Random.Range (0, 100);
		if (prob < constantRL.epsilon) {
			action = Random.Range(0, constantRL.num_actions);
		} else {
			action = getArgmaxAction(newState);
		}
	}

	private int calculateReward(int a, int p, int s, int e) {
		return constantRL.rewards [a, e];
	}

	private void updateQ(int a, int p, int s, int e, int r) {
		float delta = r + constantRL.gamma * getMaxAction (s) - constantRL.Q [p, a];
		constantRL.Q [p, a] = constantRL.Q [p, a] + constantRL.alpha * delta;
	}

	private float getMaxAction (int s) {
		float max = 0;
		for (int i=0; i<constantRL.num_actions; i++) {
			if(constantRL.Q[s, i] > max) {
				max = constantRL.Q[s, i];
			}
		}
		return max;
	}
	
	private int getArgmaxAction (int s) {
		float max = 0;
		int index = 0;
		for (int i=0; i<constantRL.num_actions; i++) {
			if(constantRL.Q[s, i] > max) {
				max = constantRL.Q[s, i];
				index = i;
			}
		}
		return index;
	}

	public void killedEnemy(bool enemyHadFlag) {
		if (enemyHadFlag) {
			updateState(constantRL.killEnemyCarringFlag);
			foreach(GameObject player in myTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.teammatekillEnemyCarringFlag);
			}
			/*
			foreach(GameObject player in enemyTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.enemyKillTeammateCarringFlag);
			}*/
		} else {

		}
	}
	
	public void killPlayer() {
		Instantiate(playerExplosion, transform.position, transform.rotation);

		if (hasFlag) {
			// drop flag
			Instantiate(flagPrefabs, transform.position, Quaternion.identity);
			transform.Find("flag").gameObject.SetActive(false);

			// update state
			//updateState(constantRL.tookFlag);
			foreach(GameObject player in myTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.enemyKillTeammateCarringFlag);
			}
			/*
			foreach(GameObject player in enemyTeam) {
				PlayerRL rl = player.GetComponent<PlayerRL>();
				rl.updateState(constantRL.teammatekillEnemyCarringFlag);
			}*/
		}
		hasFlag = false;

		dead = true;
	}

}
