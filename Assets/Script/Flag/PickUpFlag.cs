using UnityEngine;
using System.Collections;

public class PickUpFlag : MonoBehaviour {
	public string teamTag = "team1";

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == teamTag)
		{
			Destroy(gameObject);
		}
	}
}
