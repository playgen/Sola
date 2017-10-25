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
	public bool Pulse, Server;

	int _counter;
	
	// Use this for initialization
	void Start ()
	{
		Server = false;
		Pulse = false;
		_counter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Pulse)
		{
			GameObject dodge = (GameObject)Instantiate(Dodgeball);
			dodge.GetComponentInChildren<DodgeController>().BallNumber = _counter;
			Dodgeballs[_counter] = dodge;
			_counter++;
			Pulse = false;
		}
		if (UpdateNumbers.Length > 0 && !Server)
		{
			for(int i = 0; i < UpdateNumbers.Length; i = i + 10)
			{
				Dodgeballs[(int) UpdateNumbers[i]].GetComponentInChildren<DodgeController>().transform.GetComponent<Renderer>().material = Materials[(int) UpdateNumbers[i+1]];
				Dodgeballs[(int) UpdateNumbers[i]].GetComponentInChildren<SpriteRenderer>().sprite = Sprites[(int)UpdateNumbers[i + 2]];
				Dodgeballs[(int)UpdateNumbers[i]].transform.localEulerAngles = new Vector3(0.0f, 0.0f, UpdateNumbers[i + 3]);
				Dodgeballs[(int)UpdateNumbers[i]].transform.localPosition = new Vector3(UpdateNumbers[i + 4], UpdateNumbers[i + 5], 0.0f);
				Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().transform.localScale = new Vector3(UpdateNumbers[i + 6], UpdateNumbers[i + 6], 0.1f);
				Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Min = UpdateNumbers[i + 7];
				Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().Max = UpdateNumbers[i + 8];
				Dodgeballs[(int)UpdateNumbers[i]].GetComponentInChildren<DodgeController>().State = (int) UpdateNumbers[i + 9];
			}
		}
	}
}
