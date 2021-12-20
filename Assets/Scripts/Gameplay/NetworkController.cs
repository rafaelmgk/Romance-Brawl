using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class NetworkController : NetworkBehaviour {
	[Header("Network Data")]
	[SyncVar(hook = nameof(OnHitPercentageChange))] public int hitPercentage = 0;
	[SyncVar] public int health = 0;
	[SyncVar] public int playerNumber;

	public void OnHitPercentageChange(int oldHitPercentage, int newHitPercentage) {
		UIManager.CanUpdateHitPercentage = true;
	}

	[Command(requiresAuthority = false)]
	public void CmdUpdateHitPercentageOnServer(int newHitPercentage) {
		hitPercentage = newHitPercentage;
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

	[Command(requiresAuthority = false)]
	public void CmdHandleDataManagerOutOfLimitsDictionary(bool outOfLimits, int playerNumber) {
		DataManager dataManager = GameObject.FindWithTag("Data").GetComponent<DataManager>();

		if (dataManager.arePlayersOutOfLimits.ContainsKey(playerNumber))
			CmdModifyDataManagerOutOfLimitsDictionary(dataManager, outOfLimits, playerNumber);
		else
			CmdAddToDataManagerOutOfLimitsDictionary(dataManager, outOfLimits, playerNumber);
	}

	[Command(requiresAuthority = false)]
	private void CmdAddToDataManagerOutOfLimitsDictionary(DataManager dataManager, bool outOfLimits, int playerNumber) {
		dataManager.arePlayersOutOfLimits.Add(playerNumber, outOfLimits);
	}

	[Command(requiresAuthority = false)]
	private void CmdModifyDataManagerOutOfLimitsDictionary(DataManager dataManager, bool outOfLimits, int playerNumber) {
		dataManager.arePlayersOutOfLimits[playerNumber] = outOfLimits;
	}
}
