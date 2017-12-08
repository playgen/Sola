using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayer : MonoBehaviour {

	public float Speed;
	public bool _grow;

	SaveButton[] _buttons;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!_grow)
		{
			transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
		}
		else if (_grow && transform.localScale.y < 1.0f)
		{
			transform.localScale = new Vector3(1.0f, transform.localScale.y + Speed / 100, 1.0f);
		}
	}

	public void Operational (bool x)
	{
		_grow = x;
	}
}
