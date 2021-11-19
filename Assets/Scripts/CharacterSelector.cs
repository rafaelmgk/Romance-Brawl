using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CharacterSelector : NetworkBehaviour {
	//public GameObject player;

	public GameObject[] characters;
	public GameObject characterSelectPanel;
	public TMP_Text choosenChar;

	public void ChooseCharacter(int characterChoice) {
		// CmdChooseCharacter(characterChoice);
		GameManager.Instance.localChoosenCharacter = characterChoice;

		choosenChar.text = characters[characterChoice].name;
	}

	// [Command]
	// private void CmdChooseCharacter(int characterChoice) {
	// 	CustomNetworkRoomManager.SetPlayerTypeRoom(connectionToClient, characterChoice);
	// }
}
