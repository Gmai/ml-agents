using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class ShooterAgent : Agent
{

	[Header("Specific to Shooter")]
	public TargetDetector TgtDetector;
	public BulletManager BulletMan;
	public TargetManager TargetMan;
	public GameObject SpawningPoint;
	public GameObject BaseCanon;
	public GameObject GunCanon;
	public Vector3 InitialForwardBase;
	public Vector3 InitialForwardGun;
	public Quaternion InitialRotationBase;
	public Quaternion InitialRotationGun;
	public float BulletSpeed;
	public float ShootInterval;
	private float _deltaTimeShoot;
	private float _lastCummulativeReward;
	private float _actionShoot;
	private List<int> _targetsToEliminate;
	public List<int> TargetsToEliminate
	{
		get
		{
			return _targetsToEliminate;
		}

		set
		{
			_targetsToEliminate = value;
		}
	}
	public int BulletsToEliminate;
	public int TargetsMissedToEliminate;
	private int MaxShootsTargetLive;

	public string ExtraText;

	private float NormalizeX(float x)
	{
		return ((x-transform.position.x) + 54) / (52 + 52);
	}
	private float NormalizeY(float y)
	{
		return (y - transform.position.y+4) / 24;
	}
	private float NormalizeZ(float z)
	{
		return (z - transform.position.z+2) / 102;
	}
	private float NormalizeSpeedX(float x)
	{
		return (x + 2) / (2 + 2);
	}
	private float NormalizeSpeedY(float y)
	{
		return (y + 2) / (2 + 2);
	}
	private float NormalizeSpeedZ(float z)
	{
		return (z + 2) / (2 + 2);
	}
	private float NormalizeBulletSpeedX(float x)
	{
		return (x + 40) / (40 + 40);
	}
	private float NormalizeBulletSpeedY(float y)
	{
		return (y + 40) / (40 + 40);
	}
	private float NormalizeBulletSpeedZ(float z)
	{
		return (z + 40) / (40 + 40);
	}
	private Target NextTarget = null;
	public int BulletsShoot;

	public override List<float> CollectState()
	{
		List<float> state = new List<float>();
		state.Add((BaseCanon.transform.rotation.y + 1) / 2);
		state.Add((GunCanon.transform.rotation.x + 1) / 2);
		state.Add(_actionShoot);
		if (NextTarget!=null)
		{
			state.Add(NormalizeX(NextTarget.transform.position.x - transform.position.x));
			state.Add(NormalizeY(NextTarget.transform.position.y - transform.position.y));
			state.Add(NormalizeZ(NextTarget.transform.position.z - transform.position.z));
			var speed = NextTarget.transform.GetComponent<Rigidbody>().velocity;
			state.Add(NormalizeSpeedX(speed.x));
			state.Add(NormalizeSpeedY(speed.y));
			state.Add(NormalizeSpeedZ(speed.z));
			state.Add(((float)NextTarget.MaxShootsAlive) / ((float)MaxShootsTargetLive));
		}
		else
		{
			state.Add(0);
			state.Add(0);
			state.Add(0);
			state.Add(0);
			state.Add(0);
			state.Add(0);
			state.Add(0);
		}

		foreach (Bullet bul in BulletMan.AllBullets)
		{
			if (bul.transform.position.z > transform.position.z)
			{
				state.Add(NormalizeX(bul.transform.position.x - transform.position.x));
				state.Add(NormalizeY(bul.transform.position.y - transform.position.y));
				state.Add(NormalizeZ(bul.transform.position.z - transform.position.z));
				var speed = bul.transform.GetComponent<Rigidbody>().velocity;
				state.Add(NormalizeBulletSpeedX(speed.x));
				state.Add(NormalizeBulletSpeedY(speed.y));
				state.Add(NormalizeBulletSpeedZ(speed.z));
			} else {
				state.Add(0);
				state.Add(0);
				state.Add(0);
				state.Add(0);
				state.Add(0);
				state.Add(0);
			}
		}
		_actionShoot = 0;
		return state;
	}

	public void WriteToFile(List<float> states) {
		string text = "";
		if (File.Exists("state.txt"))
		{
			StreamReader sr = File.OpenText("state.txt");
			text = sr.ReadToEnd();
			sr.Close();
		}
		var file = File.CreateText("state.txt");
		text +="["+ string.Join(",", states) + "]" + Environment.NewLine;
		file.Write(text);
		file.Close();
	}

	public override void InitializeAgent() {
		TargetsToEliminate = new List<int>();
		InitialForwardBase = BaseCanon.transform.forward;
		InitialForwardGun = GunCanon.transform.forward;
		InitialRotationBase = BaseCanon.transform.rotation;
		InitialRotationGun = GunCanon.transform.rotation;
		_deltaTimeShoot = ShootInterval;
		MaxShootsTargetLive = (int)GameObject.FindObjectOfType<ShooterAcademy>().max_shoots_alive;
	}

	public void Shoot() {
		var bulletColided = Physics.OverlapSphere(transform.position, 20).Any(x=>x.transform.parent.tag=="bullet");
		if (!bulletColided)
		{
			Bullet newBullet = BulletMan.SpawnBullet();
			if (newBullet != null) { 
				BulletsShoot++;
				TargetMan.AddShootToTargets();
				newBullet.Shooter = this;
				newBullet.transform.position = SpawningPoint.transform.position;
				newBullet.Shoot(BulletSpeed, SpawningPoint.transform.forward);
				_deltaTimeShoot = 0;
				_actionShoot = 1;
			}
		}
	}

	private float AngleBetweenVector2(Vector2 v1, Vector2 v2)
	{
		float ang = Vector2.Angle(v1, v2);
		if (ang == 0) 
			return 0;

		float direction = v1.x * v2.y - v1.y * v2.x;
		if (direction == 0)
			return 180;
		else if (direction > 0)
			return ang;
		else
			return -ang;
	}

	public override void AgentStep(float[] act)
	{
		float actionY = Mathf.Clamp(act[2], -1f, 1f); 
		float actionX = Mathf.Clamp(act[1], -1f, 1f);
		float shoot = Mathf.Clamp(act[0], 0f, 1f);

		ExtraText = "shoot: " + act[0] + Environment.NewLine;
		ExtraText += "X: " + act[1] + Environment.NewLine;
		ExtraText += "Y: " + act[2] + Environment.NewLine;

		var originalRotationBase = BaseCanon.transform.rotation;
		var originalRotationGun = GunCanon.transform.localRotation;
		float rotAverageY = 0.5f * actionY;
		if (rotAverageY > 2) rotAverageY = 1;
		float rotAverageX = 0.5f * actionX;
		if (rotAverageX > 2) rotAverageX = 1;

		if (actionY < 0)
		{
			float angle = AngleBetweenVector2(new Vector2(InitialForwardBase.z, InitialForwardBase.x), new Vector2(BaseCanon.transform.forward.z, BaseCanon.transform.forward.x));
			if (angle - rotAverageY > -40)
			{
				Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.up);
				BaseCanon.transform.rotation = originalRotationBase * yQuaternion;
			}
		}
		else if (actionY > 0)
		{
			float angle = AngleBetweenVector2(new Vector2(InitialForwardBase.z, InitialForwardBase.x), new Vector2(BaseCanon.transform.forward.z, BaseCanon.transform.forward.x));
			if (angle + rotAverageY < 40)
			{
				Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.up);
				BaseCanon.transform.rotation = originalRotationBase * yQuaternion;
			}
		}

		if (actionX > 0)
		{
			float angle = AngleBetweenVector2(new Vector2(InitialForwardGun.y, InitialForwardGun.z), new Vector2(GunCanon.transform.forward.y, GunCanon.transform.forward.z));
			if (angle - rotAverageX > -15)
			{
				Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.left);
				GunCanon.transform.localRotation = originalRotationGun * xQuaternion;
			}
		}
		else if (actionX < 0)
		{
			float angle = AngleBetweenVector2(new Vector2(InitialForwardGun.y, InitialForwardGun.z), new Vector2(GunCanon.transform.forward.y, GunCanon.transform.forward.z));
			if (angle + rotAverageX < 15)
			{
				Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.left);
				GunCanon.transform.localRotation = originalRotationGun * xQuaternion;
			}
		}

		if (NextTarget == null)
		{
			NextTarget = TargetMan.GetNextTarget();
		}
		else if (NextTarget.transform.position.z > transform.position.z) {
			NextTarget = null;
		}

		if (shoot > 0)
		{
			Shoot();
		}

		while (TargetsToEliminate.Count > 0) {
			var targets = TargetsToEliminate[0];
			reward += (Mathf.Pow(2, targets- 1));
			TargetsToEliminate.RemoveAt(0);
		}

		_lastCummulativeReward = CumulativeReward;

		if (CumulativeReward > 20) {
			done = true;
		}
		reward -= 0.01f;
		Monitor.Log("Reward", reward, MonitorType.slider, gameObject.transform);
	}

	public override void AgentReset()
	{
		BaseCanon.transform.rotation = InitialRotationBase;
		GunCanon.transform.rotation = InitialRotationGun;
		_deltaTimeShoot = ShootInterval;
		BulletsShoot = 0;
		MaxShootsTargetLive = (int)GameObject.FindObjectOfType<ShooterAcademy>().max_shoots_alive;
	}

	public override void AgentOnDone()
	{
	}
}
