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
			miniCamPosition = respawns[0].GetComponent<MiniCamUIDestroyer>().characterposition;
		}
		if (isClientOnly) {
			miniCamPosition = respawns[1].GetComponent<MiniCamUIDestroyer>().characterposition;
		}
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}
}