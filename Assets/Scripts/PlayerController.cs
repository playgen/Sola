using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using PlayGen.SUGAR.Unity;

public class PlayerController : NetworkBehaviour
{
	public float Speed;
	public GameObject Hearts, Controller;
	public bool Server, Client, InGame, Damage;
	public string Name;
	public GameObject[] Stripes = new GameObject[2];
	public Material[] Materials = new Material[5];
	public int Invincibility, Health, State;
	
	private int _blinkCounter;
	private float _x,  _y, _z;
	private bool _connected;
	private Vector3 _initialPosition;

	// Use this for initialization
	void Start ()
	{
		//Set Initial values
		Hearts.transform.parent = GameObject.FindGameObjectWithTag("Rankings").transform;
		Hearts.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		transform.parent = GameObject.FindGameObjectWithTag("Scale").transform;
		Controller = GameObject.FindGameObjectWithTag("Controller");

		_initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		_blinkCounter = 0;
		_connected = false;

		Damage = false;
		InGame = false;
		Server = false;
		Client = false;
		State = 0;
		Name = "";
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		Health = Hearts.GetComponent<HeartController>().Health;
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z - 2.0f);
		transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		Invinc();
		OutOfTheWay();

		if (!isLocalPlayer && !Controller.GetComponent<Controller>().SinglePlayer)
		{
			return;
		}

		Server = isServer;

		if(isClient && !_connected)
		{
			CmdAdd();
		}

		_connected = isClient;
		Client = isClient && !Server;
		Controller.GetComponent<Controller>().Client = Client;

		Blink();
		Move();
	}

	//When you press B blink
	void Blink ()
	{
		if (_blinkCounter > 0)
		{
			_blinkCounter--;
		}
		if (Input.GetKey(KeyCode.B) && _blinkCounter == 0)
		{
			_blinkCounter = 30;
			Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			float angle;
			if (mouse.x - transform.position.x >= 0.0f)
			{
				angle = 90 - Mathf.Atan((mouse.y - transform.position.y) / (mouse.x - transform.position.x)) * 180.0f / Mathf.PI;
			}
			else
			{
				angle = 270 - Mathf.Atan((mouse.y - transform.position.y) / (mouse.x - transform.position.x)) * 180.0f / Mathf.PI;
			}
			angle = angle % 90;
			float x = Mathf.Sin(angle / 180.0f * Mathf.PI);
			float y = Mathf.Cos(angle / 180.0f * Mathf.PI);
			if (mouse.x - transform.position.x >= 0.0f && mouse.y - transform.position.y >= 0.0f)
			{
				transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
			}
			else if (mouse.x - transform.position.x < 0.0f && mouse.y - transform.position.y < 0.0f)
			{
				transform.localPosition = new Vector3(transform.localPosition.x - x, transform.localPosition.y - y, transform.localPosition.z);
			}
			else if (mouse.x - transform.position.x < 0.0f && mouse.y - transform.position.y >= 0.0f)
			{
				transform.localPosition = new Vector3(transform.localPosition.x - y, transform.localPosition.y + x, transform.localPosition.z);
			}
			else
			{
				transform.localPosition = new Vector3(transform.localPosition.x + y, transform.localPosition.y - x, transform.localPosition.z);
			}
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

	//Set Players Colour
	public void SetColour()
	{
		Stripes[0].GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
		Stripes[1].GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
		Hearts.GetComponent<HeartController>().Marker.GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
	}

	//Send Players Name to the Server
	public void SendName()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		CmdName(Controller.GetComponent<Controller>().PlayerName);
		//Debug.Log(_controller.GetComponent<Controller>().PlayerName);
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
		if (Invincibility > 0)
		{
			if (Invincibility % 10 > 5)
			{
				Stripes[0].GetComponent<Renderer>().material = Materials[4];
				Stripes[1].GetComponent<Renderer>().material = Materials[4];
				GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
			}
			else
			{
				Stripes[0].GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
				Stripes[1].GetComponent<Renderer>().material = Materials[Hearts.GetComponent<HeartController>().Place];
				GetComponent<Renderer>().material = Materials[4];
			}
			Invincibility--;
		}
	}

	//Move controller
	void Move()
	{
		transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		_x = transform.GetComponent<Rigidbody>().velocity.x;
		_y = transform.GetComponent<Rigidbody>().velocity.y;
		_z = transform.GetComponent<Rigidbody>().velocity.z;
		if (Controller.GetComponent<Controller>().Started)
		{
			if (Input.GetKey(KeyCode.W))
			{
				//SUG
				_y += Speed / 10;

			}
			if (Input.GetKey(KeyCode.S))
			{
				_y -= Speed / 10;
			}
			if (Input.GetKey(KeyCode.A))
			{
				_x -= Speed / 10;
			}
			if (Input.GetKey(KeyCode.D))
			{
				_x += Speed / 10;
			}
		}
		transform.GetComponent<Rigidbody>().velocity = new Vector3(_x, _y, _z);
	}

	//If this player isn't in this turn move them off screen
	void OutOfTheWay()
	{
		if (Health == 0 && State == 0)
		{
			transform.localPosition = new Vector3(400.0f, 0.0f, 0.0f);
			State = 1;
		}
		if (Controller.GetComponent<Controller>().Started == true && !InGame && State == 1)
		{
			transform.localPosition = new Vector3(30.0f, 0.0f, 0.0f);
			Hearts.GetComponent<HeartController>().Health = 0;
			State = 2;
		}
	}

	//When hit by a dodgeball turn Damage to true
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			Damage = true;
		}
	}

	[Command]
	public void CmdAdd()
	{
		GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>().Players++;
	}

	[Command]
	void CmdName(string playerName)
	{
		Name = playerName;


		//GameObject dodge = (GameObject)Instantiate(Dodgeball);
		//NetworkServer.Spawn(dodge);
	}
	
}
