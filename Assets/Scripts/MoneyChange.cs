using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyChange : MonoBehaviour {

	public Sprite[] Sprites;
	bool _inc;
	// Use this for initialization
	void Start ()
	{
		_inc = false;
	}
	
	void FixedUpdate ()
	{
		if (_inc)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.04f, 1.0f, 1.0f);
			if (transform.localScale.x > 1.0f)
			{
				_inc = false;
			}
		}
		else if (transform.localScale.x > 0)
		{
			transform.localScale = new Vector3(transform.localScale.x - 0.04f, 1.0f, 1.0f);
		}
		else if (transform.localScale.x < 0)
		{
			transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		}
	}

	public void Change(bool gain)
	{
		transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		_inc = true;
		if(gain)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
		if (gain)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
	}
}
