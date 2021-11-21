using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		int index = SelectionManager.Instance.playersByCharacters[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[conn.connectionId].position,
			Quaternion.identity);

		SelectionManager.Instance.currentPlayers.Add(_temp);
		NetworkServer.AddConnection((NetworkConnectionToClient)conn);

		return _temp;
	}
}
