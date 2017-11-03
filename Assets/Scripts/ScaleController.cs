﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Scale everything to the correct size
	void FixedUpdate () {
		float ratio = ((float) Screen.width) / ((float)Screen.height);
		float initial = 1.778417f;
		transform.localScale = new Vector3(ratio / initial, ratio / initial, 1);
	}
}