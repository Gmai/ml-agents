using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour {

	[SerializeField]
	private List<Target> _targets;
	[SerializeField]
	private List<Bullet> _bullets;

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

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "target") {
			if(Targets.Count<=20)
				Targets.Add(other.gameObject.GetComponent<Target>());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "target")
		{
			var index = Targets.IndexOf(other.gameObject.GetComponent<Target>());
			Targets.RemoveAt(index);
		}
	}
}
