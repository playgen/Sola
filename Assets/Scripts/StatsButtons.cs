using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsButtons : MonoBehaviour
{
	public GameObject[] All;
	public Sprite[] Sprites;
	public bool Pressed, Sprite;
	public int type;

	Controller _controller;
	bool _hovered;

	// Use this for initialization
	void Start()
	{
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
		Pressed = false;
		_hovered = false;
	}

	void FixedUpdate()
	{
		if(type == 6 && Pressed)
		{
			Pressed = false;
		}
		// When the button is pressed show the correct sprite
		if (Pressed)
		{
			transform.GetComponent<SpriteRenderer>().sprite = Sprites[2];
		}
		// Whilst the mouse is over the button display the correct Sprite/Material or it is forced on
		else if (_hovered)
		{
			transform.GetComponent<SpriteRenderer>().sprite = Sprites[1];
		}
		// Otherwise display the normal Sprite/Material
		else
		{
			transform.GetComponent<SpriteRenderer>().sprite = Sprites[0];
		}
		// Reset the hovered variable between each iteration
		_hovered = false;
		
	}

	// On mouse over set hovered true
	void OnMouseOver()
	{
		_hovered = true;
	}

	// Mouse listener. Handles the achievements, friends list etc pop upss
	void OnMouseDown()
	{
		int offset = 0;
		// Change between score leaderboards and time
		if (All[6].GetComponent<StatsButtons>().Pressed)
		{
			offset = 4;
		}
		// When you press the achievement button go to the next menu
		if(type == 0)
		{
				All[6].SetActive(true);
				All[7].SetActive(true);
				All[8].SetActive(true);
				All[1].SetActive(false);
				All[9].SetActive(false);
			Pressed = true;
		}
		// When you press the friends button either go to the friends list or the friends leaderboard
		else if (type == 1)
		{
			if(All[0].GetComponent<StatsButtons>().Pressed == false)
			{
				_controller.PopUps(0);
			}
			else
			{
				_controller.PopUps(3 + offset);
			}
		}
		// When you press the group button go to the groups list
		else if (type == 2)
		{
			_controller.PopUps(1);
		}
		// Top ranked leaderboard
		else if (type == 3)
		{
			_controller.PopUps(5 + offset);
		}
		// Near to user leaderboard
		else if (type == 4)
		{
			_controller.PopUps(6 + offset);
		}
		// Back button. Returns to previous page
		else if (type == 5)
		{
			All[1].SetActive(false);
			All[3].SetActive(false);
			All[4].SetActive(false);
			All[5].SetActive(false);
			All[6].SetActive(true);
			All[7].SetActive(true);
			All[8].SetActive(true);
		}
		// Select a leaderboard type
		else if (type == 6)
		{
			All[1].SetActive(true);
			All[3].SetActive(true);
			All[4].SetActive(true);
			All[5].SetActive(true);
			All[6].SetActive(false);
			All[7].SetActive(false);
			All[8].SetActive(false);
			Pressed = true;
		}
		// Back button. Returns to initial page
		else if (type == 7)
		{
			All[6].SetActive(false);
			All[7].SetActive(false);
			All[8].SetActive(false);
			All[9].SetActive(true);
			All[1].SetActive(true);
			All[0].GetComponent<StatsButtons>().Pressed = false;
		}
		// Display achievements list
		else if (type == 8)
		{
			_controller.PopUps(2);
		}
	}
}