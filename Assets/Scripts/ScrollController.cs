using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour {

	public float Direction, Distance;
	public bool Rotate;

	// Use this for initialization
	void Start ()
	{
		if (Rotate)
		{
			transform.localEulerAngles = new Vector3(Direction, 0.0f, 90.0f);
		}
		else
		{
			if (Direction < 0.0f)
			{
				transform.localPosition = new Vector3(Distance, transform.localPosition.y, transform.localPosition.z);
			}
			else
			{
				transform.localPosition = new Vector3(-Distance, transform.localPosition.y, transform.localPosition.z);
			}
		}
	}
	
	// Scroll across the screen in the desired direction
	void FixedUpdate ()
	{
		if (Rotate)
		{
			transform.localEulerAngles = new Vector3(Direction, 0.0f, 90.0f);
			Direction = Direction + 0.3f;
			if(Direction >= 360.0f)
			{
				Direction = 0.0f;
			}
		}
		else
		{
			transform.localPosition = new Vector3(transform.localPosition.x + Direction, transform.localPosition.y, transform.localPosition.z);
			if (transform.localPosition.x > Distance || transform.localPosition.x < -Distance)
			{
				transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			}
		}
	}
}
