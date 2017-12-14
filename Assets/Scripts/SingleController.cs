using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Controller for single player games as a replacement for the network controller.
// Has the same functions but they don't attempt to make any UNet connections
public class SingleController : MonoBehaviour {

	public GameObject DBGenerator, Dodgeball;
	public Sprite[] Sprites = new Sprite[2];

	GameObject _player;

	void Start()
	{
	}

	void FixedUpdate()
	{
		// Set _player equal to the player object
		_player = GameObject.FindGameObjectWithTag("Player");

		// Only execute on single player games
		if (GetComponent<Controller>().SinglePlayer)
		{
			// If you press tab start a game
			if (Input.GetKey(KeyCode.Tab) && GetComponent<Controller>().StartCounter == -1)
			{
				// Counter between initialization and the actual game starting
				GetComponent<Controller>().StartCounter = 61;
				_player.GetComponent<PlayerController>().InGame = true;
				// Tell the dodgeball generator that the game has started
				GameObject.FindGameObjectWithTag("DBSGenerator").GetComponent<DBGenerator>().InGame = true;
			}
			// When the player takes damage lose a life unless currently invunerable
			if (_player.GetComponent<PlayerController>().Damage)
			{
				_player.GetComponent<PlayerController>().Damage = false;
				LoseLife();
			}
		}
	}

	// If you haven't just taken damage lose a life and become invincible for 50 frames
	public void LoseLife()
	{
		if (_player.GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health != 0 && _player.GetComponent<PlayerController>().Invincibility == 0)
		{
			_player.GetComponent<PlayerController>().Invincibility = 50;
			_player.GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health--;
		}
	}

	// Spawn 5/1 dodgeballs depending on the parameter
	public void Dodge(bool five)
	{
		if (five)
		{
			GameObject.FindGameObjectWithTag("DBSGenerator").GetComponent<DBGenerator>().PulseFive = true;
		}
		else
		{
			GameObject.FindGameObjectWithTag("DBSGenerator").GetComponent<DBGenerator>().Pulse = true;
		}
	}

	// Call the resetPlayer method in the playerController script
	public void Resets()
	{
		_player.GetComponent<PlayerController>().ResetPlayer();
	}
}
