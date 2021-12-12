using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Subject : NetworkBehaviour {
	[SerializeField] private List<Observer> _observers = new List<Observer>();

	protected void Notify(Enum notificationType, object actionParams = null) {
		foreach (Observer observer in _observers)
			observer.OnNotify(notificationType, actionParams);
	}
}
