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
		if (isServer) NetworkManager.singleton.playerPrefab = characters[characterChoice];
		else if (isClient) CmdChooseCharacter(characters[characterChoice]);

		choosenChar.text = characters[characterChoice].name;
	}

	[Command]
	private void CmdChooseCharacter(GameObject prefab) {
		NetworkManager.singleton.playerPrefab = prefab;
	}
}
