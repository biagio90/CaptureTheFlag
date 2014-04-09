using UnityEngine;
using System.Collections;

public class PickUpFlag : MonoBehaviour {
	public string teamTag;
	public Vector3 flagPos;

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == teamTag)
		{
			Destroy(gameObject);
			PlayerController player = other.GetComponent<PlayerController>();
			player.hasFlag = true;
			other.gameObject.transform.Find("flag").gameObject.SetActive(true);
		} else if (other.tag != "bullet"){
			transform.position = flagPos;
		}
	}
}
