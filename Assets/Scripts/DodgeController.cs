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
	float _x, _y, _angle, _secondAngle, _varRandom, _rotationAngle;
	int _fireTime, _counter, _countDown;
	bool _hit, _server;

	// Use this for initialization
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
		_angle = 500.0f;
		Min = -90.0f;
		Max = 90.0f;
		_y = 4.775f;
		_x = 8.645f;
		if (_dbGenerator.GetComponent<DBGenerator>().Server)
		{
			_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		_server = _dbGenerator.GetComponent<DBGenerator>().Server;
		Players = GameObject.FindGameObjectsWithTag("Player");
		if (Players == null)
		{
			Players = new GameObject[] { Arrow };
		}
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
				Arrow.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		Angles();
		Reset();
		if (_fireTime > 0 && _controller.GetComponent<Controller>().Started)
		{
			_fireTime--;
		}
		if ((int)_secondAngle == (int)_angle && State == 0 && _fireTime == 0 && _controller.GetComponent<Controller>().Started && _server)
		{
			State = 1;
		}
		if (State == 1)
		{
			Arrow.GetComponent<SpriteRenderer>().enabled = false;
		}
		if (State >= 1 && State <= 4 && _counter == 0)
		{
			_counter = 5;
		}
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
		else if (State == 5)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (Speed / 100.0f), transform.localPosition.z);
		}
		else
		{
			if (RotationSpeed > 1.0f || RotationSpeed < 1.0f)
			{
				for (int i = 0; i < 5; i++)
				{
					Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed / 5));
					Angles();
					if ((int)_secondAngle == (int)_angle && State == 0 && _fireTime == 0 && _controller.GetComponent<Controller>().Started && _server)
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

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = true;
		}
		if (State != 0 && other.tag == "OuterWall")
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

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = false;
		}
	}

	void Angles()
	{
		if (Players[0].transform.position.x - transform.position.x >= 0.0f)
		{
			_angle = 90 - Mathf.Atan((Players[0].transform.position.y - transform.position.y) / (Players[0].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
		}
		else
		{
			_angle = 270 - Mathf.Atan((Players[0].transform.position.y - transform.position.y) / (Players[0].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
		}
		_angle = 180.0f - _angle;
		while (_angle < 0.0f)
		{
			_angle += 360.0f;
		}
		_secondAngle = Parent.transform.localEulerAngles.z;
		while (_secondAngle < 0.0f)
		{
			_secondAngle += 360.0f;
		}
	}

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
					Parent.transform.position = new Vector3(_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
					Max = 0.0f;
					Min = -180.0f;
				}
				else if (random == 1)
				{
					Parent.transform.position = new Vector3(-_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					Max = 180.0f;
					Min = 0.0f;
				}
				else if (random == 2)
				{
					Parent.transform.position = new Vector3(Random.Range(-_x, _x), _y, 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
					Min = -90.0f;
					Max = 90.0f;
				}
				else
				{
					Parent.transform.position = new Vector3(Random.Range(-_x, _x), -_y, 0);
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
