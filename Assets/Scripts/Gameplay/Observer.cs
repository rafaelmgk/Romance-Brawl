using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Observer : MonoBehaviour {
	public abstract void OnNotify(Enum notificationType, object actionParams = null);

	protected abstract bool IsNotificationTypeValid(Enum notificationType);

	private Dictionary<Enum, Action<object>> _ActionsByEnum = new Dictionary<Enum, Action<object>>();

	protected void RegisterAction(Enum key, Action<object> value) {
		if (!_ActionsByEnum.ContainsKey(key))
			_ActionsByEnum.Add(key, value);
	}

	protected void CallAction(Enum notificationType, object actionParams = null) {
		if (_ActionsByEnum.ContainsKey(notificationType))
			_ActionsByEnum[notificationType](actionParams);
	}
}
