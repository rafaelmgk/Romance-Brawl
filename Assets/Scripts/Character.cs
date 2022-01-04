using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject {
	[Tooltip("The character name")]
	public string characterName = "";

	[Tooltip("The character animator")]
	public AnimatorOverrideController animator;

	[Tooltip("The character attacks")]
	public List<Attack> attacks = new List<Attack>();
}
