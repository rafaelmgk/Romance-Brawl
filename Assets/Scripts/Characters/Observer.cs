using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Observer : MonoBehaviour {
	public abstract void OnNotify(Enum notification, object value = null);
}
