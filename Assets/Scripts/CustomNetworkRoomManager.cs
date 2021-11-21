using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkRoomManager : NetworkRoomManager {

	[SerializeField] private GameObject dataManagerPrefab;

	public override void OnStartHost() {
		base.OnStartHost();

		print("teste");
		GameObject dataManagerClone = Instantiate(dataManagerPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(dataManagerClone);
	}

	public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer) {
		int index = GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().playersByCharacters[conn.connectionId];

		GameObject _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[index],
			startPositions[conn.connectionId].position,
			Quaternion.identity);

		GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>().currentPlayers.Add(_temp);
		NetworkServer.AddConnection((NetworkConnectionToClient)conn);

		return _temp;
	}
}
