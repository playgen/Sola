using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller for the background cubes
public class BackgroundController : MonoBehaviour {

	public float X, Y;
	public Material[] Colours;

	bool[] _transition;
	GameObject[] _others;
	float _original, _last, _fx, _fy, _xValue, _yValue;

	// Use this for initialization
	void Start () {
		// Randomly set its colour
		GetComponent<Renderer>().material = Colours[Random.Range(0, Colours.Length)];
		_others = GameObject.FindGameObjectsWithTag("Background");
		_transition = new bool[4];
		_transition[3] = true;
		_original = transform.localPosition.y;
		_xValue = transform.localPosition.x;
		_yValue = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Move by X / 5 every iteration
		_xValue += X / 5;


		// Before you've logged in move across the screen faster
		if (_transition[3])
		{
			_xValue += X;
			_yValue += Y;
		}
		// TODO resort alignment after log in

		// Once a block reaches the end of the screen move it to the other side and set it to another colour
		if (_xValue > 11f || _xValue < -11f)
		{
			_xValue = 11f * (-_xValue / Mathf.Abs(_xValue));
			GetComponent<Renderer>().material = Colours[Random.Range(0, Colours.Length)];
			// End transition 2
			_transition[2] = false;
		}

		// First Transition
		// Called when you go to the Home Page
		if (_transition[0])
		{
			_yValue -= 0.043f;
			if(transform.localPosition.y < -7.75f)
			{
				_yValue = 7.75f;
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 1.527f);
			}
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + 0.5f);
			if(transform.localEulerAngles.z > 180.0f || transform.localEulerAngles.z < 0.0f)
			{
				_yValue = _original;
				transform.localEulerAngles = Vector3.zero;
				_transition[0] = false;
			}
		}
		// Second Transition
		// Called when you leave the home page
		if (_transition[1])
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.1f);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 1.0f, transform.localEulerAngles.z);
			if (transform.localEulerAngles.y > 180.0f || transform.localEulerAngles.y < 0.0f)
			{
				transform.localEulerAngles = Vector3.zero;
				_transition[1] = false;
			}
		}
		
		// Either set the scroll speed with the third transition or the normal values
		if (_transition[2])
		{
			// Third Transition
			// Called after you log in
			_fx += X;
			_fy += Y;
			transform.localPosition = new Vector3(_fx, _fy, transform.localPosition.z);
		}
		else
		{
			// When doing the third transition you still need to keep track of the regular x and y values so that it can realign properly
			_fx = _xValue;
			_fy = _yValue;
			transform.localPosition = new Vector3(_xValue, _yValue, transform.localPosition.z);
		}
	}

	// Start a new transition only if there is not one currently happening
	public void New(int i)
	{
		if (_transition[3])
		{
			_transition[3] = false;
		}
		// Check that no transition is currently happening
		int any = Currently();
		bool current = (any == 0);
		foreach(GameObject g in _others)
		{
			if(g.GetComponent<BackgroundController>().Currently() != 0 && g.GetComponent<BackgroundController>().Currently() != i + 1)
			{
				current = false;
			}
		}
		if (current)
		{
			_transition[i] = true;
		}
	}

	// If there is a transition currently happening return which one
	public int Currently()
	{
		int clear = 0;
		if (_transition[0])
		{
			clear = 1;
		}

		if (_transition[1])
		{
			clear = 2;
		}
		return clear;
	}
}
