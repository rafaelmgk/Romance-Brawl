using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectionManager : NetworkBehaviour {
	public static SelectionManager Instance;

	[SerializeField] public readonly SyncDictionary<int, int> playersByCharacters = new SyncDictionary<int, int>();
	[SerializeField] public readonly SyncList<GameObject> currentPlayers = new SyncList<GameObject>();

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		currentPlayers.Callback += HandleCurrentPlayers;
	}

	private void HandleCurrentPlayers(SyncList<GameObject>.Operation op, int index, GameObject oldItem, GameObject newItem) {
		if (SyncList<GameObject>.Operation.OP_ADD == op)
			GameManager.Instance.AddNewPlayer(newItem);
			// if (GameObject.Find("TargetGroup"))
			// 	GameObject.Find("TargetGroup").GetComponent<DynamicTargetGroup>().AddMemberToTargetGroup(newItem);
	}

	[ClientRpc]
	private void CmdAddNewPlayer(GameObject player) {
		GameManager.Instance.AddNewPlayer(player);
	}
}
