using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	[SerializeField] private TMP_Text player1HitPercentage;
	[SerializeField] private TMP_Text player2HitPercentage;
	[SerializeField] private TMP_Text player3HitPercentage;
	[SerializeField] private TMP_Text player4HitPercentage;

    public static bool CanUpdateHitPercentage { get; set; }

	private void UpdatePlayerHitPercentage(int playerNumber, int hitPercentage) {
		if (playerNumber == 1)
			player1HitPercentage.text = hitPercentage.ToString() + ".00%";
		else if (playerNumber == 2)
			player2HitPercentage.text = hitPercentage.ToString() + ".00%";
		else if (playerNumber == 3)
			player3HitPercentage.text = hitPercentage.ToString() + ".00%";
		else
			player4HitPercentage.text = hitPercentage.ToString() + ".00%";
	}

	private void Update() {
        if (CanUpdateHitPercentage) {
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
                PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
				print("n: " + playerBehaviour.playerNumber + ", h: " + playerBehaviour.hitPercentage);
                UpdatePlayerHitPercentage(playerBehaviour.playerNumber, playerBehaviour.hitPercentage);
            }

            CanUpdateHitPercentage = false;
        }
	}
}
