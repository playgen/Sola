using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	public float Speed;
	public GameObject Hearts;
	public bool Server, Client;

	private GameObject _controller;
	private int _blinkCounter;
	private float _x,  _y, _z;
	private bool _connected;

	// Use this for initialization
	void Start ()
	{
		_controller = GameObject.FindGameObjectWithTag("Controller");
		_blinkCounter = 0;
		_connected = false;

		Server = false;
		Client = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		Server = isServer;
		if(isClient && !_connected)
		{
			CmdAdd();
		}
		if (Server)
		{
			RpcUpdate();
		}
			_connected = isClient;
		Client = isClient && !Server;

		Blink();
		transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		_x = transform.GetComponent<Rigidbody>().velocity.x;
		_y = transform.GetComponent<Rigidbody>().velocity.y;
		_z = transform.GetComponent<Rigidbody>().velocity.z;
		if (_controller.GetComponent<Controller>().Started)
		{
			if (Input.GetKey(KeyCode.W))
			{
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

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			Hearts.GetComponent<HeartController>().Health--;
		}
	}

	[Command]
	public void CmdAdd()
	{
		GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>().Players++;
	}

	[ClientRpc]
	public void RpcUpdate()
	{
		//GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().Speed = 80;
		//Debug.Log(5);
	}
}
