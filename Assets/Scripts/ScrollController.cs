using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the scrolling text ("Connecting", "Press TAB to start", etc)
// Also attached to the home screen buttons ("Online", "Controls", etc)
public class ScrollController : MonoBehaviour {

	public float Direction, Distance;
	public bool Rotate;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Scroll across the screen in the desired direction
	void FixedUpdate ()
	{
		// For the home screen buttons
		if (Rotate)
		{
			transform.localEulerAngles = new Vector3(Direction, 0.0f, 90.0f);
			Direction = Direction + 0.3f;
			if(Direction >= 360.0f)
			{
				Direction = 0.0f;
			}
		}
		// For the Scrolling text
		else
		{
			transform.localPosition = new Vector3(transform.localPosition.x + Direction, transform.localPosition.y, transform.localPosition.z);

			// When it reaches the end of the screen put it back on the other side to create a loop
			if (transform.localPosition.x > Distance || transform.localPosition.x < -Distance)
			{
				transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			}
		}
	}
}
