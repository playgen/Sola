using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A little pop up when you recieve or send money telling you how much you just spent/recieved
public class MoneyChange : MonoBehaviour {

	public Sprite[] Sprites;
	bool _inc;
	// Use this for initialization
	void Start ()
	{
		_inc = false;
	}
	
	// Grows then Shrinks
	void FixedUpdate ()
	{
		// Growing till it reaches 1 width
		if (_inc)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.04f, 1.0f, 1.0f);
			if (transform.localScale.x > 1.0f)
			{
				_inc = false;
			}
		}
		// Shrinks until it reaches 0
		else if (transform.localScale.x > 0)
		{
			transform.localScale = new Vector3(transform.localScale.x - 0.04f, 1.0f, 1.0f);
		}
		// If it shrunk too far (-0.01) sets it to 0
		else if (transform.localScale.x < 0)
		{
			transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		}
	}

	// Tells the object which sprite to use. 
	public void Change(bool gave)
	{
		transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		_inc = true;
		// gave 1000
		if(gave)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
		// recieved 1000
		else
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
	}
}
