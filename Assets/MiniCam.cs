using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCam : MonoBehaviour {
	[SerializeField] Transform miniCamPosition;
	Vector3 offset;

	private void Awake() {
		offset = transform.position - miniCamPosition.position;
	}
	private void Update() {
		if (GameObject.FindGameObjectWithTag("Player")) {
			miniCamPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<MiniCamUIDestroyer>().characterposition;
		}
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}

}