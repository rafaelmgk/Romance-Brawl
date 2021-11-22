using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DataManager : NetworkBehaviour {
	public static DataManager Instance;

	[SerializeField] public readonly SyncDictionary<int, int> charactersByPlayer = new SyncDictionary<int, int>();

	public struct WorldBounds {
		public float leftBound;
		public float rightBound;
		public float upBound;
		public float downBound;

		public WorldBounds(float lB, float rB, float uB, float dB) {
			leftBound = lB;
			rightBound = rB;
			upBound = uB;
			downBound = dB;
		}
	}

	public WorldBounds worldBounds = new WorldBounds(-20, 20, 15, -15);

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
