using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamClamp : MonoBehaviour {
	float xMin = -4.5f;//TODO do it automatically and relative
	float xMax = 3.5f;//TODO do it automatically and relative
	float yMin = -4.5f;//TODO do it automatically and relative
	float yMax = 0f;//TODO do it automatically and relative

	public Transform player;
	private Quaternion my_rotation;

	void Start() {
		my_rotation = this.transform.rotation;
	}
	private void Update() {
		this.transform.rotation = my_rotation;
	}

	// Update is called once per frame
	void LateUpdate() {

		Vector3 pos = player.transform.position;
		pos.x = Mathf.Clamp(pos.x, xMin, xMax);
		pos.y = Mathf.Clamp(pos.y, yMin, yMax);
		transform.position = pos;

	}
}
