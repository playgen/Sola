using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using PlayGen.SUGAR.Unity;
using UnityEngine;

public class Controller : NetworkBehaviour
{
	public GameObject One, Two, Three, Wait, Host, Tab, Dots;
	public int StartCounter;
	public bool Started;

	NetworkManager _networkManager;
	GameObject _networkManagerGO, _background, _player;
	GameObject[] _players;
	float _materialCounter;
	bool _waiting;

	// Use this for initialization
	void Start ()
	{
		_background = GameObject.FindGameObjectWithTag("Background");
		_networkManagerGO = GameObject.FindGameObjectWithTag("NetworkManager");
		_networkManager = _networkManagerGO.GetComponent<NetworkManager>();
		StartCounter = -1;
		Started = false;
		
		SUGARManager.Account.DisplayPanel(success =>
		{
			_networkManagerGO.GetComponent<NetworkManagerHUD>().enabled = true;
		});
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		_players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject p in _players)
		{
			if ((p.GetComponent<PlayerController>().Server || p.GetComponent<PlayerController>().Client) && StartCounter == -1)
			{
				_player = p;
				One.SetActive(true);
				Two.SetActive(true);
				Three.SetActive(true);
				foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(true);
				}
				if (p.GetComponent<PlayerController>().Client)
				{
					Wait.SetActive(true);
					Host.SetActive(true);
				}
				else
				{
					Tab.SetActive(true);
				}
			}
		}


		if (StartCounter > 0)
		{
			if(StartCounter == 60)
			{
				foreach (GameObject p in _players)
				{
					GameObject hearts = p.GetComponent<PlayerController>().Hearts;
					hearts.transform.parent = GameObject.FindGameObjectWithTag("Rankings").transform;
					hearts.transform.localPosition = Vector3.zero;
				}
					foreach (Transform child in Dots.transform)
				{
					child.gameObject.SetActive(false);
				}
				Wait.SetActive(false);
				Host.SetActive(false);
				Tab.SetActive(false);
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

		_background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, _materialCounter));
		_materialCounter += 0.001f;
	}
	
}
