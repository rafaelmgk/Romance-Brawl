using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {
	public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn) {
		// GameManager.Instance.id = conn.connectionId;
		// if (!currentPlayers.ContainsKey(GameManager.Instance.id))
		// 	currentPlayers.Add(GameManager.Instance.id, 0);

		return base.OnRoomServerCreateRoomPlayer(conn);
	}

	// public static void SetPlayerTypeRoom(int characterId) {
	// 	if (GameManager.Instance.currentPlayers.ContainsKey(GameManager.Instance.localPlayerIndex))
	// 		GameManager.Instance.currentPlayers[playerId] = characterId;
	// }

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		print(clientIndex);
		int index = GameManager.Instance.currentPlayers[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[index].position,
			Quaternion.identity);

		NetworkServer.AddPlayerForConnection(conn, _temp);

		return _temp;
	}
}
