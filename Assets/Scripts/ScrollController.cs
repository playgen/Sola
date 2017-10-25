using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour {

	public float Direction;

	// Use this for initialization
	void Start ()
	{
		if(Direction < 0.0f)
		{
			transform.localPosition = new Vector3(13.0f, transform.position.y, 0.0f);
		}
		else
		{
			transform.localPosition = new Vector3(-13.0f, transform.position.y, 0.0f);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		transform.localPosition = new Vector3(transform.position.x + Direction, transform.position.y, 0.0f);
		if(transform.localPosition.x > 13.0f || transform.localPosition.x < -13.0f)
		{
			transform.localPosition = new Vector3(-transform.position.x, transform.position.y, 0.0f);
		}
	}
}
