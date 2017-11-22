using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

	public float X, Y;
	public Material[] Colours;
	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material = Colours[Random.Range(0, Colours.Length)];
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float xValue = transform.localPosition.x + X / 5;
		float yValue = transform.localPosition.y + Y / 5;
		if (xValue > 10.9f)
		{
			xValue = -10.9f;
			GetComponent<Renderer>().material = Colours[Random.Range(0, Colours.Length)];
		}
		if (xValue < -10.9f)
		{
			xValue = 10.9f;
			GetComponent<Renderer>().material = Colours[Random.Range(0, Colours.Length)];
		}
		transform.localPosition = new Vector3(xValue, yValue, transform.localPosition.z);
	}
}
