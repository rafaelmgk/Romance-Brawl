using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour {
	public static GameManager Instance;

	[SerializeField] public readonly SyncDictionary<int, int> currentPlayers = new SyncDictionary<int, int>();

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	// public override void OnStartClient() {
	// 	currentPlayers.Callback += OnCurrentPlayersChanged;
	// }

	// void OnCurrentPlayersChanged(SyncDictionary<int, int>.Operation op, int key, int value) {
	// 	currentPlayers.Add(key, value);
	// }
}
