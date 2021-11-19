using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterSelector : MonoBehaviour {
	//public GameObject player;

	public GameObject[] characters;
	public GameObject characterSelectPanel;

	public void ChooseCharacter(int characterChoice) {
		GameManager.Instance.choosenCharacter = characters[characterChoice];
	}
}
