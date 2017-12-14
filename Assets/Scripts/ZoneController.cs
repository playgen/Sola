using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Controller for a points zone. Staying within this space gives extra points
public class ZoneController : NetworkBehaviour {

	public float ShrinkSpeed;

	Material _material;
	bool _running;
	float _speed, _size;
	int _counter;

	// Use this for initialization
	void Start () {
		// Zones material
		_material = GetComponent<Renderer>().material;
		_running = false;
		_counter = 0;
	}
	
	void FixedUpdate () {
		// If the zone is not active and it is either a single player game or the games host
		if (!_running && !(isClient && !isServer))
		{
			_running = true;
			// Get a random size and deterioration speed
			_size = Random.Range(1.0f, 4.0f);
			_speed = Random.Range(0.3f, 0.8f);
			// Place the zone in a random place on the map
			transform.localPosition = new Vector3(Random.Range(-8.645f, 8.645f), Random.Range(-4.775f, 4.775f), transform.localPosition.z);
			// Set the initial colour to green. The zone slowly fades to red before diappearing
			_material.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
			// Update the clients
			if(isServer)
			{
				RpcZones(_size, _speed, transform.localPosition);
			}
		}
		// First
		if (transform.localScale.x < _size && _material.color.g >= 1)
		{
			// Scale the zone to its correct size
			transform.localScale = new Vector3(transform.localScale.x + (ShrinkSpeed / 4), transform.localScale.y + (ShrinkSpeed / 4), transform.localScale.z);
			// Once it's at the corret size move the colldier to inline with the players
			if (transform.localScale.x >= _size)
			{
				GetComponent<SphereCollider>().center = new Vector3(0, 0, -1200);
				transform.localScale = new Vector3(_size, _size, transform.localScale.z);
			}
		}
		// Second
		else if(_material.color.g >= 1 && _material.color.r < 1)
		{
			// Fade from green to yellow
			_material.color = new Color(_material.color.r + (_speed / 100), 1.0f, 0.0f, 1.0f);
		}
		// Third
		else if (_material.color.g > 0 && _material.color.r >= 1)
		{
			// Fade from yellow to red
			_material.color = new Color(1.0f, _material.color.g - (_speed / 100), 0.0f, 1.0f);
		}
		// Fourth
		else if (transform.localScale.x > 0)
		{
			// Shrink the zone to 0 and move the collider out of the way
			GetComponent<SphereCollider>().center = new Vector3(0, 0, 0);
			transform.localScale = new Vector3(transform.localScale.x - (ShrinkSpeed / 4), transform.localScale.y - (ShrinkSpeed / 4), transform.localScale.z);
		}
		// Fifth
		else if(transform.localScale.x < 0)
		{
			// Start a counter and completely hide the zone
			transform.localScale = new Vector3(0.0f, 0.0f, transform.localScale.z);
			_counter = 100;
		}
		// Wait until the counter ends then repeat
		else
		{
			if(_counter > 0)
			{
				_counter--;
			}
			else
			{
				_running = false;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			AddPoints(other.GetComponent<PlayerController>());
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			AddPoints(other.GetComponent<PlayerController>());
		}
	}

	// Set the players colour back to normal
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			foreach (GameObject s in other.GetComponent<PlayerController>().Stripes)
			{
				s.GetComponent<Renderer>().material = other.GetComponent<PlayerController>().Materials[4];
			}
		}
	}

	// Add points if a player is in the zone
	void AddPoints(PlayerController pCont)
	{
		// increment players score for being in the bonus area
		if (isServer)
		{
			pCont.RpcScores(1);
		}
		else if (!isClient)
		{
			pCont.ScoreDisplay.GetComponent<PlaceController>().Score += 1;
		}
		// Change the players colour to indicate they are recieving bonus points
		foreach (GameObject s in pCont.Stripes)
		{
			s.GetComponent<Renderer>().material.color = new Color(_material.color.r, _material.color.g, 0.2f, _material.color.a);
		}
	}

	//Update zones
	[ClientRpc]
	public void RpcZones(float size, float speed, Vector3 position)
	{
		_running = true;
		_size = size;
		_speed = speed;
		transform.localPosition = position;
		_material.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	}
}
