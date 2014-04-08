using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
	public GameObject explosion;
	public GameObject playerExplosion;
	
	void OnTriggerEnter(Collider other) 
	{
	//	Instantiate(explosion, transform.position, transform.rotation);
		if (other.tag == "team2")
		{
			//Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			Destroy(other.gameObject);
		}
		Destroy(gameObject);
	}
}
