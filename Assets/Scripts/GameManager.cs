using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	public Dictionary<int, int> currentPlayers;
	public int localPlayerIndex;
	public int localChoosenCharacter;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
