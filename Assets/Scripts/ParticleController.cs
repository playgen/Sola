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

	// Play particle effects
	public void Play()
	{
		foreach (ParticleSystem p in _particles)
		{
			p.Play();
		}
	}

	// Stop particle effects
	public void Stop()
	{
		foreach (ParticleSystem p in _particles)
		{
			p.Stop();
		}
	}
}