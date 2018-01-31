using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ShooterAcademy : Academy
{

	private TargetManager[] _targetManagers;
	private BulletManager[] _bulletManagers;
	private ShooterAgent[] _shooterAgents;
	private TargetSettings Settings;
	[Header("Specific to Shooter")]
	public float max_targets_in_scene;
	public float max_shoots_alive;
	public float spawning_random_x_min;
	public float spawning_random_x_max;
	public float spawning_random_y_min;
	public float spawning_random_y_max;
	public float spawning_random_z_min;
	public float spawning_random_z_max;
	public float spawning_random_speed_x_min;
	public float spawning_random_speed_x_max;
	public float spawning_random_speed_y_min;
	public float spawning_random_speed_y_max;
	public float spawning_random_speed_z_min;
	public float spawning_random_speed_z_max;

	public bool ShowRewards = false;

	public override void InitializeAcademy()
	{
		_shooterAgents = GameObject.FindObjectsOfType<ShooterAgent>();
	}

	public override void AcademyReset()
	{
		max_targets_in_scene = (float)resetParameters["max_targets_in_scene"];
		max_shoots_alive = (float)resetParameters["max_shoots_alive"];
		spawning_random_x_min = (float)resetParameters["spawning_random_x_min"];
		spawning_random_x_max = (float)resetParameters["spawning_random_x_max"];
		spawning_random_y_min = (float)resetParameters["spawning_random_y_min"];
		spawning_random_y_max = (float)resetParameters["spawning_random_y_max"];
		spawning_random_z_min = (float)resetParameters["spawning_random_z_min"];
		spawning_random_z_max = (float)resetParameters["spawning_random_z_max"];
		spawning_random_speed_x_min = (float)resetParameters["spawning_random_speed_x_min"];
		spawning_random_speed_x_max = (float)resetParameters["spawning_random_speed_x_max"];
		spawning_random_speed_y_min = (float)resetParameters["spawning_random_speed_y_min"];
		spawning_random_speed_y_max = (float)resetParameters["spawning_random_speed_y_max"];
		spawning_random_speed_z_min = (float)resetParameters["spawning_random_speed_z_min"];
		spawning_random_speed_z_max = (float)resetParameters["spawning_random_speed_z_max"];

		Settings = new TargetSettings()
		{
			max_targets_in_scene = max_targets_in_scene,
			max_shoots_alive = max_shoots_alive ,
			spawning_random_x_min = spawning_random_x_min,
			spawning_random_x_max = spawning_random_x_max,
			spawning_random_y_min = spawning_random_y_min,
			spawning_random_y_max = spawning_random_y_max,
			spawning_random_z_min = spawning_random_z_min,
			spawning_random_z_max = spawning_random_z_max,
			spawning_random_speed_x_min = spawning_random_speed_x_min,
			spawning_random_speed_x_max = spawning_random_speed_x_max,
			spawning_random_speed_y_min = spawning_random_speed_y_min,
			spawning_random_speed_y_max = spawning_random_speed_y_max,
			spawning_random_speed_z_min = spawning_random_speed_z_min,
			spawning_random_speed_z_max = spawning_random_speed_z_max
		};

		_targetManagers = GameObject.FindObjectsOfType<TargetManager>();
		foreach (var tgtm in _targetManagers)
		{
			tgtm.Reset(Settings);
		}

		_bulletManagers = GameObject.FindObjectsOfType<BulletManager>();
		foreach (var bltm in _bulletManagers)
		{
			bltm.Reset();
		}
	}

	public override void AcademyStep()
	{
		foreach (var tgtm in _targetManagers)
		{
			tgtm.Step();
		}
		foreach (var bltm in _bulletManagers)
		{
			bltm.Step();
		}
	}

	public void OnGUI()
	{
		if (ShowRewards) { 
			StringBuilder text = new StringBuilder();
			text.Append("Rewards"+Environment.NewLine);
			foreach (var ag in _shooterAgents)
			{
				text.Append(ag.transform.parent.name + ": " + ag.CumulativeReward.ToString("0.000") + Environment.NewLine);
				text.Append("BulletsShoot: " + ag.BulletsShoot.ToString() + Environment.NewLine);
			}
			foreach(var sa in _shooterAgents) { 
				if (!string.IsNullOrEmpty(sa.ExtraText)) {
					text.Append(sa.ExtraText);
				}
			}
			GUI.TextArea(new Rect(0, 0, 100, 300), text.ToString());
		}
	}
}
