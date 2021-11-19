using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {
	Dictionary<int, int> currentPlayers;
	public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn) {
		if (!currentPlayers.ContainsKey(conn.connectionId))
			currentPlayers.Add(conn.connectionId, 0);

		return base.OnRoomServerCreateRoomPlayer(conn);
	}

	public void SetPlayerTypeRoom(NetworkConnection conn, int _type) {
		if (currentPlayers.ContainsKey(conn.connectionId))
			currentPlayers[conn.connectionId] = _type;
	}

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		int index = currentPlayers[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[conn.connectionId].position,
			Quaternion.identity);

		NetworkServer.AddPlayerForConnection(conn, _temp);

		return _temp;
	}
}
