using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for the buttons within the store menu
public class Upgrade : MonoBehaviour {

	public Sprite[] Sprites;
	public GameObject[] Previous;

	public bool Hover, Pressed, Bought, IGS;
	public float Value, SoloValue;
	public string Key;
	
	// Use this for initialization
	void Start () {
		// For items with multiple levels add the cost of earlier levels that haven't been bought to its price
		Value = SoloValue;
		if (Previous.Length > 0)
		{
			if (!Previous[Previous.Length - 1].GetComponent<Upgrade>().Bought)
			{
				Value = SoloValue + Previous[Previous.Length - 1].GetComponent<Upgrade>().Value;
			}
		}
	}
	
	void FixedUpdate ()
	{
		if (!IGS)
		{
			// Here the correct sprite is chosen for each button
			// If you are hovering over a button set the sprite
			if (Hover)
			{
				GetComponent<SpriteRenderer>().sprite = Sprites[1];
			}
			// If you have bought the item
			else if (Bought)
			{
				GetComponent<SpriteRenderer>().sprite = Sprites[2];
			}
			// If none of the above use the default sprite
			else
			{
				GetComponent<SpriteRenderer>().sprite = Sprites[0];
			}
		}

		// Update the value of items with multiple levels
		if(Previous.Length > 0)
		{
			if(!Previous[Previous.Length - 1].GetComponent<Upgrade>().Bought)
			{
				Value = SoloValue + Previous[Previous.Length - 1].GetComponent<Upgrade>().Value;
			}
			else
			{
				Value = SoloValue;
			}
		}
	}
	
	// On mouse actions
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
