using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the end game screen. Assigns the players times to the podium game objects and sets them active
public class WinnerScreen : MonoBehaviour {

	public GameObject[] Podiums;
	public int Offset;
	// Use this for initialization
	void Start () {
		Offset = 0;
		// set all podiums active
		for (int j = 0; j < Podiums.Length; j++)
		{
			Podiums[j].SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	// At the end of the game show the player rankings/scores
	public void Run(GameObject[] Players)
	{
		// Add all the player objects to an arraylist
		ArrayList rankings = new ArrayList();
		for (int i = 0; i < Players.Length; i++)
		{
			rankings.Add(Players[i]);
		}
		for (int j = 0; j < Players.Length; j++)
		{
			// Calculate the highest remaining score and assign that player to the variable best
			GameObject best = (GameObject)rankings[0];
			for (int k = 0; k < rankings.Count; k++)
			{
				if (((GameObject)rankings[k]).GetComponent<PlayerController>().Score > best.GetComponent<PlayerController>().Score)
				{
					best = Players[k];
				}
			}

			// 1st place gets assigned to the 1st podium, 2nd place to the 2nd podium and so on
			PlaceController placeCon = Podiums[j + Offset].GetComponent<PlaceController>();
			// Set the podiums score and colour to match the associated player
			placeCon.Score = best.GetComponent<PlayerController>().Score;
			placeCon.Player.GetComponent<Renderer>().material = best.GetComponent<Renderer>().material;
			// Show the podium on the screen
			placeCon.EndScreen();
			// Remove the player from the array list in order to calculate the next highest score
			rankings.Remove(best);
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
