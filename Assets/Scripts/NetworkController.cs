using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkBehaviour {

	public GameObject NetworkManager, DBGenerator, Controller, Dodgeball;
	public GameObject[] Dots = new GameObject[4];
	public Sprite[] Sprites = new Sprite[2];
	public int Players;

	NetworkManager _networkManager;
	GameObject[] _updatedBalls;
	float[] _updates;
	int _length;

	// Use this for initialization
	void Start () {
		Players = 0;
		_updatedBalls = DBGenerator.GetComponent<DBGenerator>().BallUpdates;
		_networkManager = NetworkManager.GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		for (int i = 0; i < Dots.Length; i++)
		{
			if(Players > i)
			{
				Dots[i].GetComponent<SpriteRenderer>().sprite = Sprites[0];
			}
			else
			{
				Dots[i].GetComponent<SpriteRenderer>().sprite = Sprites[1];
			}
		}
		if (isServer)
		{
			DBGenerator.GetComponent<DBGenerator>().Server = true;
			int x = _networkManager.numPlayers;
			bool start = false;
			if (Input.GetKey(KeyCode.Tab) && Controller.GetComponent<Controller>().StartCounter == -1)
			{
				start = true;
			}
			if (Input.GetKeyDown(KeyCode.Y))
			{
				RpcDodge();
			}
			CreateArray();
			RpcUpdate(x, start, _updates, _length);
		}


	}

	/*
		[Command]
		void CmdDodge()
		{
			GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().pulse = true;
			GameObject dodge = (GameObject)Instantiate(Dodgeball);
			NetworkServer.Spawn(dodge);
		}
	*/

	[ClientRpc]
	public void RpcDodge()
	{
		GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().Pulse = true;
	}

	[ClientRpc]
	public void RpcUpdate(int x, bool start, float[] dodgeballs, int length)
	{
		GameObject.FindGameObjectWithTag("DBGenerator").GetComponent<DBGenerator>().UpdateNumbers = dodgeballs;
		Players = x;
		if(start)
		{
			Controller.GetComponent<Controller>().StartCounter = 60;
		}
	}

	public void CreateArray()
	{
		_length = 0;
		for(int j = 0; j < _updatedBalls.Length; j++)
		{
			if (_updatedBalls[j] != null)
			{
				_length++;
			}
		}
		_updates = new float[_length * 10];
		int q = 0;
		for(int i = 0; i < _length; i = i + 10)
		{
			if(_updatedBalls[q] != null)
			{
				_updates[i + 0] = (float) q;
				_updates[i + 1] = _updatedBalls[q].GetComponentInChildren<DodgeController>().MaterialNum;
				_updates[i + 2] = _updatedBalls[q].GetComponentInChildren<DodgeController>().ArrowNum;
				_updates[i + 3] = _updatedBalls[q].transform.localEulerAngles.z;
				_updates[i + 4] = _updatedBalls[q].transform.localPosition.x;
				_updates[i + 5] = _updatedBalls[q].transform.localPosition.y;
				_updates[i + 6] = _updatedBalls[q].GetComponentInChildren<DodgeController>().transform.localScale.y;
				_updates[i + 7] = _updatedBalls[q].GetComponentInChildren<DodgeController>().Min;
				_updates[i + 8] = _updatedBalls[q].GetComponentInChildren<DodgeController>().Max;
				_updates[i + 9] = _updatedBalls[q].GetComponentInChildren<DodgeController>().State;
				_updatedBalls[q] = null;
			}
			else
			{
				i--;
			}
			q++;
		}
	}
}
