using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {
	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		int index = GameManager.Instance.currentPlayers[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[index].position,
			Quaternion.identity);

		NetworkServer.AddPlayerForConnection(conn, _temp);

		return _temp;
	}
}
