using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for a players health bar
public class HeartController : MonoBehaviour {

	public GameObject[] Hearts = new GameObject[5];
	public Sprite[] Sprites = new Sprite[2];
	public GameObject Player;
	public int Health, Place;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//If the player Object associated with this health bar is destroyed destroy it as well
		if(Player == null)
		{
			Destroy(gameObject);
		}
		//Display the correct amount of health
		for (int i = 0; i < Hearts.Length; i++)
		{
			if (Health > i)
			{
				Hearts[i].GetComponent<SpriteRenderer>().sprite = Sprites[0];
			}
			else
			{
				Hearts[i].GetComponent<SpriteRenderer>().sprite = Sprites[1];
			}
		}
	}
}
