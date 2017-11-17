using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleController : MonoBehaviour {

	public GameObject DBGenerator, Dodgeball;
	public Sprite[] Sprites = new Sprite[2];

	GameObject _player;

	// Use this for initialization
	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		if (GetComponent<Controller>().SinglePlayer)
		{
			if (Input.GetKey(KeyCode.Tab) && GetComponent<Controller>().StartCounter == -1)
			{
				GetComponent<Controller>().StartCounter = 60;
				_player.GetComponent<PlayerController>().InGame = true;
				GameObject.FindGameObjectWithTag("DBSGenerator").GetComponent<DBGenerator>().InGame = true;
			}
			if (_player.GetComponent<PlayerController>().Damage)
			{
				_player.GetComponent<PlayerController>().Damage = false;
				LoseLife();
			}
		}
	}

	public void LoseLife()
	{
		if (_player.GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health != 0 && _player.GetComponent<PlayerController>().Invincibility == 0)
		{
			_player.GetComponent<PlayerController>().Invincibility = 50;
			_player.GetComponent<PlayerController>().Hearts.GetComponent<HeartController>().Health--;
		}
	}

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
	public void Resets()
	{
		for (int i = 0; i < GetComponent<Controller>().Players.Length; i++)
		{
			_player.GetComponent<PlayerController>().ResetPlayer();
		}
	}
}
