using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCam : MonoBehaviour {
	public Transform player;
	void Update() {
		Vector3 newPosition = player.position;
		newPosition.y = transform.position.y;
		newPosition.z = -10;
		transform.position = newPosition;
	}

}
