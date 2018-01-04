using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour {

	public GameObject[] Buttons;
	bool _left, _right;
	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
		// When you press a button go to the second page of the guide
		if (_right && transform.localPosition.x > -20.0f)
		{
			transform.localPosition = new Vector3(transform.localPosition.x - 0.5f, 0.0f, 0.0f);
		}
		// Return to the first page
		else if (_left && transform.localPosition.x < 0.0f)
		{
			transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, 0.0f, 0.0f);
		}
		// Button listener
		if (Buttons[0].GetComponent<ButtonController>().Pressed)
		{
			_right = true;
			_left = false;
			Buttons[0].GetComponent<ButtonController>().Pressed = false;
		}
		else if (Buttons[1].GetComponent<ButtonController>().Pressed)
		{
			_right = false;
			_left = true;
			Buttons[1].GetComponent<ButtonController>().Pressed = false;
		}
	}
}
