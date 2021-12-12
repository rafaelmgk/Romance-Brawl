using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LightReflectiveMirror;
using Mirror;
using TMPro;

public class ServerListLRM : MonoBehaviour {

	public Transform scrollParent;
	public GameObject serverEntry;

	private LightReflectiveMirrorTransport _LRM;

	private void Start() {
		if (_LRM == null)
			_LRM = (LightReflectiveMirrorTransport)Transport.activeTransport;
	}

	public void RefreshServerList() {
		_LRM.RequestServerList();
	}

	public void ServerListUpdate() {
		foreach (Transform t in scrollParent)
			Destroy(t.gameObject);

		for (int i = 0; i < _LRM.relayServerList.Count; i++) {
			var newEntry = Instantiate(serverEntry, scrollParent);
			newEntry.transform.GetChild(0).GetComponent<TMP_Text>().text = _LRM.relayServerList[i].serverName;

			string serverId = _LRM.relayServerList[i].serverId;
			newEntry.GetComponent<Button>().onClick.AddListener(() => ConnectToServer(serverId));
		}
	}

	private void ConnectToServer(string serverId) {
		NetworkManager.singleton.networkAddress = serverId;
		NetworkManager.singleton.StartClient();
	}
}
