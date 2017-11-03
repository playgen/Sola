using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Controller : NetworkBehaviour
{
	public GameObject One, Two, Three, Wait, Host, Tab, Dots, Text, DBG, NetController, Rankings;
	public GameObject[] Players;
	public int StartCounter;
	public bool Started, Server, Client;
	public string PlayerName;
	public int[] Positions;

	NetworkManager _networkManager;
	GameObject _networkManagerGO, _background, _player;
	float _materialCounter, _time, _oldTime;
	bool _waiting, _loggedIn, _running;

	// Use this for initialization
	void Start()
	{
		_background = GameObject.FindGameObjectWithTag("Background");
		_networkManagerGO = GameObject.FindGameObjectWithTag("NetworkManager");
		_networkManager = _networkManagerGO.GetComponent<NetworkManager>();
		StartCounter = -1;
		_oldTime = 0.0f;
		Client = false;
		Server = false;
		Started = false;
		_running = false;
		_loggedIn = false;

		SUGARManager.Account.DisplayPanel(success =>
		{
			_networkManagerGO.GetComponent<NetworkManagerHUD>().enabled = true;
			PlayerName = SUGARManager.CurrentUser.Name;
			_loggedIn = true;
		});
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//Force people to log in
		if (!_loggedIn && !SUGARManager.Account.IsActive)
		{
			SUGARManager.Account.DisplayPanel(success =>
			{
				_networkManagerGO.GetComponent<NetworkManagerHUD>().enabled = true;
				PlayerName = SUGARManager.CurrentUser.Name;
				_loggedIn = true;
			});
		}
		Players = GameObject.FindGameObjectsWithTag("Player");
		Server = NetController.GetComponent<NetworkController>().Server;
		Client = NetController.GetComponent<NetworkController>().Client;
		if (!Server && !Client)
		{
			Clear();
		}
		WaitScreen();
		RankingManager();
		StartCounting();
		Background();
		if (_running)
		{
			Run();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			SUGARManager.Leaderboard.Display("longest_time", PlayGen.SUGAR.Common.LeaderboardFilterType.Top);
		}
	}

	//Reset after the game ends
	public void GameOver()
	{
		StartCounter = -1;
		_running = false;
		Started = false;
		One.SetActive(true);
		Two.SetActive(true);
		Three.SetActive(true);
		foreach (Transform child in Dots.transform)
		{
			child.gameObject.SetActive(true);
		}
		if (NetController.GetComponent<NetworkController>().Server)
		{
			Tab.SetActive(true);
		}
		else
		{
			Wait.SetActive(true);
			Host.SetActive(true);
		}
		GameObject[] balls = GameObject.FindGameObjectsWithTag("DodgeBall");
		foreach (GameObject b in balls)
		{
			Destroy(b);
		}
	}

	//Clear data if not connected to any server
	void Clear()
	{
		One.SetActive(false);
		Two.SetActive(false);
		Three.SetActive(false);
		Wait.SetActive(false);
		Host.SetActive(false);
		Tab.SetActive(false);
		foreach (Transform child in Dots.transform)
		{
			child.gameObject.SetActive(false);
		}
		GameObject[] balls = GameObject.FindGameObjectsWithTag("DodgeBall");
		if (NetController != null)
		{
			NetController.GetComponent<NetworkController>().Players = 0;
		}
		foreach (GameObject b in balls)
		{
			Destroy(b);
		}
		DBG.GetComponent<DBGenerator>().Counter = 0;
	}

	//The countdown between when TAB is pressed and when the game actually starts
	void StartCounting()
	{
		if (StartCounter > 0)
		{
			if (StartCounter == 60)
			{
				foreach (GameObject p in Players)
				{
					GameObject hearts = p.GetComponent<PlayerController>().Hearts;
					hearts.transform.localPosition = Vector3.zero;
					p.GetComponent<PlayerController>().SetColour();
					if (p.GetComponent<PlayerController>().Client || p.GetComponent<PlayerController>().Server)
					{
						_player = p;
					}
				}
				foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(false);
				}
				Wait.SetActive(false);
				Host.SetActive(false);
				Tab.SetActive(false);
				_running = true;
				_time = Time.time;
				if (Server)
				{
					NetController.GetComponent<NetworkController>().RpcDodge(true);
					if(!Client)
					{
						NetController.GetComponent<NetworkController>().Dodge(true);
					}
				}
			}
			StartCounter--;
			if (StartCounter == 0)
			{
				Started = true;
				One.SetActive(false);
			}
			else if (StartCounter == 20)
			{
				Two.SetActive(false);
			}
			else if (StartCounter == 40)
			{
				Three.SetActive(false);
			}
		}
	}

	//Whilst your waiting for other players this handles the sprites
	void WaitScreen()
	{
		if ((Server || NetController.GetComponent<NetworkController>().Client) && StartCounter == -1)
		{
			One.SetActive(true);
			Two.SetActive(true);
			Three.SetActive(true);
			foreach (Transform child in Dots.transform)
			{
				child.gameObject.SetActive(true);
			}
			if (Server)
			{
				Tab.SetActive(true);
			}
			else
			{
				Wait.SetActive(true);
				Host.SetActive(true);
			}
		}
	}

	//Decides the ordering for the current healths
	void RankingManager()
	{
		Positions = new int[Players.Length];
		for (int z = 0; z < Players.Length; z++)
		{
			Positions[z] = -1;
		}
		for (int k = 5; k >= 0; k--)
		{
			for (int l = 0; l < Players.Length; l++)
			{
				if (Players[l].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health == k)
				{
					int q = 0;
					while (Positions[q] != -1)
					{
						q++;
					}
					Positions[q] = l;
					Players[l].GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Place = l;
				}
			}
		}
		for (int z = 0; z < Players.Length; z++)
		{
			if (Started == true && !Players[Positions[z]].GetComponent<PlayerController>().InGame)
			{
				Players[Positions[z]].GetComponent<PlayerController>().Hearts.transform.localPosition = new Vector3(0.0f, 20.0f, 0.0f);
			}
			else
			{
				Players[Positions[z]].GetComponent<PlayerController>().Hearts.transform.localPosition = new Vector3(0.0f, -0.34f * z, 0.0f);
				Players[Positions[z]].GetComponent<PlayerController>().Hearts.transform.localEulerAngles = Vector3.zero;
			}
		}
	}

	//Runs only whilst the game is going on
	void Run()
	{
		float time = ((float)((int)(10.0f * (Time.time - _time))) / 10.0f);
		if (_oldTime != time && time % 5.0f == 0.0f && time <= 100.0f && Server)
		{
			NetController.GetComponent<NetworkController>().RpcDodge(false);
			if(!Client)
			{
				NetController.GetComponent<NetworkController>().Dodge(false);
			}
		}
		_oldTime = time;
		Text.GetComponent<Text>().text = "" + time;
		bool gameOver = true;
		for (int z = 0; z < Players.Length; z++)
		{
			if (Players[Positions[z]].GetComponent<PlayerController>().Health != 0)
			{
				gameOver = false;
			}
		}
		if (_player.GetComponent<PlayerController>().State == 1)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Resource.AddResource("too", (long) ((int) _oldTime), success =>
				{
					Debug.Log(success);
				});
				SUGARManager.GameData.Send("longest_time", _oldTime);
			}
			_player.GetComponent<PlayerController>().State = 2;
		}
		if (gameOver && Server)
		{
			NetController.GetComponent<NetworkController>().RpcResets();
			if(!Client)
			{
				NetController.GetComponent<NetworkController>().Resets();
			}
		}
	}

	//Background Manager
	void Background()
	{
		_background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, _materialCounter));
		_materialCounter += 0.001f;
	}
}

