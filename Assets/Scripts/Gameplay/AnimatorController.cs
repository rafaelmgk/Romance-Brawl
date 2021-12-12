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

	public override void OnNotify(Enum notificationType, object actionParams = null) {
		if (IsNotificationTypeValid(notificationType))
			CallAction(notificationType, actionParams);
	}

	protected override bool IsNotificationTypeValid(Enum notificationType) {
		if (notificationType.GetType() == typeof(NotificationType))
			return true;

		return false;
	}
}
