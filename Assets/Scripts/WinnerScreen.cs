using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the end game screen. Assigns the players times to the podium game obhects and sets them active
public class WinnerScreen : MonoBehaviour {

	public GameObject[] podiums;
	bool _running;
	// Use this for initialization
	void Start () {
		_running = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Run(GameObject[] Players)
	{
		if(!_running)
		{
			ArrayList times = new ArrayList();
			for (int i = 0; i < Players.Length; i++)
			{
				times.Add(Players[i].GetComponent<PlayerController>().Time);
			}
			times.Sort();
			for (int j = 0; j < Players.Length; j++)
			{
				int offset = 4 - Players.Length;
				int position = times.IndexOf(Players[j].GetComponent<PlayerController>().Time);
				int score = (int)((float)times[position]);
				podiums[position + offset].SetActive(true);
				podiums[position + offset].GetComponent<PlaceController>().Player = Players[j];
				podiums[position + offset].GetComponent<PlaceController>().Score = score;
			}
		}
	}

	// Hide the podiums so the next game can happen
	public void Clear()
	{
		for (int i = 0; i < 4; i++)
		{
			podiums[i].SetActive(false);
		}
	}
}
