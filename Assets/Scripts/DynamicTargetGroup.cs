using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class DynamicTargetGroup : MonoBehaviour {
	private void Start() {
		// StartCoroutine(ReadyGo());
	}

	public void ReadyGoCoroutine() {

	}

	private IEnumerator ReadyGo() {
		yield return new WaitForSeconds(1.5f);
		// foreach(GameObject player in GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().currentPlayers)
		// 	AddMemberToTargetGroup(player);
	}

	public void AddMemberToTargetGroup(GameObject player) {
		StartCoroutine(ReadyGo());
		CinemachineTargetGroup targetGroup = GetComponent<CinemachineTargetGroup>();
		targetGroup.AddMember(player.transform, 1, 0);
	}
}
