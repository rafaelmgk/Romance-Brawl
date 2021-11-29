using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	[SerializeField] private TMP_Text player1Health;
	[SerializeField] private TMP_Text player2Health;
	[SerializeField] private TMP_Text player3Health;
	[SerializeField] private TMP_Text player4Health;

    private bool _canUpdateHealth = false;

    public void CanUpdateHealth() {
        _canUpdateHealth = true;
    }

	private void UpdatePlayerHealth(int playerNumber, int health) {
		if (playerNumber == 1)
			player1Health.text = health.ToString() + ".00%";
		else if (playerNumber == 2)
			player2Health.text = health.ToString() + ".00%";
		else if (playerNumber == 3)
			player3Health.text = health.ToString() + ".00%";
		else
			player4Health.text = health.ToString() + ".00%";
	}

	private void Update() {
        // if (_canUpdateHealth) {
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
                PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
                UpdatePlayerHealth(playerBehaviour.playerNumber, playerBehaviour.health);
            }

            // _canUpdateHealth = false;
        // }
	}
}
