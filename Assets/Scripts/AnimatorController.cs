using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimatorController : Observer {
	public enum NotificationType {
		CrouchChanged,
		JumpChanged,
		SpeedChanged,
		PlayerAttacked,
		PlayerTookDamage
	}

	[SerializeField] private Animator _animator;

	private Dictionary<Enum, Action<object>> _ActionByEnum = new Dictionary<Enum, Action<object>>();

	private void Awake() {
		RegisterAction(
			NotificationType.CrouchChanged,
			(crouchState) => _animator.SetBool("IsCrouching", (bool)crouchState)
		);
		RegisterAction(
			NotificationType.JumpChanged,
			(jumpState) => _animator.SetBool("IsJumping", (bool)jumpState)
		);
		RegisterAction(
			NotificationType.SpeedChanged,
			(speed) => _animator.SetFloat("Speed", Mathf.Abs((float)speed))
		);
		RegisterAction(
			NotificationType.PlayerAttacked,
			(animationState) => _animator.SetTrigger((string)animationState)
		);
		RegisterAction(
			NotificationType.PlayerTookDamage,
			(any) => _animator.SetTrigger("TakeDamages")
		);
	}

	public override void OnNotify(Enum notification, object value = null) {
		if (_ActionByEnum.ContainsKey(notification))
			_ActionByEnum[notification](value);
	}

	private void RegisterAction(Enum key, Action<object> value) {
		if (!_ActionByEnum.ContainsKey(key))
			_ActionByEnum.Add(key, value);
	}
}
