using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Attack {
	public struct AttackInput {
		public InputAction.CallbackContext context;
		public int attackIndex;

		public AttackInput(InputAction.CallbackContext context, int attackIndex) {
			this.context = context;
			this.attackIndex = attackIndex;
		}
	}

	[Tooltip("Index from which base position the range attack will be calculated")]
	public int attackPointIndex;

	[Tooltip("The range/hitbox of attack")]
	public Vector2 attackRange;

	[Tooltip("The damage dealt on the attack")]
	public int attackPower;

	[Tooltip("Name of the animation attack")]
	public string attackAnimation;

	public Vector3 GetAttackPoint(int index) {
		return GameObject.FindGameObjectsWithTag("Attack Point")[index].transform.position;
	}
}
