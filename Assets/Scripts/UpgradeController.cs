using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles button interactions within the shop
public class UpgradeController : MonoBehaviour {
	
	public GameObject[] Numbers;
	public GameObject[] Positions;
	public Sprite[] ChestSprite, NumberSprite;
	public GameObject Chest, Controller, Store, Single, Online, Back;
	public bool InGameStore;

	Controller _controller;
	GameObject[] _values;
	Upgrade[] _buttons;
	float _wealth, _price, _oldWealth, _oldPrice;
	bool _hovered, _done;
	string _wealthString, _priceString;
	int _justHovered, _newValue, _individual;

	// Use this for initialization
	void Start ()
	{
		_controller = Controller.GetComponent<Controller>();
		// Get all the buttons in the shop
		_buttons = GetComponentsInChildren<Upgrade>();
		_values = new GameObject[Positions.Length];
		_oldWealth = 0;
		_wealth = 0;
		_justHovered = 0;
		_price = 0.0f;

		// Initially set players wealth and item price to 0. In case the actual values weren't pulled correctly
		for (int i = 0; i < Positions.Length; i++)
		{
			_values[i] = GameObject.Instantiate(Numbers[0]);
			_values[i].transform.position = Positions[i].transform.position;
			_values[i].transform.parent = transform;
			if (InGameStore)
			{
				_values[i].transform.localScale = new Vector3(0.2f, 0.2f, _values[i].transform.localScale.z);
			}
		}
	}

	void FixedUpdate ()
	{
		CalculateDifferences();
		Dictionary();
		// If its the real store and not the in game on handle button presses in this script
		// Otherwise it's handle in the InGameStore script
		if(!InGameStore)
		{
			foreach (Upgrade up in _buttons)
			{
				Clicked(up);
			}
		}
	}

	// Get all the values from the dictionary whose values are recieved from the database
	// saves having to continuously ask the database which is slow and fills up the queue
	void Dictionary()
	{
		// Get the amount of coins
		if (_controller.Requested.ContainsKey("coins"))
		{
			_wealth = _controller.Requested["coins"];
		}
		// Get the value of each of the buttons
		foreach (Upgrade up in _buttons)
		{
			if (_controller.Requested.ContainsKey(up.Key))
			{
				float value = _controller.Requested[up.Key];
				if (value != 0)
				{
					int position = up.Previous.Length;
					if (position < value)
					{
						up.Bought = true;
					}
				}
			}
		}

	}

	// When a button in the store is pressed if you have enough money to buy the item
	// update the players wealth and buy the item
	void Clicked(Upgrade up)
	{
		if (up.Pressed == true && _wealth >= _price)
		{
			// Update wealth
			_controller.Requested.Remove("coins");
			_controller.AddResource("coins", -_price);
			_controller.GetResource("coins");

			// If it is a multi tiered item then buy the previous versions as well
			int amount = 1;
			if (up.Previous.Length != 0)
			{
				foreach(GameObject u in up.Previous)
				{
					if(!u.GetComponent<Upgrade>().Bought)
					{
						amount++;
						u.GetComponent<Upgrade>().Bought = true;
					}
				}
			}
			// If you've already bought the item don't buy it again
			if (_price > 0)
			{
				// Update the database and the dictionary
				_controller.AddResource(up.Key, amount);
				_controller.GetResource(up.Key);
				up.Bought = true;
			}
		}
		// Cancel button press
		if (up.Pressed == true)
		{
			up.Pressed = false;
		}
	}

	void CalculateDifferences()
	{
		// Initially set hovered to false
		_hovered = false;
		_price = 0.0f;
		// If you're hovering over an item that hasn't been bought yet get the items value
		// and  set hovered to true
		foreach (Upgrade b in _buttons)
		{
			if (b.Hover && !b.Bought)
			{
				_hovered = true;
				_price = b.Value;
			}
		}

		// If its the actual store and not the in game store update the chest sprite when
		// you are hovering over an item
		if (!InGameStore)
		{
			if (_hovered)
			{
				Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[1];
			}
			else
			{
				Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[0];
			}

		}

		// Convert users wealth minus the price of an item he is hovering over into a string
		_wealthString = (_wealth - _price).ToString();
		// Add zeros to the string until the length is 7. Because the wealth cap is 9999999
		while (_wealthString.Length < 7)
		{
			_wealthString = "0" + _wealthString;
		}
		// For minus numbers place the negative sign in front of the entire number (including the 0s)
		int index = _wealthString.IndexOf('-');
		if (index != -1)
		{
			_wealthString = "-" + _wealthString.Substring(0, index) + _wealthString.Substring(index + 1, _wealthString.Length - (index + 1));
		}
		// Convert the items price to a string
		_priceString = _price.ToString();
		while (_priceString.Length < 7)
		{
			_priceString = "0" + _priceString;
		}
	
		// If the numbers on screen do not match the actual numbers update them
		bool flip = false;
		if ((_wealth - _price) != _oldWealth || _price != _oldPrice)
		{
			flip = true;
		}


		// 7 Digits for your wealth and 7 for the price
		FlipNumbers(0, 7, _wealthString, flip);
		FlipNumbers(7, Positions.Length, _priceString, flip);

	}

	// Numbers animation. Whenever a number is updated it rotates it and replaces it with the updated value
	void FlipNumbers(int start, int end, string comparison, bool flip)
	{

		//It will cut off at 7 digit
		for (int i = start; i < end; i++)
		{
			// get an individual digit
			try
			{
				_individual = Int32.Parse(comparison.Substring(i - start, 1));
			}
			// For minus numbers the negative sign will fail and be set to 10 which is the position of the - character in the array
			catch (FormatException e)
			{
				_individual = 10;
			}

			// If the object has been told to rotating or is mid roation keep rotating
			if (flip || Mathf.Abs(_values[i].transform.localEulerAngles.x) > 0.4f)
			{
				_values[i].transform.localEulerAngles = new Vector3(_values[i].transform.localEulerAngles.x + 10.0f, 0.0f, 0.0f);
			}
			// When the object is a 90 degress replace it with the updated number
			if (_values[i].transform.localEulerAngles.x >= 90.0f && _values[i].transform.localEulerAngles.x <= 120.0f)
			{
				// Store the new values of wealth and price
				if (start == 0)
				{
					_oldWealth = Int32.Parse(comparison);
				}
				if (start == 7)
				{
					_oldPrice = Int32.Parse(comparison);
				}
				// Destroy old game object
				Destroy(_values[i]);
				// Create new object with the updated number
				_values[i] = GameObject.Instantiate(Numbers[_individual]);
				_values[i].transform.position = Positions[i].transform.position;
				_values[i].transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
				_values[i].transform.parent = transform;
				// Scale for the correct store
				if (InGameStore)
				{
					_values[i].transform.localScale = new Vector3(0.2f, 0.2f, _values[i].transform.localScale.z);
				}
				else
				{
					_values[i].transform.localScale = new Vector3(0.5f, 0.5f, _values[i].transform.localScale.z);
				}

				// Set the numbers colour. Red if too expensive, green if you can buy it and yellow if nothing is selected
				if (_price != 0)
				{
					for (int j = 0; j < 8; j++)
					{
						SpriteRenderer[] sprites = _values[i].transform.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer s in sprites)
						{
							if (Int32.Parse(_wealthString) >= 0)
							{
								s.sprite = NumberSprite[1];
							}
							else
							{
								s.sprite = NumberSprite[2];
							}
						}
					}
				}
			}
		}
	}
	
}
