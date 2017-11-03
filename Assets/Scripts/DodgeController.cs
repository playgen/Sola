using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeController : MonoBehaviour
{
//make it so it can't fire if its not the server one
	public GameObject Arrow, Parent;
	public Material[] Materials = new Material[7];
	public Sprite[] Sprites = new Sprite[4];
	public GameObject[] Players;

	public float RotationSpeed, Speed, ArrowNum, MaterialNum, Min, Max;
	public int State, BallNumber;

	GameObject _controller, _dbGenerator;
	float[] _angles;
	float _x, _y, _secondAngle, _varRandom, _rotationAngle;
	int _fireTime, _counter, _countDown;
	bool _hit, _server;

	// Gives the ball a random size and speed
	void Start()
	{
		_varRandom = Random.Range(0.0f, 1.0f);
		Speed = 10 + _varRandom * 10.0f;
		RotationSpeed = 1.7f + 1.7f * _varRandom;
		transform.localScale = new Vector3(1.0f - 0.7f * _varRandom, 1.0f - 0.7f * _varRandom, 0.1f);
		ArrowNum = Random.Range(0, 4);
		MaterialNum = Random.Range(0, 7);
		transform.GetComponent<Renderer>().material = Materials[(int)MaterialNum];
		Arrow.GetComponent<SpriteRenderer>().sprite = Sprites[(int)ArrowNum];
		_controller = GameObject.FindGameObjectWithTag("Controller");
		_dbGenerator = GameObject.FindGameObjectWithTag("DBGenerator");
		_hit = false;
		State = -1;
		_counter = 0;
		_countDown = 1;
		Min = -90.0f;
		Max = 90.0f;
		_y = 4.775f;
		_x = 8.645f;
		if (_dbGenerator.GetComponent<DBGenerator>().Server)
		{
			_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
		}
	}

	// Main code for the dodgeball
	void FixedUpdate()
	{
		if (_dbGenerator.GetComponent<DBGenerator>().Server)
		{
			_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
		}
		Arrow.GetComponent<SpriteRenderer>().enabled = true;
		_server = _dbGenerator.GetComponent<DBGenerator>().Server;
		Players = GameObject.FindGameObjectsWithTag("Player");
		//Just in case a ball spawns when theres no player. stops an error being thrown
		if (Players == null)
		{
			Players = new GameObject[] { Arrow };
		}
		//Initial state, stays here until the dodgeball spawns in a place where it isn't overlapping another
		if (State == -1)
		{
			State = 0;
			if (_hit)
			{
				_countDown = 1;
				State = 1;
			}
			else
			{
				GetComponent<MeshRenderer>().enabled = true;
			}
		}
		Angles();
		Reset();
		//fire time is the amount of time after the ball spawns before it can fire. just creates a delay
		if (_fireTime > 0 && _controller.GetComponent<Controller>().Started)
		{
			_fireTime--;
		}

		bool inArray = false;
		for (int j = 0; j < _angles.Length; j++)
		{
			if ((int)_secondAngle == (int)_angles[j])
			{
				inArray = true;
			}
		}
		//If the ball is facing a player and it has spawned but not yet been fired then fire
		if (inArray && State == 0 && _fireTime == 0 && _controller.GetComponent<Controller>().Started && _server)
		{
			State = 1;
		}
		//Hide the arrow
		if (State == 1)
		{
			Arrow.GetComponent<SpriteRenderer>().enabled = false;
		}

		if (State >= 1 && State <= 4 && _counter == 0)
		{
			_counter = 5;
		}
		//Blinking arrow before it fires
		else if (State >= 1 && State <= 4 && _counter != 0)
		{
			_counter--;
			if (_counter == 0)
			{
				State++;
				if (State == 2 || State == 4)
				{
					Arrow.GetComponent<SpriteRenderer>().enabled = true;
				}
				else
				{
					Arrow.GetComponent<SpriteRenderer>().enabled = false;
				}
			}
		}
		//move the ball in the direction it was fired
		else if (State == 5)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (Speed / 100.0f), transform.localPosition.z);
			Arrow.GetComponent<SpriteRenderer>().enabled = false;
		}
		else
		{
			//if the ball is still trying to find a target compare its angles to its angle in relation to all players. if there's a match fire in that direction
			if (RotationSpeed > 1.0f || RotationSpeed < 1.0f)
			{
				for (int i = 0; i < 5; i++)
				{
					Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed / 5));
					Angles();
					if (inArray && State == 0 && _fireTime == 0 && _controller.GetComponent<Controller>().Started && _server)
					{
						State = 1;
						if (_dbGenerator.GetComponent<DBGenerator>().Server)
						{
							_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
						}
					}
				}
			}
			else
			{
				Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed));
			}
			_rotationAngle = Parent.transform.localEulerAngles.z;
			if (_rotationAngle > 180.0f && Max != 270.0f)
			{
				_rotationAngle -= 360.0f;
			}
			if (_rotationAngle > Max || _rotationAngle < Min)
			{
				RotationSpeed = -RotationSpeed;
				Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed * 2));
			}
		}
	}

	//If the dodgeball hits an outer wall start a countdown to respawn and change the shape and speed of the dodgeball
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = true;
		}
		if (State != 0 && other.tag == "OuterWall" && _server)
		{
			_varRandom = Random.Range(0.0f, 1.0f);
			Speed = 10 + _varRandom * 10.0f;
			RotationSpeed = 1.7f + 1.7f * _varRandom;
			transform.localScale = new Vector3(1.0f - 0.7f * _varRandom, 1.0f - 0.7f * _varRandom, 0.1f);
			ArrowNum = Random.Range(0, 4);
			MaterialNum = Random.Range(0, 7);
			transform.GetComponent<Renderer>().material = Materials[(int) MaterialNum];
			Arrow.GetComponent<SpriteRenderer>().sprite = Sprites[(int)ArrowNum];
			_countDown = (int)Random.Range(30, 70);
			GetComponent<MeshRenderer>().enabled = false;
		}
	}

	//Colliding with another dodgeball
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = true;
		}
	}

	//Not colliding with another dodgeball
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = false;
		}
	}

	//Make an array list of the angle between the dodgeball and all current players
	void Angles()
	{
		_angles = new float[Players.Length];
		for (int i = 0; i < Players.Length; i++)
		{
			if (Players[i].GetComponent<PlayerController>().Health > 0)
			{
				if (Players[i].transform.position.x - transform.position.x >= 0.0f)
				{
					_angles[i] = 90 - Mathf.Atan((Players[i].transform.position.y - transform.position.y) / (Players[0].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
				}
				else
				{
					_angles[i] = 270 - Mathf.Atan((Players[i].transform.position.y - transform.position.y) / (Players[0].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
				}
				_angles[i] = 180.0f - _angles[i];
				while (_angles[i] < 0.0f)
				{
					_angles[i] += 360.0f;
				}
			}
			else
			{
				_angles[i] = 1000.0f;
			}
		}
		_secondAngle = Parent.transform.localEulerAngles.z;
		while (_secondAngle < 0.0f)
		{
			_secondAngle += 360.0f;
		}
	}

	//Reset the dodgeball along one of the walls
	void Reset()
	{
		if (_countDown > 0)
		{
			_countDown--;
			if (_countDown == 0)
			{
				_fireTime = (int)Random.Range(50, 500);
				int random = (int)Random.Range(0, 4);
				if (random == 0)
				{
					Parent.transform.localPosition = new Vector3(_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
					Max = 0.0f;
					Min = -180.0f;
				}
				else if (random == 1)
				{
					Parent.transform.localPosition = new Vector3(-_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					Max = 180.0f;
					Min = 0.0f;
				}
				else if (random == 2)
				{
					Parent.transform.localPosition = new Vector3(Random.Range(-_x, _x), _y, 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
					Min = -90.0f;
					Max = 90.0f;
				}
				else
				{
					Parent.transform.localPosition = new Vector3(Random.Range(-_x, _x), -_y, 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
					Min = 90.0f;
					Max = 270.0f;
				}
				transform.localPosition = Vector3.zero;
				State = -1;
				if (_dbGenerator.GetComponent<DBGenerator>().Server)
				{
					_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
				}
			}
		}
	}
}
