using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DynamicTargetGroup : MonoBehaviour {
	private void Start() {
		StartCoroutine(ReadyGo());
	}

	private IEnumerator ReadyGo() {
		yield return new WaitForSeconds(3);
		foreach(GameObject player in GameManager.Instance.currentPlayers)
			AddMemberToTargetGroup(player);
	}

	public void AddMemberToTargetGroup(GameObject player) {
		CinemachineTargetGroup targetGroup = GetComponent<CinemachineTargetGroup>();
		targetGroup.AddMember(player.transform, 1, 0);
	}
}
