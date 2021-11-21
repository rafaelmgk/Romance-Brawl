using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Destroyer : NetworkBehaviour {
	private void OnTriggerEnter2D(Collider2D other) {
		other.gameObject.SetActive(false);
		StartCoroutine(WaitAndRespawn(other.gameObject));
	}

	private IEnumerator WaitAndRespawn(GameObject other) {
		other.transform.position = new Vector3(0, 2, 0);

		yield return new WaitForSeconds(1f);
		other.SetActive(true);
	}
}