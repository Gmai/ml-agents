using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour {

	public Bullet BulletPrefab;

	[SerializeField]
	private List<Bullet> _bullets;
	public List<Bullet> Bullets
	{
		get
		{
			return _bullets;
		}

		set
		{
			_bullets = value;
		}
	}

	public List<Bullet> AllBullets;

	private void Awake()
	{
		AllBullets = new List<Bullet>();
		Bullets = new List<Bullet>();
		for (int i = 0; i < 10; i++) {
			Bullet newBullet = GameObject.Instantiate<Bullet>(BulletPrefab);
			newBullet.name = "Blt_" + Mathf.Abs(newBullet.GetInstanceID());
			newBullet.transform.parent = transform;
			newBullet.transform.GetComponent<Rigidbody>().useGravity = true;
			newBullet.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			newBullet.transform.position = transform.position;
			newBullet.transform.rotation = Quaternion.identity;
			newBullet.Alive = true;
			newBullet.transform.GetComponent<Rigidbody>().isKinematic = true;
			AllBullets.Add(newBullet);
			Bullets.Add(newBullet);
		}
	}

	public Bullet GetBullet()
	{
		if (Bullets.Count == 0)
		{
			return null;
		}
		else
		{
			Bullet obj = Bullets[0];
			Bullets.RemoveAt(0);
			obj.transform.GetComponent<Rigidbody>().isKinematic = false;
			return obj;
		}
	}

	public void ReturnBullet(Bullet bullet)
	{
		bullet.transform.parent = transform;
		bullet.transform.GetComponent<Rigidbody>().useGravity = true;
		bullet.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		bullet.transform.position = transform.position;
		bullet.transform.rotation = Quaternion.identity;
		bullet.Alive = true;
		bullet.transform.GetComponent<Rigidbody>().isKinematic = true;
		Bullets.Add(bullet);
	}
}
