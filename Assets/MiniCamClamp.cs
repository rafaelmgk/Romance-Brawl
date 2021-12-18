using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamClamp : MonoBehaviour {
	float xMin = -4f;
	float xMax = 4f;
	float yMin = -4.5f;
	float yMax = 0f;

	public Transform player;

	void Start() {

	}

	// Update is called once per frame
	void LateUpdate() {
		transform.rotation = transform.rotation;
		Vector3 pos = player.transform.position;
		pos.x = Mathf.Clamp(pos.x, xMin, xMax);
		pos.y = Mathf.Clamp(pos.y, yMin, yMax);
		transform.position = pos;

	}
}
