using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {

	[SerializeField] private GameObject dataManagerPrefab;
	[SerializeField] private GameObject uiManagerPrefab;

	private List<UIPlayerStats> _stats = new List<UIPlayerStats>();
	private int _playerCounter = 1;

	public override void OnStartHost() {
		base.OnStartHost();

		GameObject dataManagerClone = Instantiate(dataManagerPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(dataManagerClone);
	}

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		GameObject _temp = Instantiate(
			playerPrefab,
			startPositions[conn.connectionId].position,
			Quaternion.identity
		);

		int index = DataManager.Instance.charactersByPlayer[conn.connectionId];
		_temp.GetComponent<PlayerController>().SetAnimator(DataManager.Instance.characters[index].animator);
		_temp.GetComponent<PlayerController>().attacks = DataManager.Instance.characters[index].attacks;

		_temp.GetComponent<NetworkController>().playerNumber = _playerCounter;
		_playerCounter++;

		NetworkServer.AddConnection((NetworkConnectionToClient)conn);

		return _temp;
	}

	public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer) {
		CreatePlayerStats(gamePlayer);
		if (gamePlayer.GetComponent<NetworkController>().playerNumber == numPlayers)
			CreateUIManager();

		return true;
	}

	private void CreateUIManager() {
		GameObject uiManagerClone = Instantiate(uiManagerPrefab, Vector3.zero, Quaternion.identity);
		foreach (UIPlayerStats stat in _stats)
			uiManagerClone.GetComponent<UIManager>().PlayersStats.Add(stat);
		NetworkServer.Spawn(uiManagerClone);
	}

	private void CreatePlayerStats(GameObject gamePlayer) {
		NetworkController networkController = gamePlayer.GetComponent<NetworkController>();
		UIPlayerStats newPlayerStats = new UIPlayerStats(
			networkController.playerNumber,
			"Player " + networkController.playerNumber,
			networkController.hitPercentage,
			networkController.health
		);

		_stats.Add(newPlayerStats);
	}
}
