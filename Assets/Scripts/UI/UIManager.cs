using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class UIManager : NetworkBehaviour {
	public static bool CanUpdateHitPercentage { get; set; }

	[SerializeField] public readonly SyncList<UIPlayerStats> PlayersStats = new SyncList<UIPlayerStats>();
	private Dictionary<int, GameObject> PlayersStatsByPlayersNumbers = new Dictionary<int, GameObject>();

	[SerializeField] private Transform grid;
	[SerializeField] private GameObject playerStatsPrefab;

	private void UpdatePlayerHitPercentage(int playerNumber, int hitPercentage) {
		foreach (UIPlayerStats stat in PlayersStats)
			if (stat.playerNumber == playerNumber) {
				PlayersStatsByPlayersNumbers[playerNumber].transform.GetChild(1)
					.GetComponent<TMP_Text>().text = hitPercentage.ToString() + ".00%";
				break;
			}
	}

	private void CreatePlayersStats() {
		foreach (UIPlayerStats stat in PlayersStats) {
			GameObject playersStatsClone = Instantiate(playerStatsPrefab);
			PlayersStatsByPlayersNumbers.Add(stat.playerNumber, playersStatsClone);

			playersStatsClone.transform.GetChild(0).GetComponent<TMP_Text>().text = stat.character;
			playersStatsClone.transform.GetChild(1).GetComponent<TMP_Text>().text = stat.hitPercentage;
			playersStatsClone.transform.GetChild(2).GetComponent<TMP_Text>().text = stat.health;
			playersStatsClone.transform.SetParent(grid);
		}
	}

	private void Start() {
		CreatePlayersStats();
	}

	private void Update() {
		if (CanUpdateHitPercentage) {
			foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
				PlayerController playerController = player.GetComponent<PlayerController>();
				UpdatePlayerHitPercentage(playerController.playerNumber, playerController.hitPercentage);
			}

			CanUpdateHitPercentage = false;
		}
	}
}
