using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class NetworkRoomPlayerExt : NetworkRoomPlayer {
	public GameObject characterSelection;

	public GameObject[] characters;
	public TMP_Text choosenChar;

	private int _playerIndex;

	public override void OnStartClient() {
		//Debug.Log($"OnStartClient {gameObject}");
		if (isLocalPlayer) {
			characterSelection.SetActive(true);
			// CmdSendIndex(index);
			_playerIndex = index;
		}
	}

	// [Command]
	// private void CmdSendIndex(int index) {
	// 	if (GameManager.Instance == null) return;
	// 	if (!GameManager.Instance.currentPlayers.ContainsKey(index))
	// 		GameManager.Instance.currentPlayers.Add(index, 0);
	// }

	public void ChooseCharacter(int characterChoice) {
		// CmdChooseCharacter(characterChoice);
		choosenChar.text = characters[characterChoice].name;
	}

	// [Command]
	// private void CmdChooseCharacter(int characterChoice) {
	// 	if (GameManager.Instance == null) return;
	// 	if (GameManager.Instance.currentPlayers.ContainsKey(_playerIndex))
	// 		GameManager.Instance.currentPlayers[_playerIndex] = characterChoice;
	// }

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
