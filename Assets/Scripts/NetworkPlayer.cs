using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour {
	[SerializeField] public readonly SyncDictionary<int, int> playersByConnection = new SyncDictionary<int, int>();

	// private void Start() {
	// 	playersByConnection.Callback += HandlePlayersNumbers;
	// }

	// private void HandlePlayersNumbers(SyncDictionary<int, int>.Operation op, int key, int value) {
	// 	if (SyncDictionary<int, int>.Operation.OP_ADD == op)
	// 		value++;
	// }
}
