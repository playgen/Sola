using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using PlayGen.SUGAR.Unity;

public class PlayerController : NetworkBehaviour
{
	public float Speed, Time;
	public GameObject Hearts, Controller, Spin, Shield, Bomb, Bullet;
	public bool Server, Client, InGame, Damage;
	public string Name;
	public GameObject[] Stripes = new GameObject[2];
	public Material[] Materials = new Material[5];
	public int Invincibility, Health, State, CounterTime;

	private int _abilityCounter, _spinCounter;
	private float _x, _y, _z;
	private bool _connected;
	private Vector3 _initialPosition;
	private Transform[] _ability;

	// Use this for initialization
	void Start()
	{
		//Set Initial values
		Controller = GameObject.FindGameObjectWithTag("Controller");
		Hearts.transform.parent = GameObject.FindGameObjectWithTag("Rankings").transform;
		Speed += Controller.GetComponent<Controller>().Requested["speed"];

		_initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		_ability = Controller.GetComponent<Controller>().Ability.GetComponentsInChildren<Transform>();
		_abilityCounter = 0;
		_spinCounter = -1;
		_connected = false;

		Damage = false;
		InGame = false;
		Server = false;
		Client = false;
		State = 0;
		Name = "";
		Time = 99999;
	}

	// Update is called once per frame
	void FixedUpdate()
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

		if (isClient && !_connected)
		{
			CmdAdd();
		}

		_connected = isClient;
		Client = isClient && !Server;
		Controller.GetComponent<Controller>().Client = Client;

		if (Input.GetKey(KeyCode.E) && _abilityCounter == 0 && Controller.GetComponent<Controller>().Requested["selected"] != 0 && Controller.GetComponent<Controller>().Started)
		{
			if(isClient)
			{
				CmdAbility(Controller.GetComponent<Controller>().Requested["selected"], Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
			}
			else
			{
				Ability(Controller.GetComponent<Controller>().Requested["selected"], Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
			}
		}
		if(_spinCounter == 0)
		{
			Spin.SetActive(false);
			_abilityCounter = CounterTime;
		}
		if (_spinCounter >= 0)
		{
			_spinCounter--;
		}
		if (_abilityCounter > 0)
		{
			_abilityCounter--;
			int abilityCalVal = (_abilityCounter / (CounterTime / 8));
			if (abilityCalVal < 7)
			{
				_ability[abilityCalVal].GetComponent<SpriteRenderer>().enabled = false;
			}
		}
		Move();
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
		if (Controller.GetComponent<Controller>().Started && Controller.GetComponent<Controller>().GameOverCounter == 0)
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
			if (_x != 0 && _y != 0)
			{
				_x = _x / Mathf.Abs(_x);
				_y = _y / Mathf.Abs(_y);
				_x = (Speed / 10) * Mathf.Cos(45.0f * Mathf.PI / 180.0f) * _x;
				_y = (Speed / 10) * Mathf.Cos(45.0f * Mathf.PI / 180.0f) * _y;
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
		if (Controller.GetComponent<Controller>().Started == true && !InGame && State == 0)
		{
			transform.localPosition = new Vector3(300.0f, 0.0f, 0.0f);
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
			if (Shield.activeSelf)
			{
				Hearts.GetComponent<HeartController>().Health++;
				Shield.SetActive(false);
			}
		}
		if(other.tag == "Star" && other.transform.parent != transform && other.transform.parent != transform)
		{
			Damage = true;
		}
		if (other.tag == "Explosion")
		{
			Damage = true;
		}
		if (other.tag == "Bullet")
		{
			if (other.GetComponent<BulletController>().From != transform)
			{
				Damage = true;
			}
		}
	}
	//When hit by a dodgeball turn Damage to true
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			Damage = true;
			if (Shield.activeSelf)
			{
				Hearts.GetComponent<HeartController>().Health++;
				Shield.SetActive(false);
			}
		}
		if (other.tag == "Star" && other.transform.parent != transform && other.transform.parent != transform)
		{
			Damage = true;
		}
		if (other.tag == "Explosion")
		{
			Damage = true;
		}
		if (other.tag == "Bullet")
		{
			if (other.GetComponent<BulletController>().From != transform)
			{
				Damage = true;
			}
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
	}

	[Command]
	void CmdAbility(float selected, Vector3 mouse, Vector3 regular)
	{
		GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>().RpcAbility(gameObject, selected, mouse, regular);
	}


	public void Ability(float selected, Vector3 mouse, Vector3 regular)
	{
		if (selected == 1)
		{
			Explode(mouse, regular);
		}
		if (selected == 2)
		{
			Blink();
		}
		if (selected == 3)
		{
			Turret();
		}
		if (selected == 4)
		{
			Block();
		}
		if (selected == 5)
		{
			Spinners();
		}
		if(isLocalPlayer || Controller.GetComponent<Controller>().SinglePlayer)
		{
			foreach (Transform t in _ability)
			{
				t.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
	}


	//When you press B blink
	void Blink()
	{
		_abilityCounter = CounterTime;
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
		float x = Mathf.Sin(angle / 180.0f * Mathf.PI) * 2.0f;
		float y = Mathf.Cos(angle / 180.0f * Mathf.PI) * 2.0f;
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

	//When you press B spin
	void Spinners()
	{
		Spin.SetActive(true);
		_spinCounter = CounterTime / 4;
		_abilityCounter = -1;
	}

	//When you press B spin
	void Block()
	{
		Shield.SetActive(true);
		_abilityCounter = CounterTime;
	}

	//When you press B spin
	void Explode(Vector3 mouse, Vector3 regular)
	{
		GameObject bomb = Instantiate(Bomb);
		_abilityCounter = CounterTime;

		float angle;
		if (mouse.x - regular.x >= 0.0f)
		{
			angle = 90 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		else
		{
			angle = 270 - Mathf.Atan((mouse.y - regular.y) / (mouse.x - regular.x)) * 180.0f / Mathf.PI;
		}
		angle = angle % 90;
		float x = Mathf.Sin(angle / 180.0f * Mathf.PI) * 2.0f;
		float y = Mathf.Cos(angle / 180.0f * Mathf.PI) * 2.0f;
		if (mouse.x - regular.x >= 0.0f && mouse.y - regular.y >= 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(x, y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y < 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(-x, -y, 0);
		}
		else if (mouse.x - regular.x < 0.0f && mouse.y - regular.y >= 0.0f)
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(-y, x, 0);
		}
		else
		{
			bomb.GetComponent<Rigidbody>().velocity = new Vector3(y, -x, 0);
		}
	}

	//When you press B spin
	void Turret()
	{
		float turn = 0.0f;
		for(int i = 0; i < 6; i++)
		{
			GameObject bullet = Instantiate(Bullet);
			bullet.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			bullet.transform.localEulerAngles = new Vector3(0.0f, 0.0f, turn);
			bullet.GetComponentInChildren<BulletController>().From = transform;
			turn += 60.0f;
		}
		_abilityCounter = CounterTime;
	}

	void setTime(float time)
	{
		if (Time > time)
		{
			time = Time;
		}
	}
}
