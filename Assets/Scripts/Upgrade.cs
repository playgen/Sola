using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {

	public Sprite[] Sprites;
	public GameObject[] Previous;

	public bool Hover, Pressed, Red, Selected;
	public float Value, SoloValue;
	public string Key;
	
	// Use this for initialization
	void Start () {
		Value = SoloValue;
		if (Previous.Length > 0)
		{
			if (!Previous[Previous.Length - 1].GetComponent<Upgrade>().Red)
			{
				Value = SoloValue + Previous[Previous.Length - 1].GetComponent<Upgrade>().Value;
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Selected)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[3];
		}
		else if(Hover)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
		else if (Red)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[2];
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
		if(Previous.Length > 0)
		{
			if(!Previous[Previous.Length - 1].GetComponent<Upgrade>().Red)
			{
				Value = SoloValue + Previous[Previous.Length - 1].GetComponent<Upgrade>().Value;
			}
			else
			{
				Value = SoloValue;
			}
		}
	}
	
	void OnMouseOver()
	{
		Hover = true;
	}
	void OnMouseDown()
	{
		Pressed = true;
	}

	void OnMouseExit()
	{
		Hover = false;
	}
}
