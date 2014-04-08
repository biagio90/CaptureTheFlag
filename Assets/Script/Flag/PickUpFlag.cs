using UnityEngine;
using System.Collections;

public class PickUpFlag : MonoBehaviour {
	public string teamTag;
	//public Material hasFlagMaterial;

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == teamTag)
		{
			Destroy(gameObject);
			//gameObject.SetActive(false);
			MoveRobotAstar player = other.GetComponent<MoveRobotAstar>();
			player.hasFlag = true;
			//other.renderer.material.color = Color.red;
			other.gameObject.transform.Find("flag").gameObject.SetActive(true);
		}
	}
}
