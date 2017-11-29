using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkBehaviour {

	public GameObject NetworkManager, DBGenerator, Cont, Dodgeball;
	public GameObject[] Dots = new GameObject[4];
	public Sprite[] Sprites = new Sprite[2];
	public int Players, OldPlayers;
	public bool Server, Client;

	NetworkManager _networkManager;
	GameObject[] _updatedBalls;
	float[] _updates;
	int[] _lives;
	float _temp;
	public int _length;

	// Use this for initialization
	void Start ()
	{
		_updatedBalls = DBGenerator.GetComponent<DBGenerator>().BallUpdates;
		_networkManager = NetworkManager.GetComponent<NetworkManager>();
		Players = 0;
		_temp = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		Server = isServer;
		Client = isClient;
		PlayerDots();
		if (isServer)
		{
			ServerUpdate();
		}
		Name();
	}

	//Server update stuff
	void ServerUpdate()
	{
		DBGenerator.GetComponent<DBGenerator>().Server = true;
		int x = _networkManager.numPlayers;
		Players = x;
		bool start = false;
		if (Input.GetKey(KeyCode.Tab) && Cont.GetComponent<Controller>().StartCounter == -1 && Players > 0)
		{
			start = true;
		}
		CreateArray();
		for (int k = 0; k < Cont.GetComponent<Controller>().Players.Length; k++)
		{
			if (Cont.GetComponent<Controller>().Players[k].GetComponent<PlayerController>().Damage)
			{
				Cont.GetComponent<Controller>().Players[k].GetComponent<PlayerController>().Damage = false;
				if(!isClient)
				{
					LoseLife(k);
				}
				RpcLoseLife(k);
			}
		}
		RpcUpdates(x, start, _updates, _length, Cont.GetComponent<Controller>().Started);
		if (!isClient)
		{
			Updates(x, start, _updates, _length);
		}
	}

	//Name stuff
	void Name()
	{
		if (isClient)
		{
			int x = Cont.GetComponent<Controller>().Players.Length;
			string[] names = new string[x];
			float[] scales = new float[x];
			//Make it send the name the the server
			for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
			{
				Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().SendName();
				names[i] = Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().Name;
			}
			if (isServer)
			{
				RpcNames(names);
				if(!isClient)
				{
					Names(names);
				}
			}
		}
	}

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

	//Create an array of all the updated dodgeball values
	public void CreateArray()
	{
		_length = 0;
		for (int j = 0; j < _updatedBalls.Length; j++)
		{
			if (_updatedBalls[j] != null)
			{
				_length++;
			}
		}
		_updates = new float[_length * 12];
		int q = 0;
		for (int i = 0; i < _length * 12; i = i + 12)
		{
			if (_updatedBalls[q] != null)
			{
				_updates[i + 0] = (float)q;
				_updates[i + 1] = _updatedBalls[q].GetComponentInChildren<DodgeController>().MaterialNum;
				_updates[i + 2] = _updatedBalls[q].GetComponentInChildren<DodgeController>().ArrowNum;
				_updates[i + 3] = _updatedBalls[q].transform.localEulerAngles.z;
				_updates[i + 4] = _updatedBalls[q].transform.localPosition.x;
				_updates[i + 5] = _updatedBalls[q].transform.localPosition.y;
				_updates[i + 6] = _updatedBalls[q].GetComponentInChildren<DodgeController>().transform.localScale.y;
				_updates[i + 7] = _updatedBalls[q].GetComponentInChildren<DodgeController>().Min;
				_updates[i + 8] = _updatedBalls[q].GetComponentInChildren<DodgeController>().Max;
				_updates[i + 9] = _updatedBalls[q].GetComponentInChildren<DodgeController>().State;
				_updates[i + 10] = _updatedBalls[q].GetComponentInChildren<DodgeController>().RotationSpeed;
				_updates[i + 11] = _updatedBalls[q].GetComponentInChildren<DodgeController>().Speed;
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
			GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().PulseFive = true;
		}
		else
		{
			GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().Pulse = true;
		}
	}

	//Update clients
	[ClientRpc]
	public void RpcUpdates(int x, bool start, float[] dodgeballs, int length, bool started)
	{
		//Send the updated values for the dodgeballs
		GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().UpdateNumbers = dodgeballs;
		Players = x;
		Cont.GetComponent<Controller>().Started = started;
		//If the game has just started tell everyone who is currently there they are in that game and start the countdown
		if (start)
		{
			Cont.GetComponent<Controller>().StartCounter = 60;
			for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
			{
				Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().InGame = true;
				GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().InGame = true;
				GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().Counter = 0;
			}
		}
	}

	//Send a meessage to all the clients that someone has lost health
	[ClientRpc]
	public void RpcLoseLife(int position)
	{
		if(Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health != 0 && Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Invincibility == 0)
		{
			Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Invincibility = 50;
			Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health--;
		}
	}

	//Send out an updated list of the names of the current players
	[ClientRpc]
	public void RpcNames(string[] names)
	{
		for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
		{
			Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().Name = names[i];
		}
	}

	//Reset Everything
	[ClientRpc]
	public void RpcResets()
	{
		for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
		{
			Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().ResetPlayer();
			Cont.GetComponent<Controller>().GameOver();
		}
	}

	//Disconnects
	[ClientRpc]
	public void RpcDisconnect(int length)
	{
		if (isServer)
		{
			if (length == 1)
			{
				NetworkManager.GetComponent<NetworkManager>().StopHost();
				Cont.GetComponent<Controller>().Server = false;
				Players = 0;
				Server = false;
				Client = false;
			}
			Cont.GetComponent<Controller>().Clear();
		}
		else
		{
			NetworkManager.GetComponent<NetworkManager>().StopClient();
			Cont.GetComponent<Controller>().Clear();
			Players = 0;
			Server = false;
			Client = false;
		}
	}

	//Ability
	[ClientRpc]
	public void RpcAbility(GameObject player, float selected, Vector3 mouse, Vector3 regular)
	{
		player.GetComponent<PlayerController>().Ability(selected, mouse, regular);
	}

	public void Dodge(bool five)
	{
		if (five)
		{
			GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().PulseFive = true;
		}
		else
		{
			GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().Pulse = true;
		}
	}

	//Standard methods of the ClientRpcs for server only users
	public void Updates(int x, bool start, float[] dodgeballs, int length)
	{
		//Send the updated values for the dodgeballs
		GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().UpdateNumbers = dodgeballs;
		Players = x;
		//If the game has just started tell everyone who is currently there they are in that game and start the countdown
		if (start)
		{
			Cont.GetComponent<Controller>().StartCounter = 60;
			for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
			{
				Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().InGame = true;
				GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().InGame = true;
				GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().Counter = 0;
			}
		}
	}

	public void LoseLife(int position)
	{
		if (Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health != 0 && Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Invincibility == 0)
		{
			Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Invincibility = 50;
			Cont.GetComponent<Controller>().Players[position].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health--;
		}
	}

	public void Names(string[] names)
	{
		for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
		{
			Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().Name = names[i];
		}
	}

	public void Resets()
	{
		for (int i = 0; i < Cont.GetComponent<Controller>().Players.Length; i++)
		{
			Cont.GetComponent<Controller>().Players[i].GetComponent<PlayerController>().ResetPlayer();
			Cont.GetComponent<Controller>().GameOver();
		}
	}
}
