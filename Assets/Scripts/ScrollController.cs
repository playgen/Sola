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
			transform.localPosition = new Vector3(13.0f, transform.localPosition.y, 0.0f);
		}
		else
		{
			transform.localPosition = new Vector3(-13.0f, transform.localPosition.y, 0.0f);
		}
	}
	
	// Scroll across the screen in the desired direction
	void FixedUpdate ()
	{
		transform.localPosition = new Vector3(transform.localPosition.x + Direction, transform.localPosition.y, 0.0f);
		if(transform.localPosition.x > 13.0f || transform.localPosition.x < -13.0f)
		{
			transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, 0.0f);
		}
	}
}
