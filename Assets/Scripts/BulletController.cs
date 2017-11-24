using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	public Transform From;
	public int Counter;
	// Use this for initialization
	void Start ()
	{
		GetComponent<Rigidbody>().velocity += transform.right * 5.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Counter == 0)
		{
			Destroy(transform.parent.gameObject);
		}
		Counter--;
	}
}
