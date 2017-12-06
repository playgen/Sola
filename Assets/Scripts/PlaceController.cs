using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// In the end game screen this updates depending on the assigned players score, also used as the timer in game
public class PlaceController : MonoBehaviour {

	public GameObject Position, Dash, Player;
	public GameObject[] Numbers;

	public Sprite[] Sprites, DashSprites;
	public int Score, Offset;
	public bool EndPodium;

	float _original;

	// Use this for initialization
	void Start () {
		_original = transform.localPosition.y;
		if(EndPodium)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, -30.0f, transform.localPosition.z);
		}
	}
	
	void FixedUpdate ()
	{
		// Score Cap
		if (Score > 9999999)
		{
			Score = 9999999;
		}
		string scoreString = Score.ToString();
		// Display score
		for(int i = 0; i < scoreString.Length; i++)
		{
			SpriteRenderer renderer = Numbers[Numbers.Length - scoreString.Length + i].GetComponent<SpriteRenderer>();
			renderer.enabled = true;
			renderer.sprite = Sprites[Offset + int.Parse(scoreString.Substring(i, 1))];
			Dash.GetComponent<SpriteRenderer>().sprite = DashSprites[Offset / 10];
		}
		for (int i = 0; i < Numbers.Length - scoreString.Length; i++)
		{
			Numbers[i].GetComponent<SpriteRenderer>().enabled = false;
		}

		if (transform.localPosition.y > _original)
		{
			float drop = (transform.localPosition.y - _original) / 10;
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - drop, transform.localPosition.z);
		}
	}

	public void EndScreen ()
	{
		if(transform.localPosition.y <= _original)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, 4.9f, transform.localPosition.z);
		}
	}

	public void Reposition()
	{
		transform.localPosition = new Vector3(transform.localPosition.x, -30.0f, transform.localPosition.z);
	}
}
