using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameStore : MonoBehaviour {

	public Sprite[] Sprites;
	public GameObject[] Items;

	Controller _controller;
	int _optionOne, _optionTwo;

	// Use this for initialization
	void Start () {
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Items[0].GetComponent<Upgrade>().Pressed)
		{
			if (_controller.Requested["coins"] >= 3000)
			{
				Items[0].GetComponent<Upgrade>().Pressed = false;
				_controller.Requested.Remove("coins");
				_controller.AddResource("coins", -3000);
				_controller.GetResource("coins");
				_controller.Selected = _optionOne;
			}
		}
		if (Items[1].GetComponent<Upgrade>().Pressed)
		{
			Items[1].GetComponent<Upgrade>().Pressed = false;
			if (_controller.Requested["coins"] >= 3000)
			{
				_controller.Requested.Remove("coins");
				_controller.AddResource("coins", -3000);
				_controller.GetResource("coins");
				_controller.Selected = _optionTwo;
			}
		}
	}

	public void Change(int i)
	{
		if (i == 0)
		{
			_optionOne = Random.Range(0, 3);
			_optionTwo = Random.Range(0, 3);
			while (_optionOne == _optionTwo)
			{
				_optionTwo = Random.Range(0, 3);
			}
		}
		else if (i == 1)
		{
			_optionOne = Random.Range(0, 4);
			_optionTwo = Random.Range(0, 4);
			while (_optionOne == _optionTwo)
			{
				_optionTwo = Random.Range(0, 4);
			}
		}
		else
		{
			_optionOne = Random.Range(0, 6);
			if (_optionOne > 2)
			{
				_optionOne++;
			}
			_optionTwo = Random.Range(0, 6);
			if (_optionTwo > 2)
			{
				_optionTwo++;
			}
			while (_optionOne == _optionTwo)
			{
				_optionTwo = Random.Range(0, 6);
				if (_optionTwo > 2)
				{
					_optionTwo++;
				}
			}
		}
		Items[0].GetComponent<SpriteRenderer>().sprite = Sprites[_optionOne];
		Items[1].GetComponent<SpriteRenderer>().sprite = Sprites[_optionTwo];
	}
}
