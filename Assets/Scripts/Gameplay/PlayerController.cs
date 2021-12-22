using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public abstract class PlayerController : PlayerPhysics {
	public enum NotificationType {
		PlayerMoved,
		PlayerJumped,
		PlayerBasicAttacked,
		PlayerStrongAttacked,
	}

	public NetworkController networkController;

	[Tooltip("List of transceivers (observers) to send events")]
	[SerializeField] private List<Transceiver> _transceivers = new List<Transceiver>();

	[Header("Attack Settings")]
	[Tooltip("The mask layer to determine whom the attack will collide")]
	[SerializeField] private LayerMask _enemyLayers; // TODO: change 'enemy' to 'player'

	[Tooltip("The base position where the range attack will be calculated from")]
	[SerializeField] private Transform _attackPoint; // TODO: create an Attack class

	[Tooltip("The range/hitbox of the 1st attack")]
	[SerializeField] private Vector2 _attack1Range;

	[Tooltip("The range/hitbox of the 2nd attack")]
	[SerializeField] private Vector2 _attack2Range;

	[Tooltip("The damage dealt on the 1st attack")]
	[SerializeField] private int _atk1Power;

	[Tooltip("The damage dealt on the 2nd attack")]
	[SerializeField] private int _atk2Power;

	private Vector2 _movementVector;
	private float _stunTime = 0f;
	private float _stunTimer = 1f;
	private int _attackDirection;
	private bool _crouch = false;
	private bool _canAttack = true;
	private bool _canRespawn = true;
	private bool _canMove = false;

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

	private void Update() {
		if (_canMove)
			Move(_movementVector.x, _crouch);

		HandleStun();
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

	public void TakeDamage(int attackDirection, int power) {
		if (_crouch)
			return;

		AddDamage(power);
		BeThrown(attackDirection, networkController.hitPercentage);

		Notify(AnimatorController.NotificationType.PlayerTookDamage);

		_stunTime = 1f;
	}

	private void AssignAttackDirection() {
		if (_movementVector.x != 0)
			_attackDirection = (int)_movementVector.x;
	}

	// private bool AreAnyGamepadButtonsPressed() {
	// 	if (Gamepad.current == null) return false;

	// 	for (int i = 0; i < Gamepad.current.allControls.Count; i++) {
	// 		if (Gamepad.current.allControls[i].IsPressed()) return true;
	// 	}

	// 	return false;
	// }

	private void HandleStun() {
		if (_stunTime > 0) {
			_stunTime -= _stunTimer * Time.deltaTime;
			if (_stunTime < 0)
				_stunTime = 0;
		}
	}

	private void AddDamage(int power) {
		int newHitPercentage = networkController.hitPercentage + power;
		UpdateHitPercentage(newHitPercentage);
	}

	private void UpdateHitPercentage(int newHitPercentage) {
		networkController.CmdUpdateHitPercentageOnServer(newHitPercentage);
	}

	private void OnPlayerMoved(InputAction.CallbackContext context) {
		// TODO: relocate input methods to input controller
		_movementVector = context.ReadValue<Vector2>();
		if (Gamepad.current != null)
			_movementVector = DigitalizeVector2(_movementVector);

		AssignAttackDirection();

		// TODO: refactor this
		if (context.started || context.performed) {
			_canMove = true;
			if (_movementVector.y < 0f)
				Crouch(true);
		}
		if (context.canceled) {
			_canMove = false;
			if (_movementVector.y >= 0f)
				Crouch(false);
		}

		// TODO: remove this runSpeed dependency
		Notify(AnimatorController.NotificationType.SpeedChanged, _movementVector.x * runSpeed);
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
		_crouch = crouchState;

		Notify(AnimatorController.NotificationType.CrouchChanged, crouchState);
	}

	private void IgnorePlatformCollision(bool ignore = true) {
		Physics2D.IgnoreLayerCollision(3, 8, ignore);
	}

	private void OnPlayerJumped(InputAction.CallbackContext context) {
		if (context.started && _stunTime == 0)
			IgnorePlatformCollisionAndJump();
		else if (context.canceled)
			IgnorePlatformCollision(false);
	}

	private void IgnorePlatformCollisionAndJump() {
		IgnorePlatformCollision();
		DoJump();

		InvokeJumpEvent(true);
	}

	private void InvokeJumpEvent(bool jumpState) {
		Notify(AnimatorController.NotificationType.JumpChanged, jumpState);
	}

	// TODO: remove this method repetition
	private void OnPlayerBasicAttacked(InputAction.CallbackContext context) {
		if (context.started && _canAttack && !_crouch) {
			Attack(_attackPoint, _attack1Range, "Attack", _atk1Power);
			StartCoroutine(WaitForAttackAgain());
		}
	}

	private void OnPlayerStrongAttacked(InputAction.CallbackContext context) {
		if (context.started && _canAttack && !_crouch) {
			Attack(_attackPoint, _attack2Range, "Attack2", _atk2Power);
			StartCoroutine(WaitForAttackAgain());
		}
	}

	private void Attack(Transform attackPoint, Vector2 attackRange, string animation, int attackPower) {
		_canAttack = false;

		Notify(AnimatorController.NotificationType.PlayerAttacked, animation);

		Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, _enemyLayers);
		foreach (Collider2D enemy in hitEnemies) {
			if (enemy != gameObject.GetComponent<Collider2D>())
				AskServerForTakeDamage(enemy.gameObject.GetComponent<NetworkController>(), _attackDirection, attackPower);
		}
	}

	private void AskServerForTakeDamage(NetworkController enemy, int attackDirection, int firstAtkPower) {
		enemy.CmdAskServerForTakeDamage(enemy, attackDirection, firstAtkPower);
	}

	private IEnumerator WaitForAttackAgain() {
		yield return new WaitForSeconds(0.5f);
		_canAttack = true;
	}

	private void OnOutOfWorldBounds() {
		if (_canRespawn)
			StartCoroutine(WaitAndRespawn());
	}

	private IEnumerator WaitAndRespawn() {
		_canRespawn = false;

		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		UpdateHitPercentage(0);
		GameObject map = GameObject.FindWithTag("Map");
		transform.position = (map != null ? map.GetComponent<Renderer>().bounds.center : Vector3.zero);

		yield return new WaitForSeconds(1f);
		// gameObject.SetActive(true);
		_canRespawn = true;
	}

	private void OnOutOfCameraLimits(bool outOfLimits) {
		networkController.CmdHandleDataManagerOutOfLimitsDictionary(outOfLimits, networkController.playerNumber);
	}

	private void OnDrawGizmosSelected() {
		if (_attackPoint == null) return;

		Gizmos.DrawWireCube(_attackPoint.position, _attack1Range);
		Gizmos.DrawWireCube(_attackPoint.position, _attack2Range);
	}
}
