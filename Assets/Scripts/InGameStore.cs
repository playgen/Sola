using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the in game store
public class InGameStore : MonoBehaviour {

	public Sprite[] Sprites;
	public GameObject[] Items;
	public int[] _option;

	Controller _controller;

	// Use this for initialization
	void Start () {
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
	}
	
	void FixedUpdate () {
		for (int i = 0; i < Items.Length; i++)
		{
			Upgrade upgrade = Items[i].GetComponent<Upgrade>();
			// If an item in the store is selected and the user has enough money buy it
			if (upgrade.Pressed && _controller.Requested["coins"] >= upgrade.SoloValue)
			{

				upgrade.Pressed = false;
				_controller.Selected = _option[i];

				// Update the players wealth
				_controller.Requested.Remove("coins");
				_controller.AddResource("coins", -3000);
				_controller.GetResource("coins");
			}
		}
	}

	// Random choice of items in the in game store so every game is different
	public void Change(int i)
	{
		int range = 0;
		// Single player has 3 items (Shield, Blink, Tiny)
		if (i == 0)
		{
			range = 3;
		}
		// Co-op has 4 items (Shield, Blink, Tiny, Health)
		else if (i == 1)
		{
			range = 4;
		}
		// Versus has 6 items (Shield, Blink, Tiny, Spinners, Gun, Bomb)
		else
		{
			range = 6;
		}
		// Pick a random option
		_option[0] = Random.Range(0, range);
		_option[1] = Random.Range(0, range);
		// The 2 options should be different
		while (_option[0] == _option[1])
		{
			_option[1] = Random.Range(0, range);
		}
		// In versus skip the Health item
		if (_option[1] > 2 && i == 2)
		{
			_option[1]++;
		}
		// Set the items to the correct sprites
		Items[0].GetComponent<SpriteRenderer>().sprite = Sprites[_option[0]];
		Items[1].GetComponent<SpriteRenderer>().sprite = Sprites[_option[1]];
		// Scale sprites
		for(int k = 0; k < Items.Length; k++)
		{
			if (_option[k] == 3)
			{
				Items[k].transform.localScale = new Vector3(1.0f, 1.0f, 0.4f);
			}
			else if (_option[k] == 5)
			{
				Items[k].transform.localScale = new Vector3(0.15f, 0.2f, 0.4f);

			}
			else if (_option[k] == 6)
			{
				Items[k].transform.localScale = new Vector3(0.3f, 0.3f, 0.4f);
			}
			else
			{
				Items[k].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			}
		}
	}
}
