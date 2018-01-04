using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// This is the main controller for UNet iteractions. It contains most of the methods for talking to the server and clients
public class NetworkController : NetworkBehaviour {

	public GameObject Dodgeball, Controller;
	public GameObject[] Dots = new GameObject[4];
	public Sprite[] Sprites = new Sprite[2];
	public int Players, OldPlayers;
	public bool Server, Client;

	Controller _controller;
	NetworkManager _networkManager;
	DBGenerator _dBGenerator;
	GameObject[] _updatedBalls;
	float[] _updates;
	int[] _lives;
	float _temp;
	public int _length;

	// Use this for initialization
	void Start ()
	{
		// Get the dodgeball generator
		_dBGenerator = GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>();
		_controller = Controller.GetComponent<Controller>();
		// Get an array of all the dodgeballs that have changes to be sent
		// Balls should only be updated when there is a change otherwise it can make the dodgeballs movement choppy
		_updatedBalls = _dBGenerator.BallUpdates;

		_networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
		Players = 0;
		_temp = 0.0f;
	}
	
	void FixedUpdate ()
	{
		// Is this instance a server and/or a client
		Server = isServer;
		Client = isClient;

		// Get the number of players connected to the server
		Players = _controller.Players.Length;

		// Whilst in the lobby shows the amount of players currently connected
		PlayerDots();

		// If the instance is a server update the clients
		if (isServer)
		{
			ServerUpdate();
		}


		// Assigns each player a username which is there SUGAR account name
		Name();
	}

	//Server update
	void ServerUpdate()
	{
		// Tell the DB generator that this instance is the server
		_dBGenerator.Server = true;
		
		// If there is at least 1 player, the game hasn't started yet and the server presses TAB start the game
		bool start = false;
		if (Input.GetKey(KeyCode.Tab) && _controller.StartCounter == -1 && Players > 0 && (_controller.Chosen || _controller.SinglePlayer))
		{
			start = true;
		}

		// The server handles all collisions so if you lose a life on the server then inform all the clients
		for (int k = 0; k < _controller.Players.Length; k++)
		{
			if (_controller.Players[k].GetComponent<PlayerController>().Damage)
			{
				_controller.Players[k].GetComponent<PlayerController>().Damage = false;
				RpcLoseLife(k);
			}
		}

		// Creates an array with all the updated dodgeball information which is then sent to the clients. Less lag than
		// with NetworkIndentities
		CreateArray();

		// Sends all the updated information to the clients
		RpcUpdates(Players, start, _updates, _length, _controller.Started);
	}

	//Send account names from the clients to the server and from the server to clients
	void Name()
	{
		int[] ids = new int[Players];
		if (isClient)
		{
			//Send the name the the server
			for (int i = 0; i < Players; i++)
			{
				PlayerController playCont = _controller.Players[i].GetComponent<PlayerController>();
				playCont.SendName();
				// Add the name to the array
				ids[i] = playCont.ID;
			}
		}
		// Send all the names to each client
		if (isServer)
		{
			RpcNames(ids);
		}
	}

	// Display the amount of players in the lobby
	void PlayerDots()
	{
		for (int i = 0; i < Dots.Length; i++)
		{
			if (Players > i)
			{
				Dots[i].GetComponent<SpriteRenderer>().sprite = Sprites[0];
			}
			else
			{
				Dots[i].GetComponent<SpriteRenderer>().sprite = Sprites[1];
			}
		}
	}

	//Create an array of all the updated dodgeball values so they can be sent to the clients
	// This way there is less lag than with a networkIdentity
	public void CreateArray()
	{
		// Get the current number of dodgeballs
		_length = 0;
		for (int j = 0; j < _updatedBalls.Length; j++)
		{
			if (_updatedBalls[j] != null)
			{
				_length++;
			}
		}

		// Each dodgeball has 13 pieces of information it need to match with the dodgeball on the server
		_updates = new float[_length * 13];
		int q = 0;
		for (int i = 0; i < _length * 13; i = i + 13)
		{
			// The balls are not always stored in the order 1-7 but could be 1,5,7,9,10 so this skips the null values
			if (_updatedBalls[q] != null)
			{
				// For each ball we require
				// 0. Its ball number
				// 1, 2. Its colour and the colour of its arrow
				// 3, 4, 5, 6. Its rotation, Position and size
				// 7, 8. The furthest it can rotate
				// 9. Its current state (Waiting, Charging, Fired)
				// 10, 11. The balls rotation Speed and movement speed
				// 12. If it is a special ball and if so which kind
				DodgeController ball = _updatedBalls[q].GetComponentInChildren<DodgeController>();
				_updates[i + 0] = (float) q;
				_updates[i + 1] = ball.MaterialNum;
				_updates[i + 2] = ball.ArrowNum;
				_updates[i + 3] = ball.Parent.transform.localEulerAngles.z;
				_updates[i + 4] = ball.Parent.transform.localPosition.x;
				_updates[i + 5] = ball.Parent.transform.localPosition.y;
				_updates[i + 6] = ball.transform.localScale.y;
				_updates[i + 7] = ball.Min;
				_updates[i + 8] = ball.Max;
				_updates[i + 9] = ball.State;
				_updates[i + 10] = ball.RotationSpeed;
				_updates[i + 11] = ball.Speed;
				_updates[i + 12] = ball.Special;
				_updatedBalls[q] = null;
			}
			else
			{
				i--;
			}
			q++;
		}
	}

	//Send a message to the clients they should spawn another dodgeball (or 5)
	[ClientRpc]
	public void RpcDodge(bool five)
	{
		if (five)
		{
			// Spawn 5 balls
			_dBGenerator.PulseFive = true;
		}
		else
		{
			// Sppawn 1 ball
			_dBGenerator.Pulse = true;
		}
	}

	//Update clients
	[ClientRpc]
	public void RpcUpdates(int x, bool start, float[] dodgeballs, int length, bool started)
	{
		//Send the updated values for the dodgeballs
		if(_dBGenerator != null)
		{
			_dBGenerator.UpdateNumbers = dodgeballs;
		}
		Players = x;
		_controller.Started = started;
		//If the game has just started tell everyone who is currently there they are in that game and start the countdown
		if (start)
		{
			_controller.StartCounter = 61;
			for (int i = 0; i < _controller.Players.Length; i++)
			{
				_controller.Players[i].GetComponent<PlayerController>().InGame = true;
				_dBGenerator.InGame = true;
				_dBGenerator.Counter = 0;
			}
		}
	}

	//Send a message to all the clients that someone has lost health
	[ClientRpc]
	public void RpcLoseLife(int position)
	{
		PlayerController pCont = _controller.Players[position].GetComponent<PlayerController>();
		if (pCont.Hearts.GetComponent<HeartController>().Health != 0 && _controller.Players[position].GetComponent<PlayerController>().Invincibility == 0)
		{
			pCont.Invincibility = 50;
			pCont.Hearts.GetComponent<HeartController>().Health--;
		}
	}

	//Send out an updated list of the names of the current players
	[ClientRpc]
	public void RpcNames(int[] ids)
	{
		for (int i = 0; i < _controller.Players.Length; i++)
		{
			_controller.Players[i].GetComponent<PlayerController>().ID = ids[i];
		}
	}

	//Reset Everything
	[ClientRpc]
	public void RpcResets()
	{
		for (int i = 0; i < _controller.Players.Length; i++)
		{
			// Reset each player
			_controller.Players[i].GetComponent<PlayerController>().ResetPlayer();
			// Reset the game
			_controller.GameOver();
		}
	}

	// When the server disconnects kick everyone else first
	[ClientRpc]
	public void RpcDisconnect(int length)
	{
		// Server
		if (Server)
		{
			// If you are the last player disconnect
			if (length == 1)
			{
				Players = 0;
				Server = false;
				Client = false;
				_networkManager.StopHost();
			}
			// If there are still other players keep looping until everyone else has disconnected
			_controller.Clear();
		}
		// Clients disconnects immediately
		else
		{
			Players = 0;
			Server = false;
			Client = false;
			_controller.Clear();
			_networkManager.StopClient();
		}
	}

	// When an ability is used execute it on all clients as well
	[ClientRpc]
	public void RpcAbility(GameObject player, float selected, Vector3 mouse, Vector3 regular, bool upgrade)
	{
		player.GetComponent<AbilityController>().Ability(selected, mouse, regular, upgrade);
	}

	// Turn on the in game store
	[ClientRpc]
	public void RpcStore()
	{
		_controller.IGS.SetActive(true);
	}

}
