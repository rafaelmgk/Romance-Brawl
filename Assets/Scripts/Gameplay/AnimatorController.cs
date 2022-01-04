using System;
using UnityEngine;

public class AnimatorController : Transceiver {
	public enum NotificationType {
		CrouchChanged,
		JumpChanged,
		SpeedChanged,
		PlayerAttacked,
		PlayerTookDamage,
		PlayerFlipped,
		SetAnimator
	}

	[SerializeField] private Animator _animator;

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
		RegisterAction(
			NotificationType.PlayerFlipped,
			(flipState) => OnPlayerFlipped((bool)flipState)
		);
		RegisterAction(
			NotificationType.SetAnimator,
			(animatorControl) => _animator.runtimeAnimatorController = (AnimatorOverrideController)animatorControl
		);
	}

	public override bool IsNotificationTypeValid(Enum notificationType) {
		if (notificationType.GetType() == typeof(NotificationType))
			return true;

		return false;
	}

	private void OnPlayerFlipped(bool flipState) {
		GetComponent<SpriteRenderer>().flipX = flipState;
	}
}
