using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerController : Physics {
	public enum NotificationType {
		PlayerMoved,
		PlayerJumped,
		PlayerBasicAttacked,
		PlayerStrongAttacked,
	}

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	public bool crouch = false;

	public Transform attackPoint;
	public Vector2 attack1Range;
	public Vector2 attack2Range;
	public LayerMask enemyLayers;
	public int attackDirection;

	public int atk1Power;
	public int atk2Power;

	public Rigidbody2D hitBox;
	[SyncVar] public int hitPercentage = 0;
	[SyncVar] public int health = 0;

	public NetworkConnectionToClient enemyConnection;

	[SyncVar] public int playerNumber;

	private bool _canAttack = true;
	private bool _canRespawn = true;

	private Vector2 _movementVector;
	[Range(0, 1)] private float stunTime = 0f;
	[Range(0, 1)] private float stunTimer = 1f;

	[SerializeField] private List<Transceiver> _transceivers = new List<Transceiver>();

	private void OnEnable() {
		OutOfWorldBounds += OnOutOfWorldBounds;
		OutOfCameraLimits += OnOutOfCameraLimits;
	}

	private void OnDisable() {
		OutOfWorldBounds -= OnOutOfWorldBounds;
		OutOfCameraLimits -= OnOutOfCameraLimits;
	}

	private void Awake() {
		RegisterAction(
			NotificationType.PlayerMoved,
			(context) => OnPlayerMoved((InputAction.CallbackContext)context)
		);
		RegisterAction(
			NotificationType.PlayerJumped,
			(context) => OnPlayerJumped((InputAction.CallbackContext)context)
		);
		RegisterAction(
			NotificationType.PlayerBasicAttacked,
			(context) => OnPlayerBasicAttacked((InputAction.CallbackContext)context)
		);
		RegisterAction(
			NotificationType.PlayerStrongAttacked,
			(context) => OnPlayerStrongAttacked((InputAction.CallbackContext)context)
		);
	}

	private void Start() {
		if (!isLocalPlayer)
			Destroy(transform.GetChild(3).gameObject); // TODO: change this
	}

	public override void OnNotify(Enum notificationType, object actionParams = null) {
		CallAction(notificationType, actionParams);
	}

	public override bool IsNotificationTypeValid(Enum notificationType) {
		if (notificationType.GetType() == typeof(NotificationType))
			return true;

		return false;
	}

	public override void Notify(Enum notificationType, object actionParams = null) {
		foreach (Transceiver transceiver in _transceivers)
			transceiver.OnNotify(notificationType, actionParams);
	}

	private void OnPlayerMoved(InputAction.CallbackContext context) {
		// if (!isLocalPlayer) return;

		_movementVector = context.ReadValue<Vector2>();
		if (Gamepad.current != null)
			_movementVector = DigitalizeVector2(_movementVector);

		if ((context.started || context.performed) && _movementVector.y < 0f)
			Crouch(true);
		if (context.canceled || _movementVector.y >= 0f)
			Crouch(false);
	}

	private Vector2 DigitalizeVector2(Vector2 vec) {
		Vector2 vec2 = Vector2.zero;

		if (vec.x < -.5f)
			vec2.x = -1;
		if (vec.x > .5f)
			vec2.x = 1;
		if (vec.y < -.5f)
			vec2.y = -1;
		if (vec.y > .5f)
			vec2.y = 1;

		return vec2;
	}

	private void Crouch(bool crouchState) {
		IgnorePlatformCollision(crouchState);
		crouch = crouchState;

		Notify(AnimatorController.NotificationType.CrouchChanged, crouchState);
	}

	private void IgnorePlatformCollision(bool ignore = true) {
		Physics2D.IgnoreLayerCollision(3, 8, ignore);
	}

	public void OnPlayerJumped(InputAction.CallbackContext context) {
		// if (!isLocalPlayer) return;

		if (context.started && stunTime == 0)
			IgnorePlatformCollisionAndJump();
		else if (context.canceled)
			IgnorePlatformCollision(false);
	}

	private void IgnorePlatformCollisionAndJump() {
		IgnorePlatformCollision();
		DoJump();

		InvokeJumpEvent(true);
	}

	public void InvokeJumpEvent(bool jumpState) {
		Notify(AnimatorController.NotificationType.JumpChanged, jumpState);
	}

	public void OnPlayerBasicAttacked(InputAction.CallbackContext context) {
		// if (!isLocalPlayer) return;

		if (context.started && _canAttack && !crouch) {
			Attack(attackPoint, attack1Range, "Attack", atk1Power);
			StartCoroutine(WaitForAttackAgain());
		}
	}

	public void OnPlayerStrongAttacked(InputAction.CallbackContext context) {
		// if (!isLocalPlayer) return;

		if (context.started && _canAttack && !crouch) {
			Attack(attackPoint, attack2Range, "Attack2", atk2Power);
			StartCoroutine(WaitForAttackAgain());
		}
	}

	private IEnumerator WaitForAttackAgain() {
		yield return new WaitForSeconds(0.5f);
		_canAttack = true;
	}

	void Update() {
		if (!isLocalPlayer) return;
		AttackDirection();

		int crouchModifier = 1;
		if (crouch)
			crouchModifier = 0;
		horizontalMove = _movementVector.x * runSpeed * crouchModifier;

		// TODO: Move only when receive an move input event
		if ((Keyboard.current.anyKey.IsPressed() || AreAnyGamepadButtonsPressed()) && !crouch && stunTime == 0)
			Move(horizontalMove * Time.fixedDeltaTime, crouch);

		Notify(AnimatorController.NotificationType.SpeedChanged, horizontalMove);

		if (stunTime > 0) {
			stunTime -= stunTimer * Time.fixedDeltaTime;
			if (stunTime < 0)
				stunTime = 0;
		}
	}

	void Attack(Transform attackPoint, Vector2 attackRange, string animation, int attackPower) {
		_canAttack = false;

		Notify(AnimatorController.NotificationType.PlayerAttacked, animation);

		Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, enemyLayers);
		foreach (Collider2D enemy in hitEnemies) {
			if (enemy != gameObject.GetComponent<Collider2D>())
				AskServerForTakeDamage(enemy.gameObject, attackDirection, attackPower);
		}
	}

	private void AskServerForTakeDamage(GameObject enemy, int attackDirection, int firstAtkPower) {
		enemy.GetComponent<PlayerController>().CmdAskServerForTakeDamage(enemy, attackDirection, firstAtkPower);
	}


	[Command(requiresAuthority = false)]
	public void CmdAskServerForTakeDamage(GameObject enemy, int attackDirection, int power) {
		enemy.GetComponent<PlayerController>().TrgtTakeDamage(
		  enemy.GetComponent<NetworkIdentity>().connectionToClient, attackDirection, power
		);
	}

	[TargetRpc]
	public void TrgtTakeDamage(NetworkConnection target, int attackDirection, int power) {
		int atkPower = power;
		if (crouch)
			return;

		hitPercentage += atkPower;
		UpdateHitPercentage(hitPercentage);

		hitBox.AddForce(new Vector2(attackDirection * hitPercentage, hitPercentage / 3.5f), ForceMode2D.Impulse);

		Notify(AnimatorController.NotificationType.PlayerTookDamage);

		stunTime = 1f;
	}

	private void UpdateHitPercentage(int newHitPercentage) {
		UIManager.CanUpdateHitPercentage = true;
		if (isClientOnly) CmdUpdateHitPercentageOnServer(newHitPercentage);
		if (isServer) CmdUpdateHitPercentageOnAllClients();
	}

	[Command(requiresAuthority = false)]
	private void CmdUpdateHitPercentageOnServer(int newHitPercentage) {
		hitPercentage = newHitPercentage;
		UIManager.CanUpdateHitPercentage = true;
	}

	[ClientRpc]
	private void CmdUpdateHitPercentageOnAllClients() {
		UIManager.CanUpdateHitPercentage = true;
	}

	public void AttackDirection() {
		if (_movementVector.x != 0)
			attackDirection = (int)_movementVector.x;
	}

	private bool AreAnyGamepadButtonsPressed() {
		if (Gamepad.current == null) return false;

		for (int i = 0; i < Gamepad.current.allControls.Count; i++) {
			if (Gamepad.current.allControls[i].IsPressed()) return true;
		}

		return false;
	}

	private void OnOutOfWorldBounds() {
		if (_canRespawn)
			StartCoroutine(WaitAndRespawn(gameObject));
	}

	private IEnumerator WaitAndRespawn(GameObject player) {
		_canRespawn = false;

		player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		player.GetComponent<PlayerController>().hitPercentage = 0;
		UpdateHitPercentage(0);
		GameObject map = GameObject.FindWithTag("Map");
		player.transform.position = (map != null ? map.GetComponent<Renderer>().bounds.center : Vector3.zero);

		yield return new WaitForSeconds(1f);
		// player.SetActive(true);
		_canRespawn = true;
	}

	private void OnOutOfCameraLimits(bool outOfLimits) {
		CmdHandleDataManagerOutOfLimitsDictionary(outOfLimits);
	}

	[Command(requiresAuthority = false)]
	private void CmdHandleDataManagerOutOfLimitsDictionary(bool boolean) {
		DataManager dataManager = GameObject.FindWithTag("Data").GetComponent<DataManager>();

		if (isServer) {
			if (dataManager.arePlayersOutOfLimits.ContainsKey(playerNumber))
				ModifyDataManagerOutOfLimitsDictionary(dataManager, boolean);
			else
				AddToDataManagerOutOfLimitsDictionary(dataManager, boolean);

			return;
		}

		if (dataManager.arePlayersOutOfLimits.ContainsKey(playerNumber))
			CmdModifyDataManagerOutOfLimitsDictionary(dataManager, boolean);
		else
			CmdAddToDataManagerOutOfLimitsDictionary(dataManager, boolean);
	}

	private void AddToDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean) {
		dataManager.arePlayersOutOfLimits.Add(playerNumber, boolean);
	}

	private void ModifyDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean) {
		dataManager.arePlayersOutOfLimits[playerNumber] = boolean;
	}

	[Command(requiresAuthority = false)]
	private void CmdAddToDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean) {
		dataManager.arePlayersOutOfLimits.Add(playerNumber, boolean);
	}

	[Command(requiresAuthority = false)]
	private void CmdModifyDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean) {
		dataManager.arePlayersOutOfLimits[playerNumber] = boolean;
	}

	private void OnDrawGizmosSelected() {
		if (attackPoint == null) return;

		Gizmos.DrawWireCube(attackPoint.position, attack1Range);
		Gizmos.DrawWireCube(attackPoint.position, attack2Range);
	}
}
