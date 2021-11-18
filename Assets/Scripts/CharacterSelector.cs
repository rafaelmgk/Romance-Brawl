using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
  //public GameObject player;

  public GameObject[] characters;
  public GameObject characterSelectPanel;

  public void StartGame(int characterChoice)
  {
    //GameObject spawnedPlayer = Instantiate(player) as GameObject;
    GameObject selectedCharacter = Instantiate(characters[characterChoice]) as GameObject;
  }
}
