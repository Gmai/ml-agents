using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

	public ShooterAgent Agent;
	public BulletFactory BulletFac;
	[SerializeField]
	private int BulletsInfScene;


	public List<Bullet> AllBullets {
		get
		{
			return BulletFac.AllBullets;
		}
	}

	public void Step () {
		for (int i = 0; i < transform.childCount; i++) {
			Bullet bullet = transform.GetChild(i).GetComponent<Bullet>();
			if (!(bullet?.Alive ?? true)) {
				BulletFac.ReturnBullet(bullet);
				Agent.BulletsToEliminate++;
			}
		}
	}

	public void Reset()
	{
		while (transform.childCount>0)
		{
			Bullet bullet = transform.GetChild(0).GetComponent<Bullet>();
			ReturnBullet(bullet);
		}
	}

	public Bullet SpawnBullet()
	{
		var newBullet = BulletFac.GetBullet();
		if (newBullet != null) { 
			newBullet.transform.position = transform.position;
			newBullet.transform.parent = transform;
			BulletsInfScene++;
		}
		return newBullet;
	}

	public void ReturnBullet(Bullet Bullet)
	{
		BulletFac.ReturnBullet(Bullet);
		BulletsInfScene--;
	}
}
