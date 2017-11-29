using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour {
	
	public GameObject[] Numbers;
	public GameObject[] Positions;
	public Sprite[] ChestSprite, NumberSprite;
	public GameObject Chest, Controller, Store, Single, Online, Back;

	GameObject[] _values;
	Upgrade[] _buttons;
	float _wealth, _minus;
	bool _hovered, _done;
	string _wealthString, _minusString;
	int _justHovered, _newValue, _individual;

	// Use this for initialization
	void Start ()
	{
		_buttons = GetComponentsInChildren<Upgrade>();
		_values = new GameObject[Positions.Length];
		_wealth = 0;
		_justHovered = 0;
		_minus = 0.0f;
		_wealthString = _wealth.ToString();
		while (_wealthString.Length < 8)
		{
			_wealthString = "0" + _wealthString;
		}
		//It will cut off at 8 digits. Make it so that it either has a coin cap or makes it clear
		for (int i = 0; i < 13; i++)
		{
			_values[i] = GameObject.Instantiate(Numbers[0]);
			_values[i].transform.position = Positions[i].transform.position;
			_values[i].transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		CalculateDifferences();
		Dictionary();
		int quantity = (int)Controller.GetComponent<Controller>().Requested["selected"];
		foreach (Upgrade up in _buttons)
		{
			Clicked(up);
			up.Selected = false;
			if ((up.Key == "bomb" && quantity == 1) || (up.Key == "blink" && quantity == 2) || (up.Key == "gun" && quantity == 3) || (up.Key == "shield" && quantity == 4) || (up.Key == "razor" && quantity == 5))
			{
				up.Selected = true;
			}
		}
	}

	void Dictionary()
	{
		if (Controller.GetComponent<Controller>().Requested.ContainsKey("coins"))
		{
			_wealth = Controller.GetComponent<Controller>().Requested["coins"];
		}
		foreach (Upgrade up in _buttons)
		{
			if (Controller.GetComponent<Controller>().Requested.ContainsKey(up.Key))
			{
				float value = Controller.GetComponent<Controller>().Requested[up.Key];
				if (value != 0)
				{
					int position = up.Previous.Length;
					if (position < value)
					{
						up.Red = true;
					}
				}
			}
		}

	}

	void Clicked(Upgrade up)
	{
		if (up.Pressed == true && _wealth >= _minus)
		{
			Debug.Log(_wealth + " : " + _minus);
			Controller.GetComponent<Controller>().Requested.Remove("coins");
			Controller.GetComponent<Controller>().AddResource("coins", -_minus);
			Controller.GetComponent<Controller>().GetResource("coins");

			int amount = 1;
			if (up.Previous.Length != 0)
			{
				foreach(GameObject u in up.Previous)
				{
					if(!u.GetComponent<Upgrade>().Red)
					{
						amount++;
						u.GetComponent<Upgrade>().Red = true;
					}
				}
			}
			if (_minus > 0)
			{
				Controller.GetComponent<Controller>().AddResource(up.Key, amount);
				Controller.GetComponent<Controller>().GetResource(up.Key);
				up.Red = true;
			}
		}
		if(up.Red == true && up.Pressed == true)
		{
			if (up.Key == "bomb")
			{
				Controller.GetComponent<Controller>().SetResource("selected", 1);
				Controller.GetComponent<Controller>().Requested.Remove("selected");
				Controller.GetComponent<Controller>().Requested.Add("selected", 1);
			}
			else if (up.Key == "blink")
			{
				Controller.GetComponent<Controller>().SetResource("selected", 2);
				Controller.GetComponent<Controller>().Requested.Remove("selected");
				Controller.GetComponent<Controller>().Requested.Add("selected", 2);
			}
			else if (up.Key == "gun")
			{
				Controller.GetComponent<Controller>().SetResource("selected", 3);
				Controller.GetComponent<Controller>().Requested.Remove("selected");
				Controller.GetComponent<Controller>().Requested.Add("selected", 3);
			}
			else if (up.Key == "shield")
			{
				Controller.GetComponent<Controller>().SetResource("selected", 4);
				Controller.GetComponent<Controller>().Requested.Remove("selected");
				Controller.GetComponent<Controller>().Requested.Add("selected", 4);
			}
			else if (up.Key == "razor")
			{
				Controller.GetComponent<Controller>().SetResource("selected", 5);
				Controller.GetComponent<Controller>().Requested.Remove("selected");
				Controller.GetComponent<Controller>().Requested.Add("selected", 5);
			}
		}
		if (up.Pressed == true)
		{
			up.Pressed = false;
		}
	}

	void CalculateDifferences()
	{
		if (_hovered)
		{
			Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[1];
		}
		else
		{
			Chest.GetComponent<SpriteRenderer>().sprite = ChestSprite[0];
		}
		_newValue = 0;
		if (Int32.Parse(_wealthString) != (_wealth - _minus))
		{
			_newValue = 1;
		}
		_wealthString = (_wealth - _minus).ToString();
		while (_wealthString.Length < 8)
		{
			_wealthString = "0" + _wealthString;
		}
		int index = _wealthString.IndexOf('-');
		if (index != -1)
		{
			_wealthString = "-" + _wealthString.Substring(0, index) + _wealthString.Substring(index + 1, _wealthString.Length - (index + 1));
		}
		//It will cut off at 8 digits. Make it so that it either has a coin cap or makes it clear
		FlipNumbers(0, 8, _wealthString, _newValue);

		_hovered = false;
		foreach (Upgrade b in _buttons)
		{
			if (b.Hover && !b.Red)
			{
				_hovered = true;
				_minus = b.Value;
				if (_justHovered == 0)
				{
					_justHovered = 1;
				}
			}
		}
		if (!_hovered)
		{
			_minus = 0.0f;
			if (_justHovered != 0)
			{
				_justHovered = 3;
			}
		}

		_minusString = _minus.ToString();
		while (_minusString.Length < 5)
		{
			_minusString = "0" + _minusString;
		}
		FlipNumbers(8, 13, _minusString, (_justHovered % 2));

		if (_justHovered == 1)
		{
			_justHovered = 2;
		}
		if (_justHovered == 3)
		{
			_justHovered = 0;
		}
	}

	void FlipNumbers(int start, int end, string comparison, int pass)
	{
		//It will cut off at 8 digits. Make it so that it either has a coin cap or makes it clear
		for (int i = start; i < end; i++)
		{
			try
			{
				_individual = Int32.Parse(comparison.Substring(i - start, 1));
			}
			catch (FormatException e)
			{
				_individual = 10;
			}

			if (pass == 1 || Mathf.Abs(_values[i].transform.localEulerAngles.x) > 0.4f)
			{
				_values[i].transform.localEulerAngles = new Vector3(_values[i].transform.localEulerAngles.x + 10.0f, 0.0f, 0.0f);
			}
			if (_values[i].transform.localEulerAngles.x >= 90.0f && _values[i].transform.localEulerAngles.x <= 120.0f)
			{
				Destroy(_values[i]);
				_values[i] = GameObject.Instantiate(Numbers[_individual]);
				_values[i].transform.position = Positions[i].transform.position;
				_values[i].transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
				_values[i].transform.parent = transform;
				_values[i].transform.localScale = new Vector3(0.5f, 0.5f, _values[i].transform.localScale.z);

				if (_minus != 0)
				{
					for (int j = 0; j < 8; j++)
					{
						SpriteRenderer[] sprites = _values[i].transform.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer s in sprites)
						{
							if (Int32.Parse(_wealthString) > 0)
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
