using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFactory : MonoBehaviour {

	public Target TargetPrefab;
	public List<Target> AllTargets;

	[SerializeField]
	private List<Target> _targets;
	public List<Target> Targets
	{
		get
		{
			return _targets;
		}

		set
		{
			_targets = value;
		}
	}

	private void Awake()
	{
		AllTargets = new List<Target>();
		Targets = new List<Target>();
		for (int i = 0; i < 10; i++) {
			Target newTarget = GameObject.Instantiate<Target>(TargetPrefab);
			newTarget.name = "Tgt_" + Mathf.Abs(newTarget.GetInstanceID());
			newTarget.transform.parent = transform;
			newTarget.transform.position = transform.position;
			newTarget.transform.rotation = Quaternion.identity;
			newTarget.transform.GetComponent<Rigidbody>().isKinematic = true;
			newTarget.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			newTarget.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			AllTargets.Add(newTarget);
			Targets.Add(newTarget);
		}
	}

	public Target GetTarget()
	{
		if (Targets.Count == 0)
		{
			return null;
		}
		else
		{
			Target obj = Targets[0];
			Targets.RemoveAt(0);
			obj.gameObject.SetActive(true);
			return obj;
		}
	}

	public void ReturnTarget(Target Target)
	{
		Target.transform.parent = transform;
		Target.transform.position = transform.position;
		Target.transform.rotation = Quaternion.identity;
		Target.transform.GetComponent<Rigidbody>().isKinematic = true;
		Target.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		Target.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		Targets.Add(Target);
	}
}
