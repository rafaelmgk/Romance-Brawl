using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerController : PlayerPhysics {
	public enum NotificationType {
		PlayerMoved,
		PlayerJumped,
		PlayerAttacked,
		OutOfWorldBounds,
		OutOfCameraLimits,
		PlayerFlipped
	}

	[Header("Network Settings")]
	public NetworkController networkController;

	[Header("Attack Settings")]
	[Tooltip("The list of attack to be populated by the scriptable object")]
	public List<Attack> attacks = new List<Attack>();

	[Tooltip("The mask layer to determine whom the attack will collide")]
	[SerializeField] private LayerMask _enemyLayers; // TODO: change 'enemy' to 'player'

	private Vector2 _movementVector;
	private float _stunTime = 0f;
	private float _stunTimer = 1f;
	private int _attackDirection;
	private bool _crouch = false;
	private bool _canAttack = true;
	private bool _canRespawn = true;
	private bool _canMove = false;

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
			NotificationType.PlayerAttacked,
			(atkInput) => OnPlayerAttacked((Attack.AttackInput)atkInput)
		);
		RegisterAction(
			NotificationType.OutOfWorldBounds,
			(any) => OnOutOfWorldBounds()
		);
		RegisterAction(
			NotificationType.OutOfCameraLimits,
			(outOfLimits) => networkController.CmdHandleDataManagerOutOfLimitsDictionary((bool)outOfLimits, networkController.playerNumber)
		);
		RegisterAction(
			NotificationType.PlayerFlipped,
			(flipState) => networkController.CmdUpdateFlipOnServer((bool)flipState)
		);
	}

	private void Start() {
		AssignScriptableObjectData();
	}

	private void Update() {
		if (_canMove)
			Move(_movementVector.x, _crouch);

		HandleStun();
	}

	public override bool IsNotificationTypeValid(Enum notificationType) {
		if (notificationType.GetType() == typeof(NotificationType))
			return true;

		return false;
	}

	public void TakeDamage(int attackDirection, int power) {
		if (_crouch)
			return;

		AddDamage(power);
		BeThrown(attackDirection, networkController.hitPercentage);

		Notify(AnimatorController.NotificationType.PlayerTookDamage);

		// TODO: remove hard coded number
		_stunTime = 1f;
	}

	public void SetAnimator(AnimatorOverrideController animatorController) {
		Notify(AnimatorController.NotificationType.SetAnimator, animatorController);
	}

	private void AssignScriptableObjectData() {
		int index = DataManager.Instance.charactersByPlayer[networkController.playerNumber];
		SetAnimator(DataManager.Instance.characters[index].animator);
		attacks = DataManager.Instance.characters[index].attacks;
	}

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
		ReadMovement(context);
		AssignAttackDirection();

		if (context.started || context.performed)
			DefineMovement(true);
		if (context.canceled)
			DefineMovement(false);

		// TODO: remove this runSpeed dependency
		Notify(AnimatorController.NotificationType.SpeedChanged, _movementVector.x * runSpeed);
	}

	private void ReadMovement(InputAction.CallbackContext context) {
		_movementVector = context.ReadValue<Vector2>();
		if (Gamepad.current != null)
			_movementVector = InputsController.DigitalizeVector2(_movementVector);
	}

	// TODO: maybe use facingLeft
	private void AssignAttackDirection() {
		if (_movementVector.x != 0)
			_attackDirection = (int)_movementVector.x;
	}

	private void DefineMovement(bool canMove) {
		_canMove = canMove;
		Crouch(_movementVector.y < 0f);
	}

	// TO-THINK: maybe put this in PlayerPhysics
	private void Crouch(bool crouchState) {
		IgnorePlatformCollision(crouchState);
		_crouch = crouchState;

		Notify(AnimatorController.NotificationType.CrouchChanged, crouchState);
	}

	private void OnPlayerJumped(InputAction.CallbackContext context) {
		if (context.started && _stunTime == 0) {
			IgnorePlatformCollision();
			Jump();
		}
		else if (context.canceled)
			IgnorePlatformCollision(false);
	}

	private void OnPlayerAttacked(Attack.AttackInput atkInput) {
		if (atkInput.context.started && _canAttack && !_crouch) {
			Attack(attacks[atkInput.attackIndex]);
			StartCoroutine(WaitForAttackAgain());
		}
	}

	// TODO: maybe place this in AttacksController
	private void Attack(Attack atk) {
		_canAttack = false;

		Notify(AnimatorController.NotificationType.PlayerAttacked, atk.attackAnimation);

		Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(atk.GetAttackPoint(atk.attackPointIndex), atk.attackRange, 0, _enemyLayers);
		foreach (Collider2D enemy in hitEnemies) {
			if (enemy != GetComponent<Collider2D>())
				AskServerForTakeDamage(enemy.gameObject.GetComponent<NetworkController>(), _attackDirection, atk.attackPower);
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

	private void OnDrawGizmosSelected() {
		foreach (Attack atk in attacks)
			Gizmos.DrawWireCube(atk.GetAttackPoint(atk.attackPointIndex), atk.attackRange);
	}
}
