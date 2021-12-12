using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamClamp : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void LateUpdate() {
		transform.rotation = transform.rotation;
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		pos.x = Mathf.Clamp01(pos.x);
		pos.y = Mathf.Clamp01(pos.y);
		transform.position = Camera.main.ViewportToWorldPoint(pos);

	}
}
