using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayGen.SUGAR.Unity;

// Controller for an individual player
// Unless it is the local player most of this script is ignored
public class PlayerController : NetworkBehaviour
{
	public float Speed, Time;
	public GameObject Hearts, ScoreDisplay, Marker;
	public Sprite[] MarkerSprites;
	public bool Server, Client, InGame, Damage;
	public GameObject[] Stripes;
	public Material[] Materials;
	public int Invincibility, Health, State, Score, Mode, ID;

	AbilityController _abilities;
	Controller _controller;
	int _colour;
	float _x, _y, _z;
	bool _connected;
	Vector3 _initialPosition;

	// Use this for initialization
	void Start()
	{
		_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();
		_abilities = GetComponent<AbilityController>();
		// Assign the players hearts/score to the rankings object so it stays in the top left of the screen
		Hearts.transform.parent = GameObject.FindGameObjectWithTag("Rankings").transform;
		// You can buy speed boosts
		if(_controller.Requested.ContainsKey("speed"))
		{
			Speed += _controller.Requested["speed"];
		}

		_initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		_connected = false;
		Damage = false;
		InGame = false;
		Server = false;
		Client = false;
		Score = 0;
		State = 0;
		ID = -1;
		Mode = 2;
		Time = 99999;
		
		// Assign each player a colour
		if (isServer)
		{
			_colour = GameObject.FindGameObjectsWithTag("Player").Length - 1;
		}
		SetColour();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// If the user is the server tell all other users what colour this player should be
		if (isServer)
		{
			RpcColour(_colour);
		}
		// Versus
		if (Mode == 1)
		{
			// Show individual score and hide player marker
			ScoreDisplay.SetActive(true);
			Marker.SetActive(false);
		}
		// Single Player / CO-OP
		else
		{
			// Hide individual score and show player marker
			// There is only 1 score at the top of the screen for these game modes
			ScoreDisplay.SetActive(false);
			Marker.SetActive(true);
			Marker.GetComponent<SpriteRenderer>().sprite = MarkerSprites[_colour];
		}
		// Get the players score and health
		Score = ScoreDisplay.GetComponent<PlaceController>().Score;
		Health = Hearts.GetComponent<HeartController>().Health;
		// Players are continuously spinning
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z - 2.0f);

		// If a player recently took damage it becomes invincible for a while
		Invinc();
		// If the player has died or is not in the game hide them
		OutOfTheWay();

		// If this is not the local player ignore the rest of the script
		if (!isLocalPlayer && !_controller.SinglePlayer)
		{
			return;
		}
		// Is the player a server or client. If both then just server is true
		Server = isServer;
		Client = isClient && !Server;
		
		// Move the player
		Move();
	}

	//Send Players Name to the Server if it's the local player
	public void SendName()
	{
		if (isLocalPlayer)
		{
			CmdName(_controller.PlayerID);
		}
	}

	//If this player isn't in this game or has died move them off screen
	void OutOfTheWay()
	{
		if (Health == 0 && State == 0)
		{
			transform.localPosition = new Vector3(400.0f, 0.0f, 0.0f);
			State = 1;
		}
		if (_controller.Started == true && !InGame && State == 0)
		{
			transform.localPosition = new Vector3(300.0f, 0.0f, 0.0f);
			Hearts.GetComponent<HeartController>().Health = 0;
			State = 2;
		}
	}

	//Move controller
	void Move()
	{
		transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		_x = transform.GetComponent<Rigidbody>().velocity.x;
		_y = transform.GetComponent<Rigidbody>().velocity.y;
		_z = transform.GetComponent<Rigidbody>().velocity.z;
		if (_controller.Started && _controller.GameOverCounter == 0)
		{
			var x = Input.GetAxisRaw("Horizontal");
			var y = Input.GetAxisRaw("Vertical");
			_y = Speed * y / 10;
			_x = Speed * x / 10;
			// Pythagoras to make sure you constantly move at the right speed
			if (_x != 0 && _y != 0)
			{
				_x = _x / Mathf.Abs(_x);
				_y = _y / Mathf.Abs(_y);
				_x = (Speed / 10) * Mathf.Cos(45.0f * Mathf.PI / 180.0f) * _x;
				_y = (Speed / 10) * Mathf.Cos(45.0f * Mathf.PI / 180.0f) * _y;
			}
		}
		transform.GetComponent<Rigidbody>().velocity = new Vector3(_x, _y, _z);

		// Player can't move outside of the world
		if (Health > 0 && InGame)
		{
			if (transform.localPosition.x >= 8.64)
			{
				transform.localPosition = new Vector3(8.64f, transform.localPosition.y, transform.localPosition.z);
			}
			else if (transform.localPosition.x <= -8.64)
			{
				transform.localPosition = new Vector3(-8.64f, transform.localPosition.y, transform.localPosition.z);
			}
			if (transform.localPosition.y >= 4.57)
			{
				transform.localPosition = new Vector3(transform.localPosition.x, 4.57f, transform.localPosition.z);
			}
			else if (transform.localPosition.y <= -4.57)
			{
				transform.localPosition = new Vector3(transform.localPosition.x, -4.57f, transform.localPosition.z);
			}
		}
	}

	//Set players colour and the colour of its score
	public void SetColour()
	{
		GetComponent<Renderer>().material = Materials[_colour];
		ScoreDisplay.GetComponent<PlaceController>().Offset = _colour * 10;
	}

	//Reset Player
	public void ResetPlayer()
	{
		transform.localPosition = new Vector3(_initialPosition.x, _initialPosition.y, _initialPosition.z);
		Hearts.GetComponent<HeartController>().Health = 5;
		InGame = false;
		State = 0;
	}

	//If invincible switch material
	void Invinc()
	{
		// Flash different colours to indicate that it's invincible
		if (Invincibility > 0)
		{
			if (Invincibility % 10 > 5)
			{
				Stripes[0].GetComponent<Renderer>().material = Materials[_colour];
				Stripes[1].GetComponent<Renderer>().material = Materials[_colour];
				GetComponent<Renderer>().material = Materials[4];
			}
			else
			{
				Stripes[0].GetComponent<Renderer>().material = Materials[4];
				Stripes[1].GetComponent<Renderer>().material = Materials[4];
				GetComponent<Renderer>().material = Materials[_colour];
			}
			Invincibility--;
		}
	}

	//When hit by a dodgeball/Ability turn Damage to true
	void OnTriggerEnter(Collider other)
	{
		// Handles colliding with a damage dealing object
		CollisionHandler(other.transform);
		// Collected a heart pick up
		if (other.tag == "Health" && Health < 5)
		{
			if(other.GetComponent<BonusHealth>().Player != gameObject)
			{
				CmdHeart(other.GetComponent<BonusHealth>().Player);
			}
		}
	}

	// Handles being hit by a dodgeball or an ability
	void OnTriggerStay(Collider other)
	{
		// Handles colliding with a damage dealing object
		CollisionHandler(other.transform);
	}


	// Handles collisions between the player and other objects
	void CollisionHandler (Transform other)
	{
		bool hit = false;
		// Hit by dodgeball
		if(other.tag == "DodgeBall" && (isLocalPlayer || !isClient) && other.GetComponent<MeshRenderer>().enabled)
		{
			hit = true;
		}
		// Hit by blades
		if (other.tag == "Star" && other.transform.parent != transform && other.transform.parent != transform)
		{
			hit = true;
		}
		// Hit by explosion
		if (other.tag == "Explosion")
		{
			hit = true;
		}
		// Hit by bullet
		if (other.tag == "Bullet")
		{
			if (other.GetComponent<BulletController>().From != transform)
			{
				hit = true;
			}
		}
		// Hit
		if (hit)
		{
			if (isClient && isLocalPlayer)
			{
				// Tell the server
				CmdCollision();
			}
			else
			{
				// If you have a shield negate the damage and deactivate the shield
				Damage = true;
				if (_abilities.Shield.activeSelf)
				{
					Hearts.GetComponent<HeartController>().Health++;
					_abilities.Shield.SetActive(false);
				}
			}
		}
	}
	
	//Update in single player game
	public void Scores(int score)
	{
		_controller.SharedScore.GetComponent<PlaceController>().Score += score;
	}


	//Update scores on the server
	[ClientRpc]
	public void RpcScores(int score)
	{
		//Co-op
		if(Mode == 2)
		{
			// Add to the shared score
			_controller.SharedScore.GetComponent<PlaceController>().Score += score;
		}
		// Versus
		else
		{
			// Add to personal score
			ScoreDisplay.GetComponent<PlaceController>().Score += score;
		}
	}

	//Set colour
	[ClientRpc]
	public void RpcColour(int number)
	{
		_colour = number;
		SetColour();
	}

	// The command which matches the players name on the server to the clients name
	[Command]
	void CmdName(int playerName)
	{
		ID = playerName;
	}


	// Lets the request button to talk the server. If the user is the server enables the request arrows
	// on the clients
	public void Request()
	{
		if(!isServer)
		{
			CmdRequest();
		}
		else
		{
			RpcArrows(_colour);
		}
	}

	// The command which matches the players name on the server to the clients name
	[Command]
	void CmdRequest()
	{
		RpcArrows(_colour);
	}

	// Sets request arrows on on the clients meaning people can send money to another user
	[ClientRpc]
	public void RpcArrows(int number)
	{
		if(!isLocalPlayer)
		{
			_controller.CoinArrows[number].SetActive(true);
			_controller.CoinArrows[number].GetComponent<Request>().Player = GetComponent<PlayerController>();
		}
	}

	// Lets the MoneyChange class talk to the server
	public void GaveMoney(GameObject player)
	{
		if (!isServer)
		{
			CmdGave(player);
		}
		else
		{
			RpcGave(player);
		}
	}

	// Tell the server you just gave a user coins
	[Command]
	void CmdGave(GameObject player)
	{
		RpcGave(player);
	}

	// Tell the server the player just hit a ball
	[Command]
	void CmdCollision()
	{
		// If you have a shield negate the damage and deactivate the shield
		Damage = true;
		if (_abilities.Shield.activeSelf)
		{
			Hearts.GetComponent<HeartController>().Health++;
			RpcShield();
		}
	}

	// Tell the server the player just hit a heart
	[Command]
	void CmdHeart(GameObject player)
	{
		RpcHeart(player);
	}
	
	// Informs all players a user just gained a heart and destorys the heart pickup
	[ClientRpc]
	public void RpcHeart(GameObject player)
	{
		// Can't directly call heart object as they are individually spawned on each client
		GameObject[] hearts = GameObject.FindGameObjectsWithTag("Health");
		foreach (GameObject h in hearts)
		{
			if(h.GetComponent<BonusHealth>().Player == player)
			{
				Destroy(h);
				Hearts.GetComponent<HeartController>().Health++;
			}
		}
	}

	// Tell the correct user they have just been given 1000 coins
	[ClientRpc]
	public void RpcGave(GameObject player)
	{
		if (player.GetComponent<PlayerController>().isLocalPlayer)
		{

			_controller.Requested.Remove("coins");
			_controller.GetResource("coins");
			_controller.Gain.GetComponent<MoneyChange>().Change(false);
		}
	}

	// Cancel shield on all clients
	[ClientRpc]
	public void RpcShield()
	{
		_abilities.Shield.SetActive(false);
	}
}
