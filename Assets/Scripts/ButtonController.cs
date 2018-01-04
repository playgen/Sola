using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// Controller for buttons. changes the sprite/material when hovered over and turns boolean
// Pressed on when clicked

public class ButtonController : NetworkBehaviour
{
	public Material[] Materials;
	public Sprite[] Sprites;
	public bool Pressed, Sprite, On, Network;

	bool _hovered;

	// Use this for initialization
	void Start()
	{
		Pressed = false;
		_hovered = false;
	}
	
	void FixedUpdate()
	{
		// Whilst the mouse is over the button or the button is On display the correct Sprite/Material
		if ((_hovered && !Network) || On)
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
		// Otherwise display the normal Sprite/Material
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

		// If the button is also shown to clients update them
		if(Network && isServer)
		{
			RpcOn(On);
		}
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


	//Update buttons on client
	[ClientRpc]
	public void RpcOn(bool on)
	{
		On = on;
	}
}
