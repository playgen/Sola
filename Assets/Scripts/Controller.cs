using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Controller : NetworkBehaviour
{
	public GameObject Wait, Host, Tab, Dots, Text, NetController, Rankings, PlayerOffline, Ability, Lights, SharedScore, Request, Gain, IGS;
	public GameObject[] Players, Buttons, DBGS, Countdown, Zones, CoinArrows;
	public PlayerController Player;

	public IDictionary<string, float> Requested;
	public int StartCounter, GameOverCounter, Number, PlayerID, Selected;
	public bool Started, LoggedIn, SinglePlayer, Chosen;
	
	GameObject[] _background, _gameMode;
	NetworkManager _networkManager;
	NetworkController _networkController;
	GameObject _networkManagerGO;
	float _materialCounter, _time, _oldTime, _coins;
	bool _waiting, _running, _storeLoaded;
	int _timeDiff, _mode;

	// Use this for initialization
	void Start()
	{
		// Network manager
		_networkManagerGO = GameObject.FindGameObjectWithTag("NetworkManager");
		// All the background cubes
		_background = GameObject.FindGameObjectsWithTag("Background");
		// NetworkController handles the communication between clients and the host. For single player games
		// the SingleController is used instead with similar methods only without the Rpc and Cmd calls
		_networkController = NetController.GetComponent<NetworkController>();
		_networkManager = _networkManagerGO.GetComponent<NetworkManager>();
		Requested = new Dictionary<string, float>();
		_gameMode = new GameObject[0];
		Selected = -1;
		StartCounter = -1;
		_oldTime = 0.0f;
		_storeLoaded = false;
		Started = false;
		Chosen = false;
		_running = false;
		LoggedIn = false;

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.B))
		{
			AddResource("coins", -100000);
		}
		// Once you have selected an item hide the store menu
		if (Selected != -1)
		{
			IGS.SetActive(false);
		}
		LogIn();
		// Boolean which tells if it's a single player game
		SinglePlayer = GetComponent<TitleScreenController>().SinglePlayer;
		// Get all the players currently in the game
		Players = GameObject.FindGameObjectsWithTag("Player");
		// Find the game mode selection sprites
		if(_gameMode.Length == 0)
		{
			_gameMode = GameObject.FindGameObjectsWithTag("Mode");
		}
		// When you quit a game or the host disconnects reset all the background variables and return to the home screen 
		if (LoggedIn && (Input.GetKey(KeyCode.Escape) || (_networkController.Client && !_networkController.Server && Players.Length <= 1)))
		{
			Clear();
			foreach (GameObject g in _background)
			{
				g.GetComponent<BackgroundController>().New(0);
			}
			foreach (GameObject z in GameObject.FindGameObjectsWithTag("Zone"))
			{
				Destroy(z);
			}
		}

		// Called whilst in the lobby but the game has not started yet
		WaitScreen();
		// Handles player rankings
		RankingManager();
		// Countdown between game initialization and it starting
		StartCounting();
		// Handles Game mode for multiplayer
		GameType();
		if (_running)
		{
			Run();
		}
	}

	// Handles player logging in
	void LogIn()
	{
		// Will keep popping up until the user logs in
		if (!LoggedIn && !SUGARManager.Account.IsActive)
		{
			SUGARManager.Account.DisplayPanel(success =>
			{
				// Once the user has logged in get the account name and inform the system the player 
				// has logged in
				if (success)
				{
					PlayerID = SUGARManager.CurrentUser.Id;
					LoggedIn = true;

					// Get the quantity of each resource from the database
					GetResource("coins");
					GetResource("speed");
					foreach (GameObject up in Buttons)
					{
						GetResource(up.GetComponent<Upgrade>().Key);
					}

					// Particle effects and a transition as a welcome screen
					Lights.GetComponent<ParticleController>().Play();
					foreach (GameObject g in _background)
					{
						g.GetComponent<BackgroundController>().New(2);
					}
				}
			});

		}
	}

	// Whilst your waiting for other players this handles the sprites
	void WaitScreen()
	{
		// If you are the server, a client or are in a single player game and the game has not yet started
		if ((_networkController.Server || _networkController.Client || SinglePlayer) && StartCounter == -1)
		{
			// If you joined let open the store
			if (Selected == -1 && !IGS.activeSelf)
			{
				GameObject[] modes = GameObject.FindGameObjectsWithTag("Mode");
				foreach (GameObject m in modes)
				{
					if (m.GetComponent<ButtonController>().On)
					{
						IGS.SetActive(true);
						_storeLoaded = false;
					}
				}
			}
			// Initialise the store to match the game mode
			if (!_storeLoaded)
			{
				_storeLoaded = true;
				if (SinglePlayer)
				{
					IGS.GetComponent<InGameStore>().Change(0);
				}
				else if (_mode == 1)
				{
					IGS.GetComponent<InGameStore>().Change(2);
				}
				else
				{
					IGS.GetComponent<InGameStore>().Change(1);
				}
			}
			// If its a single player game instantiate a player object
			if (Players.Length == 0 && SinglePlayer)
			{
				GameObject.Instantiate(PlayerOffline);
			}
			if (SinglePlayer && Selected == -1)
			{
				IGS.SetActive(true);
			}
			if(!SinglePlayer)
			{
				Request.SetActive(true);
			}
			// Only need to get the local player once
			if(Player == null)
			{
				// Set _player to the local players controller
				foreach (GameObject p in Players)
				{
					if (p.GetComponent<PlayerController>().Client || p.GetComponent<PlayerController>().Server || SinglePlayer)
					{
						Player = p.GetComponent<PlayerController>();
						Player.ID = PlayerID;
					}
				}
			}
			// Display the countdown to start the game
			foreach (GameObject c in Countdown)
			{
				c.SetActive(true);
			}
			// If it is a multiplayer game display the amount of players in game
			if (!SinglePlayer)
			{
				foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(true);
				}
			}
			// If you are the games host display the title bar saying you can start the game
			if (_networkController.Server || SinglePlayer)
			{
				Tab.SetActive(true);
			}
			// Otherwise show the "waiting" title bar
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
		// Add all the player objects to an arraylist
		ArrayList rankings = new ArrayList();
		for (int i = 0; i < Players.Length; i++)
		{
			rankings.Add(Players[i]);
		}
		for (int j = 0; j < Players.Length; j++)
		{
			// Calculate the highest remaining score and assign that player to the variable best
			GameObject best = (GameObject)rankings[0];
			for (int k = 0; k < rankings.Count; k++)
			{
				if (((GameObject)rankings[k]).GetComponent<PlayerController>().Score > best.GetComponent<PlayerController>().Score)
				{
					best = Players[k];
				}
			}
			// Assign a place/ranking to each player from highest score to lowest
			best.GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Place = j;
			rankings.Remove(best);

		}

		for (int z = 0; z < Players.Length; z++)
		{
			GameObject healthBar = Players[z].GetComponent<PlayerController>().Hearts;
			// If the player is not in the game or the game hasn't started yet
			// hide their health bar
			if (!Players[z].GetComponent<PlayerController>().InGame)
			{
				healthBar.transform.localPosition = new Vector3(0.0f, 20.0f, 0.0f);
			}
			// Highest ranked player in game is display at the top followed by 2nd, 3rd etc
			else
			{
				healthBar.transform.localPosition = new Vector3(0.0f, -0.34f * healthBar.GetComponent<HeartController>().Place, 0.0f);
				healthBar.transform.localEulerAngles = Vector3.zero;
			}
		}
	}

	// Chooses between CO-OP and Versus
	void GameType()
	{
		// When one button is pressed highlight it and deselect the other
		if(_gameMode.Length > 1 && !Chosen)
		{
			ButtonController one = _gameMode[0].GetComponent<ButtonController>();
			ButtonController two = _gameMode[1].GetComponent<ButtonController>();
			if (_networkController.Server)
			{
				// When you choose a game mode lock it in and display the store for everyone
				if (one.Pressed)
				{
					_networkController.RpcStore();
					_storeLoaded = false;
					one.Pressed = false;
					Chosen = true;
					one.On = true;
				}
				if (two.Pressed)
				{
					_networkController.RpcStore();
					_storeLoaded = false;
					two.Pressed = false;
					Chosen = true;
					two.On = true;
				}
			}
			// Select the game mode depending on what was chosen
			if(one.On)
			{
				_mode = 1;
			}
			else
			{
				_mode = 2;
			}
		}
		// Single player
		else
		{
			_mode = 2;
		}
	}

	//The countdown between when TAB is pressed and when the game actually starts
	void StartCounting()
	{
		// StartCounter is -1 until Tab is pressed and after the countdown it is 0
		if (StartCounter > 0)
		{
			// Countdown has just started
			if (StartCounter == 61)
			{
				IGS.SetActive(false);
				Request.SetActive(false);
				// If its a co-op or single player game turn on the big score counter
				if (_mode == 2)
				{
					SharedScore.SetActive(true);
					if(!SinglePlayer)
					{
						SharedScore.GetComponent<PlaceController>().RandomColour = true;
					}
				}
				// Hide all the sprites and title bars
				foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(false);
				}
				Wait.SetActive(false);
				Host.SetActive(false);
				Tab.SetActive(false);
				foreach (GameObject m in _gameMode)
				{
					m.SetActive(false);
				}
				_running = true;
				// Start the clock
				_time = Time.time;
				// If the user is the server tell all the clients to spawn 5 dodgeballs
				// will also affect itself if the user is also a client
				if (_networkController.Server)
				{
					_networkController.RpcDodge(true);
				}
				// If it is a single player game spawn 5 dodgeballs
				if (SinglePlayer)
				{
					GetComponent<SingleController>().Dodge(true);
				}
			}
			// Countdown to 0 is displayed on the screen
			else if (StartCounter == 41)
			{
				Countdown[2].SetActive(false);
			}
			else if (StartCounter == 21)
			{
				Countdown[1].SetActive(false);
			}
			if (StartCounter == 1)
			{
				Started = true;
				Countdown[0].SetActive(false);
				// If youre the host spawn the points and block zones
				if (_networkController.Server || SinglePlayer)
				{
					foreach (GameObject z in Zones)
					{
						GameObject currentZone = GameObject.Instantiate(z);
						// Spawn on the server if its an online game
						if (_networkController.Server)
						{
							NetworkServer.Spawn(currentZone);
						}
					}
				}
			}
			StartCounter--;
		}
	}

	// Handles pop ups
	public void PopUps(int i)
	{
		// Friends list
		if (i == 0)
		{
			SUGARManager.UserFriend.Display();
		}
		// Groups list
		else if (i == 1)
		{
			SUGARManager.UserGroup.Display();
		}
		// Achievements list
		else if (i == 2)
		{
			SUGARManager.Evaluation.DisplayAchievementList();
		}
		// time leaderboards
		else if (i == 3)
		{
			SUGARManager.Leaderboard.Display("longest_time", PlayGen.SUGAR.Common.LeaderboardFilterType.Friends);
		}
		else if (i == 5)
		{
			SUGARManager.Leaderboard.Display("longest_time", PlayGen.SUGAR.Common.LeaderboardFilterType.Top);
		}
		else if (i == 6)
		{
			SUGARManager.Leaderboard.Display("longest_time", PlayGen.SUGAR.Common.LeaderboardFilterType.Near);
		}
		// score leaderboards
		else if (i == 7)
		{
			SUGARManager.Leaderboard.Display("highest_score", PlayGen.SUGAR.Common.LeaderboardFilterType.Friends);
		}
		else if (i == 9)
		{
			SUGARManager.Leaderboard.Display("highest_score", PlayGen.SUGAR.Common.LeaderboardFilterType.Top);
		}
		else if (i == 10)
		{
			SUGARManager.Leaderboard.Display("highest_score", PlayGen.SUGAR.Common.LeaderboardFilterType.Near);
		}
	}

	//Reset between games
	public void GameOver()
	{
		// Reset the dodgeball generators
		foreach (GameObject DBG in DBGS)
		{
			DBG.GetComponent<DBGenerator>().Counter = 0;
			DBG.GetComponent<DBGenerator>().InGame = false;
		}
		Selected = -1;
		Chosen = false;
		if(_gameMode.Length > 0)
		{
			_gameMode[0].GetComponent<ButtonController>().On = false;
			_gameMode[1].GetComponent<ButtonController>().On = false;
		}
		_storeLoaded = false;
		// Show lobby sprites and reset variables
		if (!SinglePlayer)
		{
			Request.SetActive(true);
		}
		StartCounter = -1;
		_running = false;
		Started = false;
		Countdown[0].SetActive(true);
		Countdown[1].SetActive(true);
		Countdown[2].SetActive(true);
		if (SinglePlayer)
		{
			Tab.SetActive(true);
		}
		else
		{
			foreach (GameObject m in _gameMode)
			{
				m.SetActive(true);
			}
			foreach (Transform child in Dots.transform)
			{
				child.gameObject.SetActive(true);
			}
			if (_networkController.Server)
			{
				Tab.SetActive(true);
			}
			else
			{
				Wait.SetActive(true);
				Host.SetActive(true);
			}
		}
		// Hide the score menu
		SharedScore.SetActive(false);
		// Destroy all the dodgeballs
		GameObject[] balls = GameObject.FindGameObjectsWithTag("DodgeBall");
		foreach (GameObject b in balls)
		{
			Destroy(b);
		}
		// Set all players scores to 0
		foreach (GameObject p in Players)
		{
			p.GetComponent<PlayerController>().ScoreDisplay.GetComponent<PlaceController>().Score = 0;
		}
		// Reset the score menu
		SharedScore.GetComponent<PlaceController>().Reset();
		SharedScore.SetActive(false);

		// Clear the end game screen
		GetComponent<WinnerScreen>().Clear();
	}

	//Clear data if not connected to a server or when you quit the game
	public void Clear()
	{
		Selected = -1;
		Chosen = false;
		_storeLoaded = false;
		// Reset the game mode sprites
		_gameMode = new GameObject[0];
		// Reset the score menu
		SharedScore.GetComponent<PlaceController>().Reset();
		SharedScore.SetActive(false);

		// Hide all the sprites
		IGS.SetActive(false);
		Request.SetActive(false);
		Countdown[0].SetActive(false);
		Countdown[1].SetActive(false);
		Countdown[2].SetActive(false);
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
		// destroy the mode selectors. They will automatically delete themselves anyway but there is a delay
		GameObject[] modes = GameObject.FindGameObjectsWithTag("Mode");
		foreach (GameObject m in modes)
		{
			Destroy(m);
		}
		// Reset the dodgeball generators
		foreach (GameObject DBG in DBGS)
		{
			DBG.GetComponent<DBGenerator>().Counter = 0;
			DBG.GetComponent<DBGenerator>().InGame = false;
		}

		// Destroy all the dodgeballs
		foreach (GameObject b in GameObject.FindGameObjectsWithTag("DodgeBall"))
		{
			Destroy(b);
		}
		// Destroy all the players in a single player game. Automatically done
		// in online games
		if (SinglePlayer)
		{
			foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
			{
				Destroy(p);
			}
		}

		// Reset variables
		SinglePlayer = false;
		_running = false;
		Started = false;
		StartCounter = -1;

		// Go to the home page
		GetComponent<TitleScreenController>().Home();

		// Reset the network controller
		if (_networkController.Client && !_networkController.Server)
		{
			_networkController.Client = false;
			_networkController.Server = false;
			_networkController.Players = 0;
			_networkManager.StopClient();
		}

		// If you are the server host tell all the clients to disconnect
		if (_networkController.Server)
		{
			NetController.GetComponent<NetworkController>().RpcDisconnect(GameObject.FindGameObjectsWithTag("Player").Length);
		}

		// Hide all end screen sprites
		GetComponent<WinnerScreen>().Clear();
	}
		
	//Runs only whilst the game is going on
	void Run()
	{
		// Get the current time
		float time = Time.time;
		// Get the time in milliseconds (Used for adding score to the player)
		int milliseconds = ((int)((time - _time) * 100));
		// Time rounded to 1 d.p
		time = ((float)((int)(10.0f * (time - _time))) / 10.0f);
		// Spawn a new ball every 5 seconds up to 100 seconds
		if (_oldTime != time && time % 5.0f == 0.0f && time <= 100.0f && time != 0.0f)
		{
			// If server tell all clients to spawn a ball
			if(_networkController.Server)
			{
				_networkController.RpcDodge(false);
			}
			// If single player spawn a ball
			else
			{
				GetComponent<SingleController>().Dodge(false);
			}
		}
		// Unless at least one player has more than 0 health game over will stay true
		bool gameOver = true;
		for (int k = 0; k < Players.Length; k++)
		{
			PlayerController pCont = Players[k].GetComponent<PlayerController>();
			pCont.Mode = _mode;
			if (pCont.Health != 0)
			{
				// Set game over galse if a player has health
				gameOver = false;
				// If the game has started
				if (milliseconds > 2)
				{
					// If the player is still in the game keep incrementing its time
					pCont.Time = time;
					// increment players score for still being alive
					if (_networkController.Server)
					{
						pCont.RpcScores(milliseconds - _timeDiff);
					}
					else if (SinglePlayer)
					{
						SharedScore.GetComponent<PlaceController>().Score += milliseconds - _timeDiff;
					}
				}
			}
		}
		// Send the game data to the database
		if (Player.State == 1)
		{
			// Make sure there is a valid user connected
			if (SUGARManager.CurrentUser != null)
			{
				long score;
				// In versus send the individual users score
				if (_mode == 1)
				{
					// Get local players score
					score = Player.ScoreDisplay.GetComponent<PlaceController>().Score;
					SUGARManager.GameData.Send("score", score);
				}
				// In single player send the individual users score
				else if (SinglePlayer)
				{
					score = (long)((int)SharedScore.GetComponent<PlaceController>().Score);
					SUGARManager.GameData.Send("score", score);
				}
				// In coop send the shared score
				else
				{
					score = (long) ((int) SharedScore.GetComponent<PlaceController>().Score);
					SUGARManager.GameData.Send("scoreCo", score);
					score = score / Players.Length;
				}
				// Coin cap of 9999999
				if (score + Requested["coins"] > 9999999)
				{
					score = 9999999 - (long) Requested["coins"];
				}
				// Add the players score to their coin amount
				SUGARManager.Resource.Add("coins", score, success =>
				{
					GetResource("coins");
					Debug.Log(success);
				});
				// Send the players time as game data for the leaderboards and achievements
				SUGARManager.GameData.Send("time", time);
				// Send the players score as game data for the leaderboards and achievements
				if (_mode == 2)
				{
				}
				else
				{
					SUGARManager.GameData.Send("scoreCo", score);
				}
			}
			// Change the players state so they don't send game data multiple times
			Player.State = 2;
		}
		// Once the game is over display the end game screen (with the scores)
		// until the counter ends
		if(gameOver)
		{
			if (_networkController.Server || SinglePlayer)
			{
				foreach (GameObject g in GameObject.FindGameObjectsWithTag("Zone"))
				{
					Destroy(g);
				}
			}
			// Offset is 4 for coop and single player and 0 for vs
			GetComponent<WinnerScreen>().Offset = (_mode - 1) * 4;
			GetComponent<WinnerScreen>().Run(Players);
			if(GameOverCounter == 0)
			{
				GameOverCounter = 270;
				Lights.GetComponent<ParticleController>().Play();
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
		}
		// Once the game has finished and the end screen has been displayed for enough time return to the lobby
		if (gameOver && GameOverCounter == 0)
		{
			if (_networkController.Server)
			{
				_networkController.RpcResets();
			}
			else if (SinglePlayer)
			{
				GetComponent<SingleController>().Resets();
			}
			GameOver();
		}
		// If the game hasn't started set the start time to the current time
		if (!Started)
		{
			_time = Time.time;
		}

		// Store the old time values
		_timeDiff = milliseconds;
		_oldTime = time;
	}

	// Get the value of a resource in the database
	public void GetResource(string key)
	{
		// Make sure there is a valid user
		if (SUGARManager.CurrentUser != null)
		{
			// Get resource API call to the database
			SUGARManager.Resource.Get(success =>
			{
				// If you already have a value that matches the key remove it
				if (Requested.ContainsKey(key))
				{
					Requested.Remove(key);
				}
				// Add the (key, value) tuple to the dictionary which stores the get requests 
				// so you only have to request the data once
				if (success.Count > 0)
				{
					Requested.Add(key, success[0].Quantity);
				}
				// If the database contains no information for that key set the quantity to 0
				else
				{
					Requested.Add(key, 0);
				}
			}, new string[] { key });
		}
	}

	// Add the value of a resource in the database
	public void AddResource(string key, float value)
	{
		// Make sure there is a valid user
		if (SUGARManager.CurrentUser != null)
		{
			// Add resource API call to the database
			SUGARManager.Resource.Add(key, (long) value, success =>
			{
				Debug.Log(success);
			});
		}
	}

	// Add the value of a resource in the database
	public void TransferResource(int id, string key, float value)
	{
		// Make sure there is a valid user
		if (SUGARManager.CurrentUser != null)
		{
			// Add resource API call to the database
			SUGARManager.Resource.Transfer(id, key, (long)value, success =>
			{
				Debug.Log(success);
			});
		}
	}

	// Tell the database you used an ability
	public void UsedAbility()
	{
		SUGARManager.GameData.Send("ability", 1);
	}
}

