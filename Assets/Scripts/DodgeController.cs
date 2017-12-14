using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The controller for an individual dodgeball
public class DodgeController : MonoBehaviour
{
	public GameObject Arrow, Parent, Icon, Invis;
	public Material[] Materials;
	public Sprite[] Sprites, SpecialSprites;
	public GameObject[] Players, Duplicates;

	public float RotationSpeed, Speed, ArrowNum, MaterialNum, Min, Max;
	public int State, BallNumber, Special;

	GameObject _controller, _dbGenerator, _closestPlayer;
	float[] _angles;
	float _x, _y, _secondAngle, _varRandom, _rotationAngle, _stretch;
	int _fireTime, _sightTime, _counter, _countDown, _lastSpecial, _multi;
	bool _hit, _server, _left;

	// Gives the ball a random size and speed
	void Start()
	{
		SetUp();
		_controller = GameObject.FindGameObjectWithTag("Controller");
		// 2 different controllers, one for single player games without all the network stuff attached and one for online 
		if(_controller.GetComponent<Controller>().SinglePlayer)
		{
			_dbGenerator = GameObject.FindGameObjectWithTag("DBSGenerator");
		}
		else
		{
			_dbGenerator = GameObject.FindGameObjectWithTag("DBGenerator");
		}
		_hit = false;
		_left = false;
		State = -1;
		_counter = 0;
		_multi = 0;
		_countDown = 1;
		Min = -90.0f;
		Max = 90.0f;
		_y = 4.775f;
		_x = 8.645f;
		// If it is an online game add the ball to the array of balls which will be used 
		// to send updates to clients
		if (_dbGenerator.GetComponent<DBGenerator>().Server)
		{
			_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
		}
	}

	// Main code for the dodgeball
	void FixedUpdate()
	{
		// Find a list of all the players
		Players = GameObject.FindGameObjectsWithTag("Player");
		// If it is not a special ball do nothing
		if (Special == 0)
		{
			Icon.GetComponent<SpriteRenderer>().enabled = false;
		}
		// If it is a special ball set the correct icon and give the value to _lastSpecial
		else if (Special < 4)
		{
			Icon.GetComponent<SpriteRenderer>().sprite = SpecialSprites[Special - 1];
			_lastSpecial = Special;
		}
		else
		{
			// If its an invisible ball set it to the translucent sprite
			if(_lastSpecial == 1)
			{
				GetComponent<MeshRenderer>().enabled = false;
				Icon.GetComponent<SpriteRenderer>().enabled = false;
				Invis.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		if (_dbGenerator.GetComponent<DBGenerator>().Server)
		{
			_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
		}
		_server = _dbGenerator.GetComponent<DBGenerator>().Server;
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
		}
		// Get the angles between the dodgeball and all the players
		Angles();
		// Respawn the dodgeball if necessary
		Reset();
		//fire time is the amount of time after the ball spawns before it can fire. just creates a delay
		if (_fireTime > 0 && _controller.GetComponent<Controller>().Started)
		{
			// Dont show the ball until you're sure it didn't spawn ontop of another ball
			if(_sightTime == 0)
			{
				GetComponent<MeshRenderer>().enabled = true;
				Arrow.GetComponent<SpriteRenderer>().enabled = true;
				// If it is a special ball show the icon
				if (_lastSpecial < 4 && _lastSpecial > 0)
				{
					Icon.GetComponent<SpriteRenderer>().enabled = true;
				}
			}
			else
			{
				_sightTime--;
			}
			_fireTime--;
		}

		// Check if any of the angles to the player match the dodgeballs angles
		bool inArray = false;
		for (int j = 0; j < _angles.Length; j++)
		{
			if ((int)_secondAngle == (int)_angles[j])
			{
				inArray = true;
			}
		}
		//States represent different stages in the balls life
		// Start : 0, Countdown to fire : 1 - 4, Fired - 5
		//If the ball is facing a player and it has spawned but not yet been fired then fire
		if (inArray && State == 0 && _fireTime == 0 && _controller.GetComponent<Controller>().Started && (_server || _controller.GetComponent<Controller>().SinglePlayer))
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
			// Activate the special ball
			if (Special != 0)
			{
				Special = 4;
			}

			// If it is a Multiply ball activate the other balls and fire them
			if (_lastSpecial == 2)
			{
				foreach (GameObject g in Duplicates)
				{
					g.SetActive(true);
					g.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (Speed / 100.0f), transform.localPosition.z);
					g.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
				}
			}

			// Move the dodgeball
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (Speed / 100.0f), transform.localPosition.z);
			// Disable the arrow sprite
			Arrow.GetComponent<SpriteRenderer>().enabled = false;

			//If it is a Stretch ball stretch it out until its scale is (2, 0.5, 1) and also rotate the ball
			if(Special == 4 && _lastSpecial == 3 && _countDown == 0)
			{
				if (transform.localScale.x < _stretch * 2)
				{
					transform.localScale = new Vector3(transform.localScale.x + (_stretch / 50.0f), transform.localScale.y - (_stretch / 100.0f), transform.localScale.z);
				}
				if(_left)
				{
					transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z - 2.0f);
				}
				else
				{
					transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + 2.0f);
				}
			}
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
						// Set the state to 1 which means its preparing to fire
						State = 1;
					}
				}
			}
			// If there is no match keep rotating
			else
			{
				Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed));
			}
			_rotationAngle = Parent.transform.localEulerAngles.z;
			if (_rotationAngle > 180.0f && Max != 270.0f)
			{
				_rotationAngle -= 360.0f;
			}
			// Onece it rotates all the way one way, start rotating in the other direction
			if (_rotationAngle > Max || _rotationAngle < Min)
			{
				RotationSpeed = -RotationSpeed;
				Parent.transform.localEulerAngles = new Vector3(Parent.transform.localEulerAngles.x, Parent.transform.localEulerAngles.y, Parent.transform.localEulerAngles.z + (RotationSpeed * 2));
			}
		}
	}
	
	//Make an array list of the angle between the dodgeball and all current players
	void Angles()
	{
		_angles = new float[Players.Length];
		for (int i = 0; i < Players.Length; i++)
		{
			// Only include players that are still in the game
			if (Players[i].GetComponent<PlayerController>().Health > 0)
			{
				if (Players[i].transform.position.x - transform.position.x >= 0.0f)
				{
					_angles[i] = 90 - Mathf.Atan((Players[i].transform.position.y - transform.position.y) / (Players[i].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
				}
				else
				{
					_angles[i] = 270 - Mathf.Atan((Players[i].transform.position.y - transform.position.y) / (Players[i].transform.position.x - transform.position.x)) * 180.0f / Mathf.PI;
				}
				_angles[i] = 180.0f - _angles[i];
				// Set the angles constraints to 0-360
				while (_angles[i] < 0.0f)
				{
					_angles[i] += 360.0f;
				}
			}
			// If the player is dead give them an unreachable angle
			else
			{
				_angles[i] = 1000.0f;
			}
		}
		// Angle of the dodgeball within the constraint 0-360
		_secondAngle = Parent.transform.localEulerAngles.z;
		while (_secondAngle < 0.0f)
		{
			_secondAngle += 360.0f;
		}
	}

	//Reset the dodgeball along one of the walls and decides if it will be a special ball.
	// will keep going until the ball doesn't spawn on top of another ball
	void Reset()
	{
		if (_countDown > 0)
		{
			_countDown--;
			// Wait until the countdown has ended to respawn
			if (_countDown == 0)
			{
				// Time until the ball fires so it doesn't immediately spawn then fire
				_fireTime = (int)Random.Range(50, 500);
				// Time until the ball can be seen. Stops the ball from jumping around whilst if it spawns on top of another ball
				_sightTime = 2;
				// The ball spawns on a random wall
				int random = (int)Random.Range(0, 4);
				// Right wall
				if (random == 0)
				{
					Parent.transform.localPosition = new Vector3(_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
					Max = 0.0f;
					Min = -180.0f;
				}
				// Left wall
				else if (random == 1)
				{
					Parent.transform.localPosition = new Vector3(-_x, Random.Range(-_y, _y), 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					Max = 180.0f;
					Min = 0.0f;
				}
				// Top wall
				else if (random == 2)
				{
					Parent.transform.localPosition = new Vector3(Random.Range(-_x, _x), _y, 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
					Min = -90.0f;
					Max = 90.0f;
				}
				// Bottom wall
				else
				{
					Parent.transform.localPosition = new Vector3(Random.Range(-_x, _x), -_y, 0);
					Parent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
					Min = 90.0f;
					Max = 270.0f;
				}
				// Start state
				State = -1;
				int special = Random.Range(0, 100);
				// each special ball has a 3% probablitiy of spawning
				// Invisible ball
				if(special < 3)
				{
					Special = 1;
				}
				// Multi Ball
				else if (special < 6)
				{
					Special = 2;
				}
				// Stretch Ball
				else if (special < 9)
				{
					Special = 3;
					// Rotates either left or right
					_left = (Random.Range(-1, 1) > 0);
					// Gets the original width of the ball
					_stretch = transform.localScale.x;
				}
				// Regular ball
				else
				{
					Special = 0;
				}
				// If the user is the server add the dodgeball to the array of updates to inform the clients
				// so they can match the servers ball
				if (_dbGenerator.GetComponent<DBGenerator>().Server)
				{
					_dbGenerator.GetComponent<DBGenerator>().BallUpdates[BallNumber] = Parent;
				}

				// Set rotation and position to 0
				transform.localEulerAngles = Vector3.zero;
				transform.localPosition = Vector3.zero;
				// Reset any special characteristics
				Invis.GetComponent<SpriteRenderer>().enabled = false;
				foreach (GameObject g in Duplicates)
				{
					g.SetActive(false);
					g.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				}
			}
		}
	}

	// Initialises the dodgeball
	void SetUp()
	{
		// Random float in the range 0-1 everything else is based off. Smaller balls move faster
		_varRandom = Random.Range(0.0f, 1.0f);
		// Base speed of 10 with a max speed of 20
		Speed = 10 + _varRandom * 10.0f;
		// Speed at which it rotates whilst looking for a player to fire at. between 1.7-3.4.
		// Range has no specific meaning but it works pretty well
		RotationSpeed = 1.7f + 1.7f * _varRandom;
		// Size of between 0.3-1.0
		transform.localScale = new Vector3(1.0f - 0.7f * _varRandom, 1.0f - 0.7f * _varRandom, 0.1f);
		// Set the ball and its arrow to a random colour
		ArrowNum = Random.Range(0, 4);
		MaterialNum = Random.Range(0, 7);
		transform.GetComponent<Renderer>().material = Materials[(int)MaterialNum];
		Arrow.GetComponent<SpriteRenderer>().sprite = Sprites[(int)ArrowNum];
		// Hide ball initially until it is spawned in a place without a dodgeball
		GetComponent<MeshRenderer>().enabled = false;
	}

	// If the ball tries to respawn in a place where there is already a dodgeball then _hit = true which will try to respawn it again
	//If the dodgeball hits an outer wall start a countdown to respawn and change the shape and speed of the dodgeball
	void OnTriggerEnter(Collider other)
	{
		// Hit another dodgeball
		if (other.tag == "DodgeBall")
		{
			_hit = true;
		}
		// Hit an outer wall and resets
		if (State != 0 && other.tag == "OuterWall" && (_server || _controller.GetComponent<Controller>().SinglePlayer))
		{
			SetUp();
			// Time until the ball respawns
			_countDown = Random.Range(30, 70);
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

	//No longer colliding with another dodgeball
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "DodgeBall")
		{
			_hit = false;
		}
	}
}
