using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public abstract class PlayerPhysics : Transceiver {
	protected event Action OutOfWorldBounds;
	protected event Action<bool> OutOfCameraLimits;

	[Header("Movement Settings")]
	[Tooltip("The player running speed")]
	[SerializeField] protected float runSpeed = 20f;

	[Tooltip("How much to smooth out the movement")]
	[Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;

	[Tooltip("Amount of force added when the player jumps")]
	[SerializeField] private float _jumpForce = 400f;

	[Tooltip("Whether or not a player can steer while jumping")]
	[SerializeField] private bool _airControl = false;

	[Tooltip("The percentage to decrease from speed when crouching")]
	[Range(0, 1)] [SerializeField] private float _crouchSpeed = .36f;

	[Header("Ground Settings")]
	[Space]
	[Tooltip("A mask determining what is ground to the character")]
	[SerializeField] private LayerMask _groundLayerMask;

	[Tooltip("A position marking where to check if the player is grounded")]
	[SerializeField] private Transform _groundCheck;

	[Header("Components Reference")]
	[Space]
	[Tooltip("The player rigidbody")]
	[SerializeField] private Rigidbody2D _rigidbody2D;

	private const float _GROUND_CHECK_RADIUS = .2f;

	private Vector2 _velocity = Vector2.zero;
	private float _airTime = 0;
	private int _extraJumps = 1;
	private bool _isGrounded;
	private bool _isOutOfCameraLimits;
	private bool _isFacingRight = true;
	private WorldData _worldData;

	private void FixedUpdate() {
		OnLanding();
		OnGround();
		OnAir();

		CheckWorldBoundaries();
		CheckCameraLimits();
	}

	protected void Move(float move, bool crouch) {
		if (_isGrounded || _airControl) {
			if (crouch)
				move *= _crouchSpeed;

			// TODO: remove hard coded number
			SmoothVelocity(new Vector2(move * runSpeed * Time.fixedDeltaTime * 10f, _rigidbody2D.velocity.y));

			if (move != 0)
				AssignIsFacingRight(move > 0);
		}
	}

	protected void DoJump() {
		if (_extraJumps > 0) {
			_rigidbody2D.velocity = Vector2.zero;
			_rigidbody2D.AddForce(new Vector2(0f, _jumpForce));

			_extraJumps--;
		}
	}

	protected void Flip() {
		_isFacingRight = !_isFacingRight;

		transform.Rotate(0f, 180f, 0f);
	}

	protected void BeThrown(int attackDirection, int hitPercentage) {
		// TODO: remove hard coded number
		_rigidbody2D.AddForce(new Vector2(attackDirection * hitPercentage, hitPercentage / 3.5f), ForceMode2D.Impulse);
	}

	private void SmoothVelocity(Vector2 targetVelocity) {
		_rigidbody2D.velocity = Vector2.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, _movementSmoothing);
	}

	private void AssignIsFacingRight(bool isFacingRight) {
		_isFacingRight = isFacingRight;
	}

	private void OnLanding() {
		if (_airTime > 0 && _isGrounded == true) {
			_extraJumps = 1;
			// player.InvokeJumpEvent(false);

			_airTime = 0;
		}
	}

	private void OnGround() {
		_isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _GROUND_CHECK_RADIUS, _groundLayerMask);

		if (_isGrounded)
			RechargeJumps();
	}

	private void RechargeJumps() {
		_extraJumps = 1;
	}

	private void OnAir() {
		if (!_isGrounded)
			_airTime += Time.deltaTime;
	}

	private void CheckWorldBoundaries() {
		WorldData.WorldBounds worldBounds = FindWorldBounds(FindWorldData());

		if (IsPositionSquaredOutside(worldBounds.leftBound, worldBounds.rightBound,
			worldBounds.downBound, worldBounds.upBound))
			OutOfWorldBounds?.Invoke();
	}

	private WorldData.WorldBounds FindWorldBounds(WorldData worldData) {
		return worldData.GenerateWorldBounds();
	}

	private WorldData FindWorldData() {
		if (_worldData != null)
			return _worldData;

		_worldData = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldData>();
		return _worldData;
	}

	private bool IsPositionSquaredOutside(float leftBound, float rightBound, float downBound, float upBound) {
		if (transform.position.x < leftBound || transform.position.x > rightBound ||
			transform.position.y < downBound || transform.position.y > upBound)
			return true;
		else
			return false;
	}

	private void CheckCameraLimits() {
		WorldData.CameraLimits cameraLimits = FindCameraLimits(FindWorldData());

		if (IsPositionSquaredOutside(cameraLimits.leftLimit, cameraLimits.rightLimit,
			cameraLimits.downLimit, cameraLimits.upLimit))
			ChangeOutOfCameraLimits(true);
		else
			ChangeOutOfCameraLimits(false);
	}

	private WorldData.CameraLimits FindCameraLimits(WorldData worldData) {
		return worldData.GenerateCameraLimits();
	}

	private void ChangeOutOfCameraLimits(bool newState) {
		// Without this condition the event would be fired a few times instead of one, flickering the camera
		if (_isOutOfCameraLimits != newState) {
			_isOutOfCameraLimits = newState;
			OutOfCameraLimits?.Invoke(newState);
		}
	}
}
