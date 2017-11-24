using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
				int position = times.IndexOf(Players[j].GetComponent<PlayerController>().Time);
				string score = times[position].ToString();
				int x = int.Parse(score.ToLower());
				Debug.Log(x.GetType());
				podiums[position].SetActive(true);
			}
		}
	}
}
