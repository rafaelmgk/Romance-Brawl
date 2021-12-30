using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksController : Transceiver {
	public enum NotificationType {
		PlayerFlipped
	}

	private void Awake() {
		RegisterAction(
			NotificationType.PlayerFlipped,
			(flipState) => transform.Rotate(0f, 180f, 0f)
		);
	}

	public override bool IsNotificationTypeValid(Enum notificationType) {
		if (notificationType.GetType() == typeof(NotificationType))
			return true;

		return false;
	}
}
