using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniCam : MonoBehaviour {
	[SerializeField] Transform miniCamPosition;

	Vector3 offset;
	public GameObject[] respawns;
	private void Awake() {
		offset = transform.position - miniCamPosition.position;
	}
	void Update() {
		respawns = GameObject.FindGameObjectsWithTag("Player");
	}
	public void SetMiniCam(int PlayerNumber) {
		miniCamPosition = respawns[PlayerNumber].GetComponent<MiniCamUIDestroyer>().characterposition;
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}
}