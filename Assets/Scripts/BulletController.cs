using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for a bullet which is part of the gun ability
public class BulletController : MonoBehaviour {

	public Transform From;
	public int Counter;
	// Use this for initialization
	void Start ()
	{
		// Add velocity
		GetComponent<Rigidbody>().velocity += transform.right * 5.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// After the countdown it should destroy itself
		if(Counter == 0)
		{
			Destroy(transform.parent.gameObject);
		}
		Counter--;
	}
}
