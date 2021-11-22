using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DataManager : NetworkBehaviour {
	public static DataManager Instance;

	[SerializeField] public readonly SyncDictionary<int, int> charactersByPlayer = new SyncDictionary<int, int>();

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
