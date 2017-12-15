using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for the shield item. Turned on until the counter runs out and blocks attacks whilst on
public class ShieldController : MonoBehaviour {

	public GameObject Player;
	public int Counter, Upgraded;

	int _count;
	// Use this for initialization
	void Start () {
		_count = Counter;
	}
	
	// Stay active until the counter runs out
	void FixedUpdate () {
		if(Upgraded > 0)
		{
			Upgraded--;
		}
		else if (_count == 0)
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
