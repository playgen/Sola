using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the bomb item
public class BombController : MonoBehaviour {

	public GameObject Explosion;
	// Counter until the explosion and then till the sprite disables
	public int Counter, ECounter;
	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
		// Decrement counters
		if(Counter > 0)
		{
			Counter--;
		}
		else if (ECounter > 0)
		{
			ECounter--;
		}
		else if (Counter == 0)
		{
			// After the first counter explode and stop moving
			Explosion.SetActive(true);
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			Counter--;
		}
		else
		{
			// Destroy after explosion
			Destroy(gameObject);
		}
	}
}
