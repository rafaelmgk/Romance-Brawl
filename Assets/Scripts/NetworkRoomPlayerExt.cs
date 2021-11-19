using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkRoomPlayerExt : NetworkRoomPlayer {
	public override void OnStartClient() {
		//Debug.Log($"OnStartClient {gameObject}");
		// if (isLocalPlayer) {
		// 	GameManager.Instance.localPlayerIndex = index;
		// 	if (!GameManager.Instance.currentPlayers.ContainsKey(index))
		// 		GameManager.Instance.currentPlayers.Add(index, 0);
		// }
	}

	public override void OnClientEnterRoom() {
		//Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
	}

	public override void OnClientExitRoom() {
		//Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
	}

	public override void IndexChanged(int oldIndex, int newIndex) {
		//Debug.Log($"IndexChanged {newIndex}");
	}

	public override void ReadyStateChanged(bool oldReadyState, bool newReadyState) {
		//Debug.Log($"ReadyStateChanged {newReadyState}");
	}

	public override void OnGUI() {
		base.OnGUI();
	}
}
