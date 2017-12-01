using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TitleScreenController : MonoBehaviour {

	public GameObject Store, Shop, Single, Online, Controls, Host, Join, Back, NetworkController, Connecting, PTC, BackTL, Guide, Lights;
	public Material[] Materials;
	public bool SinglePlayer;

	GameObject[] _background;
	GameObject _networkManager;
	bool _started, _stop;

	// Use this for initialization
	void Start () {
		_networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
		_background = GameObject.FindGameObjectsWithTag("Background");
		_started = false;
		_stop = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if(Store.activeSelf && !_stop)
		{
			Lights.GetComponent<ParticleController>().Run();
			_stop = true;
		}
		if (!_started && GetComponent<Controller>().LoggedIn)
		{
			_started = true;
			Store.SetActive(true);
			Single.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
		}
		if (Store.GetComponent<ButtonController>().Pressed == true)
		{
			Store.transform.GetComponent<Renderer>().material = Materials[0];
			Store.GetComponent<ButtonController>().Pressed = false;
			Store.SetActive(false);
			Single.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Shop.SetActive(true);
			BackTL.SetActive(true);
			Transition(2);
		}
		if (Online.GetComponent<ButtonController>().Pressed == true)
		{
			Online.transform.GetComponent<Renderer>().material = Materials[0];
			Host.transform.GetComponent<Renderer>().material = Materials[0];
			Online.GetComponent<ButtonController>().Pressed = false;
			Host.SetActive(true);
			Join.SetActive(true);
			Back.SetActive(true);
			Store.SetActive(false);
			Single.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Transition(2);
		}
		if (Back.GetComponent<ButtonController>().Pressed == true)
		{
			Back.transform.GetComponent<Renderer>().material = Materials[0];
			Back.GetComponent<ButtonController>().Pressed = false;
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			Store.SetActive(true);
			Single.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
			Transition(1);
		}
		if (Host.GetComponent<ButtonController>().Pressed == true)
		{
			Host.transform.GetComponent<Renderer>().material = Materials[0];
			Lights.GetComponent<ParticleController>().Stop();
			_stop = false;
			Host.GetComponent<ButtonController>().Pressed = false;
			_networkManager.GetComponent<NetworkManager>().StartHost();
			NetworkServer.Spawn(NetworkController);
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
		}
		if (Join.GetComponent<ButtonController>().Pressed == true)
		{
			Join.transform.GetComponent<Renderer>().material = Materials[0];
			Lights.GetComponent<ParticleController>().Stop();
			_stop = false;
			_networkManager.GetComponent<NetworkManager>().StartClient();
			PTC.SetActive(true);
			Connecting.SetActive(true);
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			Join.GetComponent<ButtonController>().Pressed = false;
		}
		if (Single.GetComponent<ButtonController>().Pressed == true)
		{
			Single.transform.GetComponent<Renderer>().material = Materials[0];
			Lights.GetComponent<ParticleController>().Stop();
			_stop = false;
			Single.GetComponent<ButtonController>().Pressed = false;
			SinglePlayer = true;
			Store.SetActive(false);
			Single.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Transition(2);
		}
		if (Controls.GetComponent<ButtonController>().Pressed == true)
		{
			Controls.transform.GetComponent<Renderer>().material = Materials[0];
			Controls.GetComponent<ButtonController>().Pressed = false;
			Controls.SetActive(false);
			Guide.SetActive(true);
			Store.SetActive(false);
			Single.SetActive(false);
			Online.SetActive(false);
			BackTL.SetActive(true);
			Transition(2);
		}
		if (BackTL.GetComponent<ButtonController>().Pressed == true)
		{
			BackTL.GetComponent<ButtonController>().Pressed = false;
			BackTL.SetActive(false);
			Shop.SetActive(false);
			Guide.SetActive(false);
			Store.SetActive(true);
			Single.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
			Transition(1);
		}
		if (Input.GetKeyDown(KeyCode.Escape) && Connecting.activeSelf)
		{
			_networkManager.GetComponent<NetworkManager>().StopClient();
			PTC.SetActive(false);
			Connecting.SetActive(false);
			Transition(1);
		}
		if (NetworkController.GetComponent<NetworkController>().Server || NetworkController.GetComponent<NetworkController>().Client)
		{
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			PTC.SetActive(false);
			Connecting.SetActive(false);
		}
	}

	public void Home()
	{
		Store.SetActive(true);
		Single.SetActive(true);
		Online.SetActive(true);
		Controls.SetActive(true);
		Shop.SetActive(false);
		BackTL.SetActive(false);
		Guide.SetActive(false);
	}

	public void Transition(int i)
	{
		foreach (GameObject g in _background)
		{
			g.GetComponent<BackgroundController>().New(i - 1);
		}
	}
}
