using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsController : Transceiver {
	private PlayerInput _playerInput;

	public static Vector2 DigitalizeVector2(Vector2 vec) {
		Vector2 vec2 = Vector2.zero;

		if (vec.x < -.5f)
			vec2.x = -1;
		if (vec.x > .5f)
			vec2.x = 1;
		if (vec.y < -.5f)
			vec2.y = -1;
		if (vec.y > .5f)
			vec2.y = 1;

		return vec2;
	}

	private void Start() {
		_playerInput = GetComponent<PlayerInput>();
		// TODO: test auto-switch online
		_playerInput.SwitchCurrentControlScheme(_playerInput.defaultControlScheme);
	}

	public override bool IsNotificationTypeValid(Enum notificationType) {
		return false; // InputsController only sends events, it shouldn't receive
	}

	public void Movement(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerMoved, context);
	}

	public void Jump(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerJumped, context);
	}

	public void BasicAtk(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerAttacked, new Attack.AttackInput(context, 0));
	}

	public void StrongAtk(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerAttacked, new Attack.AttackInput(context, 1));
	}
}
