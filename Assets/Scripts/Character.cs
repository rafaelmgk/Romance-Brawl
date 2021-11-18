using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{

  public string characterName = "Default";

  public GameObject characterObject;
  public Rigidbody2D hitBoX;
  public int health = 0;
}
