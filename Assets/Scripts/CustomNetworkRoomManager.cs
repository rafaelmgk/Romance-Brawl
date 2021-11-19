using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {
	public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn) {
		// if (!currentPlayers.ContainsKey((int)conn.authenticationData))
		// 	currentPlayers.Add((int)conn.authenticationData, 0);

		return base.OnRoomServerCreateRoomPlayer(conn);
	}

	// public static void SetPlayerTypeRoom(int characterId) {
	// 	if (GameManager.Instance.currentPlayers.ContainsKey(GameManager.Instance.localPlayerIndex))
	// 		GameManager.Instance.currentPlayers[playerId] = characterId;
	// }

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		print("teste");
		int index = GameManager.Instance.localChoosenCharacter;

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[index].position,
			Quaternion.identity);

		NetworkServer.AddPlayerForConnection(conn, _temp);

		return _temp;
	}
}
