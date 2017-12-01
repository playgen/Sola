using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceController : MonoBehaviour {

	public GameObject Position, Player;
	public GameObject[] Numbers, Stripes;
	public Sprite[] Sprites;
	public int Score;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Score > 99999)
		{
			Score = 99999;
		}
		string scoreString = Score.ToString();
		while(scoreString.Length < 5)
		{
			scoreString = "0" + scoreString;
		}
		for(int i = 0; i < 5; i++)
		{
			Numbers[i].GetComponent<SpriteRenderer>().sprite = Sprites[int.Parse(scoreString.Substring(i, 1))];
		}
		Player.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Player.transform.localEulerAngles.z + 2.0f);
		foreach (GameObject s in Stripes)
		{
			s.GetComponent<Renderer>().material = Player.GetComponent<PlayerController>().Stripes[0].GetComponent<Renderer>().material;
		}
	}
}
