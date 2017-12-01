using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Particle controller. Has a play and stop method
public class ParticleController : MonoBehaviour {

	ParticleSystem[] _particles;
	// Use this for initialization
	void Start()
	{
		_particles = GetComponentsInChildren<ParticleSystem>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{

	}

	public void Play()
	{
		foreach (ParticleSystem p in _particles)
		{
			p.Play();
		}
	}
	public void Stop()
	{
		foreach (ParticleSystem p in _particles)
		{
			p.Stop();
		}
	}
}