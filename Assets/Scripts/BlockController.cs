using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Controller for a block in the game. These are zones that the players cannot pass through
// Players can however blink over them
public class BlockController : NetworkBehaviour {

	public float activated;

	SpriteRenderer _sprite;
	float _speed, _size;
	int _counter, _running;

	// Use this for initialization
	void Start()
	{
		// Zones material
		_sprite = GetComponent<SpriteRenderer>();
		// Initial counter so it doesn't immediately spawn
		_counter = Random.Range(100, 300);
		_speed = 0.5f;
		_running = -1;
	}
	
	void FixedUpdate()
	{
		// If the zone is not active and it is either a single player game or the games host
		if (_running == 0 && !(isClient && !isServer))
		{
			_running = 1;
			// Get a random size
			_size = Random.Range(1.0f, 2.0f);
			transform.localScale = new Vector3(_size, _size, 1.0f);
			// Place the zone in a random place on the map
			transform.localPosition = new Vector3(Random.Range(-8.645f, 8.645f), Random.Range(-4.775f, 4.775f), transform.localPosition.z);
			// Set the initial alpha value to 0
			_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			// Update the clients
			if (isServer)
			{
				RpcZones(_size, transform.localPosition);
			}
		}
		// Once the zone is above the activated threshold enable the collider
		if(_sprite.color.a > activated)
		{
			GetComponent<SphereCollider>().enabled = true;
		}
		else
		{
			GetComponent<SphereCollider>().enabled = false;
		}
	
		// First
		if (_sprite.color.a <= 1 && _running == 1)
		{
			// Make the zone opaque
			_sprite.color = new Color(1.0f, 1.0f, 1.0f, _sprite.color.a + (_speed / 100));
		}
		// Second
		else if(_sprite.color.a >= 1 && _running == 1)
		{
			// Wait for the counter to end
			_counter = Random.Range(50, 150);
			_running = 2;
		}
		// Third
		else if (_sprite.color.a > 0 && _running == 3)
		{
			// Make the zone transparent
			_sprite.color = new Color(1.0f, 1.0f, 1.0f, _sprite.color.a - (_speed / 100));
		}
		// Fourth
		else if (_sprite.color.a <= 0 && _running == 3)
		{
			// Wait for the counter to end
			_counter = Random.Range(100, 300);
			_running = 4;
		}

		// Decrement counter if its above 0
		if (_counter > 0)
		{
			_counter--;
			if (_counter == 0)
			{
				if (_running == 2)
				{
					_running = 3;
				}
				else
				{
					_running = 0;
				}
			}
		}
	}

	//Update zones
	[ClientRpc]
	public void RpcZones(float size, Vector3 position)
	{
		_running = 1;
		_size = size;
		transform.localPosition = position;
		_sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}
}
