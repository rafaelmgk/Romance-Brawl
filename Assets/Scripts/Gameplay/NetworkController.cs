using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class NetworkController : NetworkBehaviour {
	[Header("Components Reference")]
	[SerializeField] private PlayerController _playerController;

	[Header("Network Data")]
	[SyncVar(hook = nameof(OnHitPercentageChange))] public int hitPercentage = 0;
	[SyncVar] public int health = 0;
	[SyncVar] public int playerNumber;
	[SyncVar(hook = nameof(OnFlipChange))] public bool isFacingLeft = false;

	// TODO: refactor how this is working
	private void Start() {
		if (!isLocalPlayer)
			Destroy(transform.GetChild(3).gameObject);
	}

	[Command(requiresAuthority = false)]
	public void CmdUpdateHitPercentageOnServer(int newHitPercentage) {
		hitPercentage = newHitPercentage;
	}

	public void OnHitPercentageChange(int oldHitPercentage, int newHitPercentage) {
		UIManager.CanUpdateHitPercentage = true;
	}

	[Command(requiresAuthority = false)]
	public void CmdAskServerForTakeDamage(NetworkController enemy, int attackDirection, int power) {
		enemy.TrgtTakeDamage(enemy.gameObject.GetComponent<NetworkIdentity>().connectionToClient,
			attackDirection, power);
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
	public void CmdUpdateFlipOnServer(bool newState) {
		isFacingLeft = newState;
	}

	public void OnFlipChange(bool oldState, bool newState) {
		_playerController.Notify(AnimatorController.NotificationType.PlayerFlipped, isFacingLeft);
	}

	[TargetRpc]
	private void TrgtTakeDamage(NetworkConnection target, int attackDirection, int power) {
		_playerController.TakeDamage(attackDirection, power);
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
