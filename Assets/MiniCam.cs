using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniCam : NetworkBehaviour {
	[SerializeField] Transform miniCamPosition;
	Vector3 offset;
	public GameObject[] respawns;
	private void Awake() {
		offset = transform.position - miniCamPosition.position;
	}
	void Update() {

		respawns = GameObject.FindGameObjectsWithTag("Player");
		if (isServer) {
			miniCamPosition = respawns[0].GetComponent<Transform>();
		}
		if (isClientOnly) {
			if (respawns.Length >= 1) {
				for (int i = 1; i < respawns.Length; i++) {
					miniCamPosition = respawns[i].GetComponent<Transform>();
				}
			}
		}
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}
}