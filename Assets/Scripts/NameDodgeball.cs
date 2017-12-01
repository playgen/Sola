using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For the dodgeball attached to the title of the home page
public class NameDodgeball : MonoBehaviour {

	float _movement;
	// Use this for initialization
	void Start () {
		_movement = -0.2f;
	}
	
	// Move around the title
	void FixedUpdate () {
		transform.localPosition = new Vector3(0.0f, transform.localPosition.y +_movement, transform.localPosition.z);
		if(transform.localPosition.y < -12.0)
		{
			transform.localPosition = new Vector3(0.0f, transform.localPosition.y, 5.0f);
			_movement = -_movement;
		}
		if (transform.localPosition.y > 0.0f)
		{
			transform.localPosition = new Vector3(0.0f, transform.localPosition.y, -5.0f);
			_movement = -_movement;
		}
	}
}
