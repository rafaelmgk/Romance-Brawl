using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;

public class NetworkRoomPlayerExt : NetworkRoomPlayer {
	[SerializeField] private GameObject _characterSelection;
	[SerializeField] private TMP_Text _choosenChar;

	public override void OnStartClient() {
		//Debug.Log($"OnStartClient {gameObject}");
	}

	public void HandleSelectionPanelVisibility(bool visibility) {
		_characterSelection.SetActive(visibility);
	}

	private void SendIndex() {
		_characterSelection.SetActive(true);
		CmdSendIndex(index);
	}

	public override void OnStartAuthority() {
		base.OnStartAuthority();

		if (GetComponent<NetworkIdentity>().hasAuthority && index == 0) SendIndex();
	}

	[Command]
	private void CmdSendIndex(int index) {
		if (GameObject.FindGameObjectWithTag("Data") == null) return;
		if (!GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().charactersByPlayer.ContainsKey(index))
			GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().charactersByPlayer.Add(index, 0);
	}

	public void ChooseCharacter(int characterChoice) {
		CmdChooseCharacter(characterChoice);
		_choosenChar.text = DataManager.Instance.characters[characterChoice].name;
	}

	[Command]
	private void CmdChooseCharacter(int characterChoice) {
		if (GameObject.FindGameObjectWithTag("Data") == null) return;
		if (GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().charactersByPlayer.ContainsKey(index))
			GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().charactersByPlayer[index] = characterChoice;
	}

	public override void OnClientEnterRoom() {
		//Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
		if (isLocalPlayer) HandleSelectionPanelVisibility(true);
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
