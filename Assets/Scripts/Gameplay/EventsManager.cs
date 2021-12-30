using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EventsManager : MonoBehaviour {
	[Tooltip("All the transceivers that will receive notifications")]
	[SerializeField] private List<Transceiver> _transceivers = new List<Transceiver>();

	public void OnNotify(Enum notificationType, object actionParams = null) {
		foreach (Transceiver transceiver in _transceivers)
			transceiver.OnNotify(notificationType, actionParams);
	}
}
