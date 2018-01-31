using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public ShooterAgent Shooter;
	public float ExplosionRange;
	public List<Target> TargetsToEliminate;
	public bool Alive;

	private void Start()
	{
		Alive = true;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (Alive) { 
			if (collision.collider.tag == "target")
				Explode();
		}
	}

	public void Explode() {
		var objs = Physics.OverlapSphere(transform.position, ExplosionRange);
		var targetsEliminated = 0;
		foreach(var obj in objs)
		{
			if (obj.transform.tag == "target") {
				Target tgt = obj.transform.GetComponent<Target>();
				tgt.GetComponent<Rigidbody>().isKinematic = true;
				tgt.GetComponent<Rigidbody>().velocity = Vector3.zero;
				tgt.GetComponent<Rigidbody>().rotation = Quaternion.identity;
				tgt.Alive = false;
				targetsEliminated++;
			}
		}

		if (targetsEliminated > 0)
		{
			Shooter.TargetsToEliminate.Add(targetsEliminated);
		}
		Alive = false;
	}

	public void Shoot(float speed,Vector3 direction) {
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		transform.forward = direction;
		rb.velocity = direction.normalized * speed;
	}

}
