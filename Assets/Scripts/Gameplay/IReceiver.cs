using System;

public interface IReceiver {
	void OnNotify(Enum notificationType, object actionParams = null);
	bool IsNotificationTypeValid(Enum notificationType);
}
