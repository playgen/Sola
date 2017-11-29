using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Controller : NetworkBehaviour
{
	public GameObject One, Two, Three, Wait, Host, Tab, Dots, Text, NetController, Rankings, PlayerOffline, Ability, GameTime, Lights;
	public GameObject[] Players, Buttons, DBGS;
	public int[] Positions;

	public IDictionary<string, float> Requested;
	public int StartCounter, GameOverCounter;
	public bool Started, Server, Client, LoggedIn, SinglePlayer;
	public string PlayerName;

	NetworkManager _networkManager;
	GameObject _networkManagerGO, _player;
	float _materialCounter, _time, _oldTime, _coins;
	bool _waiting, _running;

	// Use this for initialization
	void Start()
	{
		_networkManagerGO = GameObject.FindGameObjectWithTag("NetworkManager");
		_networkManager = _networkManagerGO.GetComponent<NetworkManager>();
		Requested = new Dictionary<string, float>();
		StartCounter = -1;
		_oldTime = 0.0f;
		Client = false;
		Server = false;
		Started = false;
		_running = false;
		LoggedIn = false;
		SinglePlayer = GetComponent<TitleScreenController>().SinglePlayer;
		SUGARManager.Account.DisplayPanel(success =>
		{
			_networkManagerGO.GetComponent<NetworkManagerHUD>().enabled = true;
			PlayerName = SUGARManager.CurrentUser.Name;
			LoggedIn = true;
			Lights.GetComponent<ParticleController>().Run();
			GetResource("coins");
			GetResource("selected");
			foreach (GameObject up in Buttons)
			{
				GetResource(up.GetComponent<Upgrade>().Key);
			}
		});
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//Force people to log in
		if (!LoggedIn && !SUGARManager.Account.IsActive)
		{
			SUGARManager.Account.DisplayPanel(success =>
			{
				_networkManagerGO.GetComponent<NetworkManagerHUD>().enabled = true;
				PlayerName = SUGARManager.CurrentUser.Name;
				LoggedIn = true;
				GetResource("coins");
				GetResource("selected");
				foreach (GameObject up in Buttons)
				{
					GetResource(up.GetComponent<Upgrade>().Key);
				}
			});

		}
		SinglePlayer = GetComponent<TitleScreenController>().SinglePlayer;
		Players = GameObject.FindGameObjectsWithTag("Player");
		Server = NetController.GetComponent<NetworkController>().Server;
		Client = NetController.GetComponent<NetworkController>().Client;
		if (Input.GetKey(KeyCode.Escape) || (Client && !Server && Players.Length <= 1))
		{
			Clear();
		}
		WaitScreen();
		RankingManager();
		StartCounting();
		if (_running)
		{
			Run();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			SUGARManager.Leaderboard.Display("longest_time", PlayGen.SUGAR.Common.LeaderboardFilterType.Top);
			SetResource("coins", 5000);
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
		if (SinglePlayer)
		{
			Tab.SetActive(true);
		}
		else
		{
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
		}
		GameObject[] balls = GameObject.FindGameObjectsWithTag("DodgeBall");
		foreach (GameObject b in balls)
		{
			Destroy(b);
		}
		GetComponent<WinnerScreen>().Clear();
	}

	//Clear data if not connected to any server
	public void Clear()
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
		foreach (Transform t in Ability.GetComponentsInChildren<Transform>())
		{
			t.GetComponent<SpriteRenderer>().enabled = false;
		}
		foreach (GameObject DBG in DBGS)
		{
			DBG.GetComponent<DBGenerator>().Counter = 0;
			DBG.GetComponent<DBGenerator>().InGame = false;
		}
		StartCounter = -1;
		
		SinglePlayer = false;
		_running = false;
		Started = false;
		GetComponent<TitleScreenController>().Home();
		GetComponent<TitleScreenController>().SinglePlayer = false;

		Players = new GameObject[0];
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] balls = GameObject.FindGameObjectsWithTag("DodgeBall");
		foreach (GameObject b in balls)
		{
			Destroy(b);
		}
		foreach (GameObject p in players)
		{
			Destroy(p);
		}
		foreach (Transform child in Dots.transform)
		{
			child.gameObject.SetActive(false);
		}
		if (Client && !Server)
		{
			NetController.GetComponent<NetworkController>().Client = false;
			_networkManager.GetComponent<NetworkManager>().StopClient();
			NetController.GetComponent<NetworkController>().Players = 0;
			NetController.GetComponent<NetworkController>().Server = false;
			NetController.GetComponent<NetworkController>().Client = false;
		}
		if (Server)
		{
			NetController.GetComponent<NetworkController>().RpcDisconnect(GameObject.FindGameObjectsWithTag("Player").Length);
		}
		GetComponent<WinnerScreen>().Clear();
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
					if (p.GetComponent<PlayerController>().Client || p.GetComponent<PlayerController>().Server || SinglePlayer)
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
				if(SinglePlayer)
				{
					GetComponent<SingleController>().Dodge(true);
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
		if ((Server || NetController.GetComponent<NetworkController>().Client || SinglePlayer) && StartCounter == -1)
		{
			if(Players.Length == 0)
			{
				GameObject.Instantiate(PlayerOffline);
			}
			One.SetActive(true);
			Two.SetActive(true);
			Three.SetActive(true);
			if (!SinglePlayer)
			{
				foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(true);
				}
			}
			if (Server || SinglePlayer)
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
		GameTime.SetActive(true);
		if (_oldTime != time && time % 5.0f == 0.0f && time <= 100.0f && Server && time != 0.0f)
		{
			NetController.GetComponent<NetworkController>().RpcDodge(false);
			if(!Client)
			{
				NetController.GetComponent<NetworkController>().Dodge(false);
			}
		}
		if(_oldTime != time && time % 5.0f == 0.0f && time <= 100.0f && SinglePlayer)
		{
			GetComponent<SingleController>().Dodge(false);
		}
		_oldTime = time;
		bool gameOver = true;
		for (int z = 0; z < Players.Length; z++)
		{
			if (Players[Positions[z]].GetComponent<PlayerController>().Health != 0)
			{
				gameOver = false;
				Players[Positions[z]].GetComponent<PlayerController>().Time = time;
			}
		}
		if (_player.GetComponent<PlayerController>().State == 1)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Resource.Add("coins", (long) ((int) _oldTime), success =>
				{
					Debug.Log(success);
				});
				SUGARManager.GameData.Send("time", _oldTime);
			}
			_player.GetComponent<PlayerController>().State = 2;
		}
		if(gameOver)
		{
			GetComponent<WinnerScreen>().Run(Players);
			if(GameOverCounter == 0)
			{
				GameOverCounter = 400;
				Lights.GetComponent<ParticleController>().Run();
			}
			else
			{
				GameOverCounter--;
			}
		}
		else
		{
			if (GameOverCounter > 0)
			{
				GameOverCounter = 0;
			}
			GameTime.GetComponent<PlaceController>().Score = (int)time;
		}
		if (gameOver && Server && GameOverCounter == 0)
		{
			GameOver();
			NetController.GetComponent<NetworkController>().RpcResets();
			if(!Client)
			{
				NetController.GetComponent<NetworkController>().Resets();
			}
		}
		if (gameOver && SinglePlayer && GameOverCounter == 0)
		{
			GameOver();
			GetComponent<SingleController>().Resets();
		}
		if (!Started)
		{
			_time = Time.time;
		}
	}
	
	public void GetResource(string key)
	{
		if (SUGARManager.CurrentUser != null)
		{
			SUGARManager.Resource.Get(success =>
			{
				if (Requested.ContainsKey(key))
				{
					Requested.Remove(key);
				}
				if(success.Count > 0)
				{
					Requested.Add(key, success[0].Quantity);
				}
				else
				{
					Requested.Add(key, 0);
				}
			}, new string[] { key });
		}
	}

	public void AddResource(string key, float value)
	{
		if (SUGARManager.CurrentUser != null)
		{
			SUGARManager.Resource.Add(key, (long) value, success =>
			{
				Debug.Log(success);
			});
		}
	}

	public void SetResource(string key, float value)
	{
		if (SUGARManager.CurrentUser != null)
		{
			SUGARManager.Resource.Set(key, (long) value, success =>
			{
				Debug.Log(success);
			});
		}
	}
}

