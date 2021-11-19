using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CharacterSelector : MonoBehaviour {
	//public GameObject player;

	public GameObject[] characters;
	public GameObject characterSelectPanel;
	public TMP_Text choosenChar;

	public void ChooseCharacter(int characterChoice) {
		NetworkManager.singleton.playerPrefab = characters[characterChoice];
		choosenChar.text = characters[characterChoice].name;
	}
}
