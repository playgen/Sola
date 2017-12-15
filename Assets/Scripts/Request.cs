using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles a player sending another player money
public class Request : MonoBehaviour
{

	public PlayerController Player;

	Controller _controller;
	int counter;

	// Use this for initialization
	void Start()
	{
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
		counter = -1;
	}

	// The arrow you press to give a user money disappears after a set amount of time
	void FixedUpdate()
	{
		if (counter >= 0)
		{
			counter--;
			if (counter == 0)
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
		if (_controller.Requested.ContainsKey("coins"))
		{
			if (_controller.Requested["coins"] >= 1000)
			{
				// Inform the other player they just got more money
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach (GameObject p in players)
				{
					if (p.GetComponent<PlayerController>().isLocalPlayer)
					{
						p.GetComponent<PlayerController>().GaveMoney(Player.gameObject);
					}
				}
				_controller.TransferResource(Player.ID, "coins", 1000);
				// Update dictionary with new wealth
				_controller.Requested.Remove("coins");
				_controller.GetResource("coins");
				_controller.Gain.GetComponent<MoneyChange>().Change(true);
			}
		}
	}
}
