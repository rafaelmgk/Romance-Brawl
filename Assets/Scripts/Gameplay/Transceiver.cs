using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transceiver : MonoBehaviour, ITransmitter, IReceiver {
	[Header("Components Reference")]
	[SerializeField] private EventsManager _eventsManager;

	public abstract bool IsNotificationTypeValid(Enum notificationType);

	public virtual void OnNotify(Enum notificationType, object actionParams = null) {
		CallAction(notificationType, actionParams);
	}

	public virtual void Notify(Enum notificationType, object actionParams = null) {
		_eventsManager.OnNotify(notificationType, actionParams);
	}

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
