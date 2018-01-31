using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
	[SerializeField]
	private float _shootsAlive;
	public float MaxShootsAlive;
	public bool Alive;
	public bool Expired = false;

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "wall") {
			Expired = true;
		}
	}

	public void Step()
	{
		if (Alive) { 
			if (MaxShootsAlive > 0) { 
				if (_shootsAlive > MaxShootsAlive) {
					Expired = true;
				}
			}
		}
	}

	public void Reset(float targetMaxShoots)
	{
		Expired = false;
		Alive = true;
		MaxShootsAlive = targetMaxShoots;
		_shootsAlive = 0;
	}

	public void AddShoot() {
		if(Alive)
			_shootsAlive++;
	}
}
