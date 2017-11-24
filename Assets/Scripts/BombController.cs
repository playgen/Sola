using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour {

	public GameObject Explosion;
	public int Counter, ECounter;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Counter > 0)
		{
			Counter--;
		}
		else if (Counter == 0)
		{
			Explosion.SetActive(true);
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			Counter--;
		}
		else if(ECounter > 0)
		{
			ECounter--;
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
