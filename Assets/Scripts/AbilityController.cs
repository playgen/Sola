using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// Ability controller for an individual player
public class AbilityController : NetworkBehaviour
{
	public GameObject Spin, Shield, Bomb, Bullet, Health;
	public int CounterTime;

	Controller _controller;
	Transform[] _ability;
	bool _upgraded;
	int _abilityCounter, _spinCounter, _resizeCounter;

	// Use this for initialization
	void Start ()
	{
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
		_ability = _controller.Ability.GetComponentsInChildren<Transform>();
		_upgraded = false;
		_resizeCounter = 0;
		_abilityCounter = 0;
		_spinCounter = -1;
	}
	
	// If E is pressed and it is the local player connected to this script use an ability
	void FixedUpdate ()
	{
		// Reset the players speed and size to normal
		if (_resizeCounter > 0)
		{
			_resizeCounter--;
			if (_resizeCounter == 0)
			{
				if (_upgraded)
				{
					_upgraded = false;
					GetComponent<PlayerController>().Speed = GetComponent<PlayerController>().Speed / 2.0f;
				}
				else
				{
					GetComponent<PlayerController>().Speed = GetComponent<PlayerController>().Speed / 1.4f;
				}
				transform.localScale = new Vector3(0.4f, 0.4f, 0.1f);
			}
		}
		// If this is not the local player ignore the rest of the script
		if (!isLocalPlayer && !_controller.SinglePlayer)
		{
			return;
		}
		// When you use an ability if it is off cooldown and the game has started use it
		if (Input.GetKey(KeyCode.E) && _abilityCounter == 0 && _controller.Selected != -1 && _controller.Started)
		{
			_controller.UsedAbility();
			// Has the ability been upgraded
			bool upgrade = Upgraded(_controller.Selected);
			// In an online game tell the server you just used an ability
			if (isClient)
			{
				CmdAbility(_controller.Selected, Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position, upgrade);
			}
			else
			{
				Ability(_controller.Selected, Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position, upgrade);
			}
		}
		// Blades ability deactivates after a set amount of time
		if (_spinCounter == 0)
		{
			Spin.SetActive(false);
			Spin.transform.localScale = new Vector3(1.0f, 1.0f, 10.0f);
			_abilityCounter = CounterTime;
		}
		if (_spinCounter >= 0)
		{
			_spinCounter--;
		}

		// Ability cooldown/ displayed timer
		if (_abilityCounter > 0)
		{
			_abilityCounter--;
			int abilityCalVal = (_abilityCounter / (CounterTime / 8));
			if (abilityCalVal < 7)
			{
				_ability[abilityCalVal].GetComponent<SpriteRenderer>().enabled = false;
			}
			if(_abilityCounter == 0 && CounterTime == 600)
			{
				CounterTime = CounterTime / 2;
			}
		}
	}
	
	public void Ability(float selected, Vector3 mouse, Vector3 regular, bool upgrade)
	{
		// Use the correct ability
		if (selected == 0)
		{
			Block(upgrade);
		}
		else if (selected == 1)
		{
			Blink(upgrade);
		}
		else if (selected == 2)
		{
			Tiny(upgrade);
		}
		else if (selected == 3)
		{
			Heart(mouse, regular, upgrade);
		}
		else if (selected == 4)
		{
			Spinners(upgrade);
		}
		else if (selected == 5)
		{
			Turret(upgrade);
		}
		else if (selected == 6)
		{
			Explode(mouse, regular, upgrade);
		}
		// Display cooldown timer
		if (isLocalPlayer || _controller.SinglePlayer)
		{
			foreach (Transform t in _ability)
			{
				t.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
	}


	//Blink ability
	void Blink(bool upgrade)
	{
		// Find the angle between the player and the mouse
		_abilityCounter = CounterTime;
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float angle;
		if (mouse.x - transform.position.x >= 0.0f)
		{
			angle = 90 - Mathf.Atan((mouse.y - transform.position.y) / (mouse.x - transform.position.x)) * 180.0f / Mathf.PI;
		}
		else
		{
			angle = 270 - Mathf.Atan((mouse.y - transform.position.y) / (mouse.x - transform.position.x)) * 180.0f / Mathf.PI;
		}
		angle = angle % 90;
		// Get a fixed distance using pythagoras
		float x = Mathf.Sin(angle / 180.0f * Mathf.PI) * 2.0f;
		float y = Mathf.Cos(angle / 180.0f * Mathf.PI) * 2.0f;
		// If upgraded increase blink distance
		if (upgrade)
		{
			x = x * 1.5f;
			y = y * 1.5f;
		}
		// Move the player by a fixed distance in the correct direction
		if (mouse.x - transform.position.x >= 0.0f && mouse.y - transform.position.y >= 0.0f)
		{
			transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
		}
		else if (mouse.x - transform.position.x < 0.0f && mouse.y - transform.position.y < 0.0f)
		{
			transform.localPosition = new Vector3(transform.localPosition.x - x, transform.localPosition.y - y, transform.localPosition.z);
		}
		else if (mouse.x - transform.position.x < 0.0f && mouse.y - transform.position.y >= 0.0f)
		{
			transform.localPosition = new Vector3(transform.localPosition.x - y, transform.localPosition.y + x, transform.localPosition.z);
		}
		else
		{
			transform.localPosition = new Vector3(transform.localPosition.x + y, transform.localPosition.y - x, transform.localPosition.z);
		}
	}

	//Blades ability
	void Spinners(bool upgrade)
	{
		// If upgraded increase the razors size
		if (upgrade)
		{
			Spin.transform.localScale = new Vector3(2.0f, 2.0f, 10.0f);
		}
		// Set blades active
		Spin.SetActive(true);
		_spinCounter = CounterTime / 4;
		_abilityCounter = -1;
	}

	//Shield ability
	void Block(bool upgrade)
	{
		// If upgraded increase shield duration

		// Set shield active
		Shield.SetActive(true);
		if (!upgrade)
		{
			CounterTime = CounterTime * 2;
		}
		_abilityCounter = CounterTime;
	}

	//Bomb ability
	void Explode(Vector3 mouse, Vector3 regular, bool upgrade)
	{
		// Make a bomb and fire it in the direction of the mouse
		GameObject bomb = Instantiate(Bomb);
		bomb.transform.position = transform.position;
		_abilityCounter = CounterTime;

		float angle;
		if (mouse.x - regular.x >= 0.0f)
		{
			angle = 90 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		else
		{
			angle = 270 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		angle = angle % 90;
		float x = Mathf.Sin(angle / 180.0f * Mathf.PI) * 2.0f;
		float y = Mathf.Cos(angle / 180.0f * Mathf.PI) * 2.0f;
		if (mouse.x - regular.x >= 0.0f && mouse.y - regular.y >= 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(x, y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y < 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(-x, -y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y >= 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(-y, x, 0);
		}
		else
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(y, -x, 0);
		}

		// If upgraded fire another bomb behind you
		if (upgrade)
		{
			GameObject upBomb = Instantiate(Bomb);
			bomb.transform.position = transform.position;
			if (mouse.x - regular.x >= 0.0f && mouse.y - regular.y >= 0.0f)
			{
				bomb.GetComponent<Rigidbody>().velocity = new Vector3(-x, -y, 0);
			}
			else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y < 0.0f)
			{
				bomb.GetComponent<Rigidbody>().velocity = new Vector3(x, y, 0);
			}
			else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y >= 0.0f)
			{
				bomb.GetComponent<Rigidbody>().velocity = new Vector3(y, -x, 0);
			}
			else
			{
				bomb.GetComponent<Rigidbody>().velocity = new Vector3(-y, x, 0);
			}
		}
	}

	// Fire bullets into the surrounding area
	void Turret(bool upgrade)
	{
		// If upgraded fire more bullets
		int amount;
		if(upgrade)
		{
			amount = 6;
		}
		else
		{
			amount = 3;
		}

		// Fire bullets
		float turn = 0.0f;
		for (int i = 0; i < amount; i++)
		{
			GameObject bullet = Instantiate(Bullet);
			bullet.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			bullet.transform.localEulerAngles = new Vector3(0.0f, 0.0f, turn);
			bullet.GetComponentInChildren<BulletController>().From = transform;
			turn += 360.0f / amount;
		}
		_abilityCounter = CounterTime;
	}

	// Shrink the player and gain a speed boost
	void Tiny(bool upgrade)
	{
		// If upgraded increase the speed by more
		if(upgrade)
		{
			GetComponent<PlayerController>().Speed = GetComponent<PlayerController>().Speed * 2.0f;
			_upgraded = true;
		}
		else
		{
			GetComponent<PlayerController>().Speed = GetComponent<PlayerController>().Speed * 1.4f;
		}
		transform.localScale = new Vector3(0.2f, 0.2f, 0.1f);
		_resizeCounter = 150;
		_abilityCounter = CounterTime;
	}

	// Throw out a heart that can be picked up by the other users
	void Heart(Vector3 mouse, Vector3 regular, bool upgrade)
	{
		// Make a heart and fire it in the direction of the mouse
		GameObject heart = Instantiate(Health);
		heart.GetComponent<BonusHealth>().Player = gameObject;
		heart.transform.position = transform.position;
		_abilityCounter = CounterTime;

		float angle;
		if (mouse.x - regular.x >= 0.0f)
		{
			angle = 90 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		else
		{
			angle = 270 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		angle = angle % 90;
		float x = Mathf.Sin(angle / 180.0f * Mathf.PI) * 2.0f;
		float y = Mathf.Cos(angle / 180.0f * Mathf.PI) * 2.0f;
		if (mouse.x - regular.x >= 0.0f && mouse.y - regular.y >= 0.0f)
		{
			heart.GetComponent<Rigidbody>().velocity = new Vector3(x, y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y < 0.0f)
		{
			heart.GetComponent<Rigidbody>().velocity = new Vector3(-x, -y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y >= 0.0f)
		{
			heart.GetComponent<Rigidbody>().velocity = new Vector3(-y, x, 0);
		}
		else
		{
			heart.GetComponent<Rigidbody>().velocity = new Vector3(y, -x, 0);
		}
		GetComponent<PlayerController>().Damage = true;


		// If upgraded send another heart behind you
		if (upgrade)
		{
			GameObject upHeart = Instantiate(Health);
			upHeart.GetComponent<BonusHealth>().Player = gameObject;
			upHeart.transform.position = transform.position;
			if (mouse.x - regular.x >= 0.0f && mouse.y - regular.y >= 0.0f)
			{
				upHeart.GetComponent<Rigidbody>().velocity = new Vector3(-x, -y, 0);
			}
			else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y < 0.0f)
			{
				upHeart.GetComponent<Rigidbody>().velocity = new Vector3(x, y, 0);
			}
			else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y >= 0.0f)
			{
				upHeart.GetComponent<Rigidbody>().velocity = new Vector3(y, -x, 0);
			}
			else
			{
				upHeart.GetComponent<Rigidbody>().velocity = new Vector3(-y, x, 0);
			}
		}
	}

	// Tells the server to tell all the clients this player just used an ability
	[Command]
	void CmdAbility(float selected, Vector3 mouse, Vector3 regular, bool upgrade)
	{
		GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>().RpcAbility(gameObject, selected, mouse, regular, upgrade);
	}

	// Check if an ability has been upgraded
	bool Upgraded(int selected)
	{
		if(selected == 0)
		{
			return _controller.Requested.ContainsKey("shield");
		}
		else if(selected == 1)
		{
			return _controller.Requested.ContainsKey("blink");
		}
		else if (selected == 2)
		{
			return _controller.Requested.ContainsKey("tiny");
		}
		else if (selected == 3)
		{
			return _controller.Requested.ContainsKey("heart");
		}
		else if (selected == 4)
		{
			return _controller.Requested.ContainsKey("razor");
		}
		else if (selected == 5)
		{
			return _controller.Requested.ContainsKey("gun");
		}
		else
		{
			return _controller.Requested.ContainsKey("bomb");
		}
	}
}
