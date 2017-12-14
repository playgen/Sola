using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles a player sending another player money
public class Request : MonoBehaviour {

	public PlayerController Player;

	Controller _controller;
	int counter;

	// Use this for initialization
	void Start () {
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
		counter = -1;
	}

	// The arrow you press to give a user money disappears after a set amount of time
	void FixedUpdate () {
		if (counter >= 0)
		{
			counter--;
			if(counter == 0)
			{
				gameObject.SetActive(false);
			}
		}
		else if (counter == -1)
		{
			counter = 200;
		}
	}
	
	// On click give 1000 coins to the other user and give visual feedback
	void OnMouseDown()
	{
		_controller.TransferResource(Player.ID, "coins", 1000);
		_controller.Gain.GetComponent<MoneyChange>().Change(true);
		Player.GaveMoney();
	}
}
