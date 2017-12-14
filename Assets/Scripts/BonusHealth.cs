using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the Health item. Exists whilst the counter is not 0
public class BonusHealth : MonoBehaviour
{
	public GameObject Player;
	public int Counter;
	// Use this for initialization
	void Start()
	{

	}

	void FixedUpdate()
	{
		// Decrement counters
		if (Counter > 0)
		{
			Counter--;
		}
		else if (Counter == 0)
		{
			Destroy(gameObject);
		}
	}
}
