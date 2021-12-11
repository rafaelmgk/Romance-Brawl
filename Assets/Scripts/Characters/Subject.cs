using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public abstract class Subject : NetworkBehaviour {
	[SerializeField] private List<Observer> _observers = new List<Observer>();

	public void Notify(Enum notification, object value = null) {
		foreach (Observer observer in _observers)
			observer.OnNotify(notification, value);
	}
}
