using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for the shield itm. Turned on until the counter runs out and blocks attacks whilst on
public class ShieldController : MonoBehaviour {

	public GameObject Player;
	public int Counter;

	int _count;
	// Use this for initialization
	void Start () {
		_count = Counter;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (_count == 0)
		{
			_count = Counter;
			gameObject.SetActive(false);
		}
		else
		{
			_count--;
		}
	}
}
