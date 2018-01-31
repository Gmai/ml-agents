using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBulletOnTouch : MonoBehaviour {

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.parent.tag == "bullet")
		{
			collision.transform.parent.GetComponent<Bullet>().Alive = false;
		}
	}
}
