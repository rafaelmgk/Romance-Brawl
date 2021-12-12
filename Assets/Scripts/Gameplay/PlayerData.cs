using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class PlayerData {
	[Tooltip("Amount of force added when the player jumps.")]
	public float jumpForce = 400f;

	[Tooltip("Amount of maxSpeed applied to crouching movement. 1 = 100%")]
	[Range(0, 1)] public float crouchSpeed = .36f;

	[Tooltip("How much to smooth out the movement.")]
	[Range(0, .3f)] public float movementSmoothing = .05f;

	[Tooltip("Whether or not a player can steer while jumping.")]
	public bool airControl = false;
}
