using UnityEngine;
using System.Collections;

public class RandomAttack : MonoBehaviour {
	private float prob_go_enemybase;
	public float range_action;
	public GameObject enemybase;
	public GameObject homebase;

	private Vector3 destination;
	private float homeflagdistance;
	private bool team1;

	// Use this for initialization
	void Start () {
		/*
		bool = (this.tag == 'team1'); 
		if(team1)
			homeflagdistance = distance(flagTeam1.position,baseTeam1.position);
		else
       */
	}
	
	// Update is called once per frame
	void Update () {

		if (Vector3.Distance (transform.position, enemybase.transform.position) > Vector3.Distance (transform.position, homebase.transform.position))
						prob_go_enemybase = 40;
		else 
			prob_go_enemybase = 80;

		destination = getrandomposition (prob_go_enemybase, range_action);
		MoveRobotAstar script = GetComponent<MoveRobotAstar>();
		script.newDestination(destination);
}


	Vector3	getrandomposition (float prob_go_enemybase,float range_action){
		 Vector3 direction = Vector3.Normalize (transform.forward);
		 Vector3 enemybasedir = enemybase.transform.position - transform.position;
		 Quaternion quat;
		 float angle = Random.Range(0,360);
		 float distance = Random.Range(0,range_action);
		if(Mathf.FloorToInt(Random.Range(0,100)) < prob_go_enemybase){
			angle = Vector3.Angle(direction,enemybasedir);
			quat = Quaternion.AngleAxis(angle,direction);
		}
		else{
		 angle = Random.Range(0,360);
		 quat = Quaternion.AngleAxis(angle,direction);
		}
		direction = quat * direction;
		direction = direction * distance;
		return (direction + transform.position);
		 		 
	}
}
