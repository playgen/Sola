using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//This is the class for handling changing between different screens. Just sets the correct stuff active and everything else disabled
public class TitleScreenController : MonoBehaviour {

	public GameObject Store, Shop, Single, Online, Controls, Host, Join, Back, NetworkController, Connecting, PTC, BackTL, Guide, Lights, Title;
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
		// Called when you quit a game you're in
		if(Store.activeSelf && !_stop)
		{
			Lights.GetComponent<ParticleController>().Play();
			_stop = true;
		}
		// Called after log in
		if (!_started && GetComponent<Controller>().LoggedIn)
		{
			_started = true;
			Store.SetActive(true);
			Single.SetActive(true);
			Title.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
		}
		// Called when you enter the store
		if (Store.GetComponent<ButtonController>().Pressed == true)
		{
			Store.transform.GetComponent<Renderer>().material = Materials[0];
			Store.GetComponent<ButtonController>().Pressed = false;
			Store.SetActive(false);
			Single.SetActive(false);
			Title.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Shop.SetActive(true);
			BackTL.SetActive(true);
			Transition(2);
		}
		// Called when you select the online button
		if (Online.GetComponent<ButtonController>().Pressed == true)
		{
			Online.transform.GetComponent<Renderer>().material = Materials[0];
			Host.transform.GetComponent<Renderer>().material = Materials[0];
			Online.GetComponent<ButtonController>().Pressed = false;
			Host.SetActive(true);
			Join.SetActive(true);
			Back.SetActive(true);
			Store.SetActive(false);
			Title.SetActive(false);
			Single.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Transition(2);
		}
		// Called when you select the back button
		if (Back.GetComponent<ButtonController>().Pressed == true)
		{
			Back.transform.GetComponent<Renderer>().material = Materials[0];
			Back.GetComponent<ButtonController>().Pressed = false;
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			Store.SetActive(true);
			Single.SetActive(true);
			Title.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
			Transition(1);
		}
		// Called when you request to be a host
		if (Host.GetComponent<ButtonController>().Pressed == true)
		{
			Host.transform.GetComponent<Renderer>().material = Materials[0];
			Lights.GetComponent<ParticleController>().Stop();
			Host.GetComponent<ButtonController>().Pressed = false;
			// Respawns the networkcontroller on the server so you can pass messages to clients
			_networkManager.GetComponent<NetworkManager>().StartHost();
			NetworkServer.Spawn(NetworkController);
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			_stop = false;
		}
		// Called when you ask to join a game
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
		// Called when you ask to play a single player game
		if (Single.GetComponent<ButtonController>().Pressed == true)
		{
			Single.transform.GetComponent<Renderer>().material = Materials[0];
			Lights.GetComponent<ParticleController>().Stop();
			_stop = false;
			Single.GetComponent<ButtonController>().Pressed = false;
			SinglePlayer = true;
			Store.SetActive(false);
			Single.SetActive(false);
			Title.SetActive(false);
			Online.SetActive(false);
			Controls.SetActive(false);
			Transition(2);
		}
		// Called when you go to the controls menu
		if (Controls.GetComponent<ButtonController>().Pressed == true)
		{
			Controls.transform.GetComponent<Renderer>().material = Materials[0];
			Controls.GetComponent<ButtonController>().Pressed = false;
			Controls.SetActive(false);
			Guide.SetActive(true);
			Store.SetActive(false);
			Single.SetActive(false);
			Title.SetActive(false);
			Online.SetActive(false);
			BackTL.SetActive(true);
			Transition(2);
		}
		// Called when you press the back button in the store or controls menu
		if (BackTL.GetComponent<ButtonController>().Pressed == true)
		{
			BackTL.GetComponent<ButtonController>().Pressed = false;
			BackTL.SetActive(false);
			Shop.SetActive(false);
			Guide.SetActive(false);
			Store.SetActive(true);
			Single.SetActive(true);
			Title.SetActive(true);
			Online.SetActive(true);
			Controls.SetActive(true);
			Transition(1);
		}
		// If you're trying to join a server but cancel
		if (Input.GetKeyDown(KeyCode.Escape) && Connecting.activeSelf)
		{
			_networkManager.GetComponent<NetworkManager>().StopClient();
			PTC.SetActive(false);
			Connecting.SetActive(false);
			Transition(1);
		}
		// When you actually connecct to the server hide all the extra buttons
		if (NetworkController.GetComponent<NetworkController>().Server || NetworkController.GetComponent<NetworkController>().Client)
		{
			Host.SetActive(false);
			Join.SetActive(false);
			Back.SetActive(false);
			PTC.SetActive(false);
			Connecting.SetActive(false);
		}
	}

	// When another function requests to go to the home page set the buttons to the correct value
	public void Home()
	{
		Store.SetActive(true);
		Single.SetActive(true);
		Title.SetActive(true);
		Online.SetActive(true);
		Controls.SetActive(true);
		Shop.SetActive(false);
		BackTL.SetActive(false);
		Guide.SetActive(false);
	}

	// Request a background transition
	public void Transition(int i)
	{
		foreach (GameObject g in _background)
		{
			g.GetComponent<BackgroundController>().New(i - 1);
		}
	}
}
