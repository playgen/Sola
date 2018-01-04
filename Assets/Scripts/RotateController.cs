using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just a rotate function
public class RotateController : MonoBehaviour
{

	public float rotation;
	// Use this for initialization
	void Start()
	{

	}

	void FixedUpdate()
	{
		// Add rotation
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + rotation);
	}
}
