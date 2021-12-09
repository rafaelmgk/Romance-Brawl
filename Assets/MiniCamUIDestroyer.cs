using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniCamUIDestroyer : NetworkBehaviour {
	[SerializeField] GameObject miniCamUI;
	[SerializeField] GameObject character;
	public Transform characterposition;
	private void Start() {
		if (!isLocalPlayer) {
			Destroy(miniCamUI);
			return;
		}
		characterposition = character.transform;

	}
}