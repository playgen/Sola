using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
	public Material[] Materials;
	public Sprite[] Sprites;
	public bool Pressed, Sprite;

	// Use this for initialization
	void Start()
	{
		Pressed = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{

	}
	void OnMouseOver()
	{
		if(Sprite)
		{
			transform.GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
		else
		{
			transform.GetComponent<Renderer>().material = Materials[1];
		}
	}

	void OnMouseExit()
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


	void OnMouseDown()
	{
		Pressed = true;
		if (Sprite)
		{
			transform.GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
		else
		{
			transform.GetComponent<Renderer>().material = Materials[0];
		}
	}
}
