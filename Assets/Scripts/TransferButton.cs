using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransferButton : MonoBehaviour {

	public Sprite[] Sprites;

	Controller _controller;
	PlayerController _player;
	int _requestCounter;
	bool _hovered, _pressed;

	// Use this for initialization
	void Start () {
		_requestCounter = 0;
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_player = _controller.Player;
		if(_pressed)
		{
			_pressed = false;
			_player.Request();
		}
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
	}


	// On mouse over set hovered true
	void OnMouseOver()
	{
		_hovered = true;
	}

	// On click set pressed true
	void OnMouseDown()
	{
		if(_requestCounter == 0)
		{
			_requestCounter = 200;
			_pressed = true;
		}
	}
	
}
