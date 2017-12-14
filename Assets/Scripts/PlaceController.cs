using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IDisplays the players score throughout the game and in the end screen
public class PlaceController : MonoBehaviour {

	public GameObject Position, Dash, Player;
	public GameObject[] Numbers;

	public Sprite[] Sprites, DashSprites;
	public int Score, Offset;

	// True for the end screen but not the scores shown throughout the game
	public bool EndPodium, RandomColour, IgnDash;

	float _original;

	// Use this for initialization
	void Start () {
		// Original height
		_original = transform.localPosition.y;
		// If its a script for the end screen hide the gameobject until the game ends
		if(EndPodium)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, -30.0f, transform.localPosition.z);
		}
	}
	
	void FixedUpdate ()
	{
		Run();

		// End game transition. Drops the final scores from the top of the screen until the reach there initial position
		if (transform.localPosition.y > _original)
		{
			// Slows down the closer it is to its end position
			float drop = (transform.localPosition.y - _original) / 10;
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - drop, transform.localPosition.z);
			Player.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Player.transform.localEulerAngles.z + 2.0f);
		}
	}

	// Move to the top of the screen so that it can move down onto the screen
	public void EndScreen ()
	{
		if(transform.localPosition.y <= _original)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, 4.9f, transform.localPosition.z);
		}
	}

	// Hide the Podium so that it is hidden for the next game
	public void Reposition()
	{
		transform.localPosition = new Vector3(transform.localPosition.x, -30.0f, transform.localPosition.z);
	}

	// Reset the display
	public void Reset()
	{
		Run();
		// Reset the score menu
		Score = 0;
		Offset = 0;
		RandomColour = false;
	}

	// Updates the display
	void Run()
	{
		// Score Cap
		if (Score > 9999999)
		{
			Score = 9999999;
		}
		string scoreString = Score.ToString();
		// Display score
		for (int i = 0; i < scoreString.Length; i++)
		{
			// Set the value 1 number at a time. And only display the required numbers. (i.e 4000 won't be shown as 00004000)
			SpriteRenderer renderer = Numbers[Numbers.Length - scoreString.Length + i].GetComponent<SpriteRenderer>();
			// Give a random offset, when we're using random colours we dont need the dash
			if (RandomColour)
			{
				Offset = ((scoreString.Length - i) % 4) * 10;
			}
			// If you want the dash add it
			if (!IgnDash)
			{
				Dash.GetComponent<SpriteRenderer>().sprite = DashSprites[Offset / 10];
			}
			// Get the matching letters and - to the player so they all have the same colour.
			// Sprites(0-9 Blue 10-19 Green 20-21 Purple 30-31 Yellow)
			renderer.sprite = Sprites[Offset + int.Parse(scoreString.Substring(i, 1))];
			renderer.enabled = true;
		}
		// Hide the extra numbers 0030000 will be shown as 30000
		for (int i = 0; i < Numbers.Length - scoreString.Length; i++)
		{
			Numbers[i].GetComponent<SpriteRenderer>().enabled = false;
		}
	}
}
