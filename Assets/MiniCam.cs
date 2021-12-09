using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniCam : NetworkBehaviour {
	[SerializeField] Transform miniCamPosition;
	Vector3 offset;

	private void Awake() {
		offset = transform.position - miniCamPosition.position;
	}
	private void Update() {
		if (GameObject.FindGameObjectWithTag("Player")) {
			if (GameObject.FindGameObjectWithTag("Player").GetComponent<MiniCamUIDestroyer>().characterposition == null) {
				return;
			} else { miniCamPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<MiniCamUIDestroyer>().characterposition; }
		}
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}

}