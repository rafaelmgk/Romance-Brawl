using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineController : MonoBehaviour {
	[SerializeField] private CinemachineVirtualCamera _vcam0;
	[SerializeField] private CinemachineVirtualCamera _vcam1;

	private void Update() {
		if (IsAtLeastOnePlayerOutOfLimits()) {
			_vcam0.Priority = 0;
			_vcam1.Priority = 1;
		} else {
			_vcam0.Priority = 1;
			_vcam1.Priority = 0;
		}
	}

	private bool IsAtLeastOnePlayerOutOfLimits() {
		foreach (KeyValuePair<int, bool> entry in GameObject.FindWithTag("Data").GetComponent<DataManager>().arePlayersOutOfLimits)
			if (entry.Value) return true;

		return false;
	}
}
