using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "bullet") {
			collision.transform.GetComponent<Bullet>().Alive = false;
		}
	}
}
