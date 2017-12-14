using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is attached to the camera. It scales the camera dependant on the Aspect ratio
public class ScaleController : MonoBehaviour {

	float _ratio, _initial;
	// Use this for initialization
	void Start () {
		// The ratio the camera is initialised too
		_initial = 1.778417f;
	}
	
	// Scale the camera to fit the aspect ratio
	void FixedUpdate () {
		_ratio = ((float) Screen.width) / ((float)Screen.height);
		GetComponent<Camera>().orthographicSize = 5 / (_ratio / _initial);
	}
}
