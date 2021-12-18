using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transceiver : MonoBehaviour, ITransmitter, IReceiver {
	public abstract void OnNotify(Enum notificationType, object actionParams = null);

	public abstract bool IsNotificationTypeValid(Enum notificationType);

	// The transceiver is obligated to receiver, but can choose to transmit
	public virtual void Notify(Enum notificationType, object actionParams = null) { }

	private Dictionary<Enum, Action<object>> _ActionsByEnum = new Dictionary<Enum, Action<object>>();

	protected void RegisterAction(Enum key, Action<object> value) {
		if (IsNotificationTypeValid(key) && !_ActionsByEnum.ContainsKey(key))
			_ActionsByEnum.Add(key, value);
	}

	protected void CallAction(Enum notificationType, object actionParams = null) {
		if (IsNotificationTypeValid(notificationType) && _ActionsByEnum.ContainsKey(notificationType))
			_ActionsByEnum[notificationType](actionParams);
	}
}
