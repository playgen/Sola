using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// Button that when pressed requests a coin transfer from the other players
public class TransferButton : MonoBehaviour {

	public Sprite[] Sprites;

	Controller _controller;
	PlayerController _player;
	int _requestCounter;
	bool _hovered;

	// Use this for initialization
	void Start () {
		_requestCounter = 0;
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
	}
	
	void FixedUpdate () {
		// _player is the local player
		_player = _controller.Player;
		// Change the sprite depending on how recently the button was pressed
		if(_requestCounter > 0)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[2];
			_requestCounter--;
		}
		else if (_hovered)
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
		_hovered = false;
	}


	// On mouse over set hovered true
	void OnMouseOver()
	{
		_hovered = true;
	}

	// On click request coins as long as the countdown has ended
	void OnMouseDown()
	{
		if(_requestCounter == 0)
		{
			_requestCounter = 200;
			_player.Request();
		}
	}
	
}
