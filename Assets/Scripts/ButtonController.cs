using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for buttons. changes the sprite/material when hovered over and turns boolean
// Pressed on when clicked

public class ButtonController : MonoBehaviour
{
	public Material[] Materials;
	public Sprite[] Sprites;
	public bool Pressed, Sprite;

	bool _hovered;

	// Use this for initialization
	void Start()
	{
		Pressed = false;
		_hovered = false;
	}
	
	void FixedUpdate()
	{
		if (_hovered)
		{
			if (Sprite)
			{
				transform.GetComponent<SpriteRenderer>().sprite = Sprites[1];
			}
			else
			{
				transform.GetComponent<Renderer>().material = Materials[1];
			}
		}
		else
		{
			if (Sprite)
			{
				transform.GetComponent<SpriteRenderer>().sprite = Sprites[0];
			}
			else
			{
				transform.GetComponent<Renderer>().material = Materials[0];
			}
		}
		// Reset the hovered variable between each iteration
		_hovered = false;
	}

	// On mouse over set hovered true
	void OnMouseOver()
	{
		_hovered = true;
	}

	// On click set pressed true
	void OnMouseDown()
	{
		Pressed = true;
	}
}
