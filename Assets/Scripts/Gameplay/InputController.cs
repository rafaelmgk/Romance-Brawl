using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, ITransmitter {
	[SerializeField] private NetworkTransceiver transceiver;

	private PlayerInput _playerInput;

	private void Start() {
		_playerInput = GetComponent<PlayerInput>();
		// TODO: test auto-switch online
		_playerInput.SwitchCurrentControlScheme(_playerInput.defaultControlScheme);
	}

	public void Notify(Enum notificationType, object actionParams = null) {
		transceiver.OnNotify(notificationType, actionParams);
	}

	public void Movement(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerMoved, context);
	}

	public void Jump(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerJumped, context);
	}

	public void BasicAtk(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerBasicAttacked, context);
	}

	public void StrongAtk(InputAction.CallbackContext context) {
		Notify(PlayerController.NotificationType.PlayerStrongAttacked, context);
	}
}
