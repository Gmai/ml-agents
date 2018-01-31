using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetManager : MonoBehaviour {

	public ShooterAgent Agent;
	public TargetFactory TargetFac;
	[SerializeField]
	private int _targetsInScene;
	private TargetSettings _settings;
	private List<Target> _targetsScene = new List<Target>();

	public List<Target> AllTargets
	{
		get
		{
			return TargetFac.AllTargets;
		}

	}

	public void Reset(TargetSettings settings) {
		for (int i = 0; i < transform.childCount; i++)
		{
			Target target = transform.GetChild(i).GetComponent<Target>();
			ReturnTarget(target);
		}
		StopAllCoroutines();
		_targetsInScene = 0;
		_settings = settings;
	}

	public Target GetNextTarget() {
		if (_targetsScene.Count > 0)
			return _targetsScene[0];
		else return null;
	}

	public void Step()
	{
		foreach (var tgt in _targetsScene)
		{
			tgt.Step();
		}
		if (_settings != null) { 
			for (int i = 0; i < transform.childCount; i++)
			{
				Target target = transform.GetChild(i).GetComponent<Target>();
				if (target?.Expired ?? false)
				{
					Agent.TargetsMissedToEliminate++;
					ReturnTarget(target);
				}else if (!(target?.Alive ?? true)){
					ReturnTarget(target);
				}
			}
			if (_targetsInScene < _settings.max_targets_in_scene)
				SpawnTarget();
		}
	}

	public void SpawnTarget()
	{
			var newPosition = transform.position+ GetRandomPosition();
			var collision = Physics.OverlapBox(newPosition, new Vector3(1f,1f,1f)).Where(x=>x.name!= "TargetDetector").ToList();
			if (collision.Count == 0)
			{
				var newTarget = TargetFac.GetTarget();
				if (newTarget != null) { 
					newTarget.Reset(_settings.max_shoots_alive);
					newTarget.transform.position = newPosition;
					newTarget.transform.parent = transform;
					newTarget.transform.GetComponent<Rigidbody>().isKinematic = false;
					newTarget.transform.GetComponent<Rigidbody>().velocity = GetRandomSpeed();
					//spawned = true;
					_targetsScene.Add(newTarget);
					_targetsInScene++;
				}
			}
	}

	private Vector3 GetRandomPosition()
	{
		return new Vector3(
				Random.Range(_settings.spawning_random_x_min, _settings.spawning_random_x_max),
				Random.Range(_settings.spawning_random_y_min, _settings.spawning_random_y_max),
				Random.Range(_settings.spawning_random_z_min, _settings.spawning_random_z_max));
	}

	private Vector3 GetRandomSpeed()
	{
		return new Vector3(
				Random.Range(_settings.spawning_random_speed_x_min, _settings.spawning_random_speed_x_max),
				Random.Range(_settings.spawning_random_speed_y_min, _settings.spawning_random_speed_y_max),
				Random.Range(_settings.spawning_random_speed_z_min, _settings.spawning_random_speed_z_max));
	}

	public void ReturnTarget(Target target) {
		TargetFac.ReturnTarget(target);
		_targetsScene.Remove(target);
		_targetsInScene--;
	}

	public void AddShootToTargets() {
		foreach (var tgt in TargetFac.AllTargets) {
			tgt.AddShoot();
		}
	}
}
