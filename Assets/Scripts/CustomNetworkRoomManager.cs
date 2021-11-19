using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {
	public static Dictionary<int, int> currentPlayers;
	private NetworkConnection _currentConnection;
	private struct AuthData {
		public int connectionId;
	}

	public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn) {
		// if (!currentPlayers.ContainsKey((int)conn.authenticationData))
		// 	currentPlayers.Add((int)conn.authenticationData, 0);

		return base.OnRoomServerCreateRoomPlayer(conn);
	}

	public static void SetPlayerTypeRoom(NetworkConnection conn, int _type) {
		if (currentPlayers.ContainsKey(conn.connectionId))
			currentPlayers[conn.connectionId] = _type;
	}

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		print("teste");
		int index = currentPlayers[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[conn.connectionId].position,
			Quaternion.identity);

		NetworkServer.AddPlayerForConnection(conn, _temp);

		return _temp;
	}
}
