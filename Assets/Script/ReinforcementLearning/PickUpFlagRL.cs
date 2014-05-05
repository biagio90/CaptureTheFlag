using UnityEngine;
using System.Collections;

public class PickUpFlagRL : MonoBehaviour {
	public string teamTag;
	public Vector3 flagPos;
	
	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == teamTag)
		{
			Destroy(gameObject);
			PlayerRL player = other.GetComponent<PlayerRL>();
			player.catchTheFLag();
			//other.gameObject.transform.Find("flag").gameObject.SetActive(true);
		} else if (other.tag != "bullet"){
			transform.position = flagPos;
			PlayerRL player = other.GetComponent<PlayerRL>();
			player.catchEnemysFLag();
		}
	}
}
