using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Update is called once per frame
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
		_hovered = false;
	}
	void OnMouseOver()
	{
		_hovered = true;
	}


	void OnMouseDown()
	{
		Pressed = true;
	}
}
