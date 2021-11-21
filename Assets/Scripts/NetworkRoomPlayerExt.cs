using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;
using TMPro;

public class NetworkRoomPlayerExt : NetworkRoomPlayer {
	public GameObject characterSelection;

	public GameObject[] characters;
	public TMP_Text choosenChar;

	public override void OnStartClient() {
		//Debug.Log($"OnStartClient {gameObject}");
	}

	public void HandleSelectionPanelVisibility(bool visibility) {
		characterSelection.SetActive(visibility);
	}

	private void SendIndex() {
		characterSelection.SetActive(true);
		CmdSendIndex(index);
	}

	public override void OnStartAuthority() {
		base.OnStartAuthority();

		if (GetComponent<NetworkIdentity>().hasAuthority && index == 0) SendIndex();
	}

	[Command]
	private void CmdSendIndex(int index) {
		if (SelectionManager.Instance == null) return;
		if (!SelectionManager.Instance.playersByCharacters.ContainsKey(index))
			SelectionManager.Instance.playersByCharacters.Add(index, 0);
	}

	public void ChooseCharacter(int characterChoice) {
		CmdChooseCharacter(characterChoice);
		choosenChar.text = characters[characterChoice].name;
	}

	[Command]
	private void CmdChooseCharacter(int characterChoice) {
		if (SelectionManager.Instance == null) return;
		if (SelectionManager.Instance.playersByCharacters.ContainsKey(index))
			SelectionManager.Instance.playersByCharacters[index] = characterChoice;
	}

	public override void OnClientEnterRoom() {
		//Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
	}

	public override void OnClientExitRoom() {
		//Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
	}

	public override void IndexChanged(int oldIndex, int newIndex) {
		//Debug.Log($"IndexChanged {newIndex}");
		if (GetComponent<NetworkIdentity>().hasAuthority) SendIndex();
	}

	public override void ReadyStateChanged(bool oldReadyState, bool newReadyState) {
		//Debug.Log($"ReadyStateChanged {newReadyState}");
		if (isLocalPlayer) HandleSelectionPanelVisibility(!readyToBegin);
	}

	public override void OnGUI() {
		base.OnGUI();
	}
}
