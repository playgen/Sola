using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour {

	public float rotation;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + rotation);
	}
}
