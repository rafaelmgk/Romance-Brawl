using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class NetworkController : NetworkBehaviour {

	[Command(requiresAuthority = false)]
	public void CmdUpdateHitPercentageOnServer(PlayerController player, int newHitPercentage) {
		player.hitPercentage = newHitPercentage;
	}

	[Command(requiresAuthority = false)]
	public void CmdAskServerForTakeDamage(PlayerController enemy, int attackDirection, int power) {
		enemy.networkController.TrgtTakeDamage(enemy.gameObject.GetComponent<NetworkIdentity>().connectionToClient,
			attackDirection, power);
	}

	[TargetRpc]
	private void TrgtTakeDamage(NetworkConnection target, int attackDirection, int power) {
		PlayerController playerController = GetComponent<PlayerController>();
		playerController.TakeDamage(attackDirection, power);
	}
}
