using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the end game screen. Assigns the players times to the podium game obhects and sets them active
public class WinnerScreen : MonoBehaviour {

	public GameObject[] Podiums;
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
			ArrayList rankings = new ArrayList();
			for (int i = 0; i < Players.Length; i++)
			{
				rankings.Add(Players[i]);
			}
			for (int j = 0; j < Players.Length; j++)
			{
				GameObject best = (GameObject) rankings[0];
				for (int k = 0; k < rankings.Count; k++)
				{
					if (((GameObject) rankings[k]).GetComponent<PlayerController>().Score > best.GetComponent<PlayerController>().Score)
					{
						best = Players[k];
					}
				}
				PlaceController placeCon = Podiums[j].GetComponent<PlaceController>();
				placeCon.Score = best.GetComponent<PlayerController>().Score;
				placeCon.Player.GetComponent<Renderer>().material = best.GetComponent<Renderer>().material;
				placeCon.EndScreen();
				rankings.Remove(best);
			}
		}
	}

	// Hide the podiums so the next game can happen
	public void Clear()
	{
		for (int j = 0; j < Podiums.Length; j++)
		{
			PlaceController placeCon = Podiums[j].GetComponent<PlaceController>();
			placeCon.Reposition();
		}
	}
}
