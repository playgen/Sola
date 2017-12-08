using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour {

	public Sprite[] Sprites;
	public bool Hovered;
	// Use this for initialization
	void Start () {
		Hovered = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	void OnMouseOver()
	{
		GetComponent<SpriteRenderer>().sprite = Sprites[0];
		Hovered = true;
	}
	void OnMouseDown()
	{
	}

	void OnMouseExit()
	{
		GetComponent<SpriteRenderer>().sprite = Sprites[1];
		Hovered = false;
	}
}
