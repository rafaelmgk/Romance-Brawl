using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	public List<GameObject> currentPlayers = new List<GameObject>();

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void AddNewPlayer(GameObject player) {
		print("teste");
		currentPlayers.Add(player);
	}
}
