using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour {
	[SerializeField] private PlayerBehaviour _playerBehaviour;
	[SerializeField] private Animator _animator;

	private void OnEnable() {
		PlayerBehaviour.CrouchChanged += OnCrouchChanged;
		PlayerBehaviour.JumpChanged += OnJumpChanged;
		PlayerBehaviour.SpeedChanged += OnSpeedChanged;
		PlayerBehaviour.PlayerAttacked += OnPlayerAttacked;
		PlayerBehaviour.PlayerTookDamage += OnPlayerTookDamage;
	}

	private void OnDisable() {
		PlayerBehaviour.CrouchChanged -= OnCrouchChanged;
		PlayerBehaviour.JumpChanged -= OnJumpChanged;
		PlayerBehaviour.SpeedChanged -= OnSpeedChanged;
		PlayerBehaviour.PlayerAttacked -= OnPlayerAttacked;
		PlayerBehaviour.PlayerTookDamage -= OnPlayerTookDamage;
	}

	private bool AuthPlayer(int playerNumber) {
		return (playerNumber == _playerBehaviour.playerNumber);
	}

	private void OnCrouchChanged(PlayerBehaviour playerBehaviour, bool crouchState) {
		if (AuthPlayer(playerBehaviour.playerNumber))
			_animator.SetBool("IsCrouching", crouchState);
	}

	private void OnJumpChanged(PlayerBehaviour playerBehaviour, bool jumpState) {
		if (AuthPlayer(playerBehaviour.playerNumber))
			_animator.SetBool("IsJumping", jumpState);
	}

	private void OnSpeedChanged(PlayerBehaviour playerBehaviour, float speed) {
		if (AuthPlayer(playerBehaviour.playerNumber))
			_animator.SetFloat("Speed", Mathf.Abs(speed));
	}

	private void OnPlayerAttacked(PlayerBehaviour playerBehaviour, string animationState) {
		if (AuthPlayer(playerBehaviour.playerNumber))
			_animator.SetTrigger(animationState);
	}

	private void OnPlayerTookDamage(PlayerBehaviour playerBehaviour) {
		if (AuthPlayer(playerBehaviour.playerNumber))
			_animator.SetTrigger("TakeDamages");
	}
}
