using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {

	[SerializeField] private GameObject dataManagerPrefab;
	[SerializeField] private GameObject uiManagerPrefab;

	private List<UIPlayerStats> stats = new List<UIPlayerStats>();

	private int _playerCounter = 1;

	public override void OnStartHost() {
		base.OnStartHost();

		GameObject dataManagerClone = Instantiate(dataManagerPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(dataManagerClone);
	}

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		int index = GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().charactersByPlayer[conn.connectionId];

		GameObject _temp = Instantiate(
			spawnPrefabs[index],
			startPositions[conn.connectionId].position,
			Quaternion.identity
		);
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
		foreach (UIPlayerStats stat in stats)
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

		stats.Add(newPlayerStats);
	}
}
