using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBGenerator : MonoBehaviour {

	public GameObject[] BallUpdates = new GameObject[25];
	public GameObject[] Dodgeballs = new GameObject[25];
	public Material[] Materials = new Material[7];
	public Sprite[] Sprites = new Sprite[4];
	public float[] UpdateNumbers;
	public GameObject Dodgeball;
	public bool Pulse, PulseFive, Server, InGame;
	public int Counter;
	
	// Use this for initialization
	void Start ()
	{
		Server = false;
		Pulse = false;
		PulseFive = false;
		Counter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//Create one dodgeball
		if (Counter < Dodgeballs.Length - 1)
		{
			if (Pulse)
			{
				GameObject dodge = (GameObject)Instantiate(Dodgeball);
				dodge.GetComponentInChildren<DodgeController>().BallNumber = Counter;
				Dodgeballs[Counter] = dodge;
				Counter++;
				Pulse = false;
			}
			//Create five dodgeball
			if (PulseFive)
			{
				for (int i = 0; i < 5; i++)
				{
					GameObject dodge = (GameObject)Instantiate(Dodgeball);
					dodge.GetComponentInChildren<DodgeController>().BallNumber = Counter;
					Dodgeballs[Counter] = dodge;
					Counter++;
					PulseFive = false;
				}
			}
		}
		//Update each dodgeball from the info sent by the server
		if (UpdateNumbers.Length > 0 && !Server && InGame)
		{
			for (int i = 0; i < UpdateNumbers.Length; i = i + 13)
			{
				if (Dodgeballs[(int)UpdateNumbers[i]] != null)
				{
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().transform.GetComponent<Renderer>().material = Materials[(int)UpdateNumbers[i + 1]];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<SpriteRenderer>().sprite = Sprites[(int)UpdateNumbers[i + 2]];
					Dodgeballs[(int)UpdateNumbers[i]].transform.localEulerAngles = new Vector3(0.0f, 0.0f, UpdateNumbers[i + 3]);
					Dodgeballs[(int)UpdateNumbers[i]].transform.localPosition = new Vector3(UpdateNumbers[i + 4], UpdateNumbers[i + 5], 0.0f);
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().transform.localScale = new Vector3(UpdateNumbers[i + 6], UpdateNumbers[i + 6], 0.1f);
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Min = UpdateNumbers[i + 7];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Max = UpdateNumbers[i + 8];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().State = (int)UpdateNumbers[i + 9];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().RotationSpeed = UpdateNumbers[i + 10];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Speed = (int)UpdateNumbers[i + 11];
					Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Special = (int)UpdateNumbers[i + 12];
					if ((int)UpdateNumbers[i + 9] != 5)
					{
						Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().transform.localPosition = Vector3.zero;
					}
				}
			}
		}
	}
}
