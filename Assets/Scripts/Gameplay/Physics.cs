using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public abstract class Physics : NetworkTransceiver {
	public PlayerData playerData = new PlayerData();

	[Tooltip("A mask determining what is ground to the character.")]
	public LayerMask groundLayerMask;

	[Tooltip("A position marking where to check if the player is grounded.")]
	public Transform groundCheck;

	protected event Action OutOfWorldBounds;
	protected event Action<bool> OutOfCameraLimits;

	private const float GroundCheckRadius = .2f;

	private bool _isGrounded;
	private bool _isOutOfCameraLimits;
	private bool _facingRight = true;
	private float _airTime = 0;
	private Vector2 _velocity = Vector2.zero;
	[SerializeField] private Rigidbody2D _rigidbody2D;
	private WorldData _worldData;

	// [SyncVar]
	private int _extraJumps = 1;

	private void FixedUpdate() {
		OnLanding();
		OnGround();
		OnAir();

		CheckWorldBoundaries();
		CheckCameraLimits();
	}

	private void OnLanding() {
		if (_airTime > 0 && _isGrounded == true) {
			_extraJumps = 1;
			// player.InvokeJumpEvent(false);

			_airTime = 0;
		}
	}

	private void OnGround() {
		_isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayerMask);

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

	public void Move(float move, bool crouch) {
		if (_isGrounded || playerData.airControl) {
			if (crouch)
				move *= playerData.crouchSpeed;

			Vector2 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
			HandleVelocity(targetVelocity);

			if (move != 0)
				_facingRight = (move > 0);
		}
	}

	private void HandleVelocity(Vector2 targetVelocity) {
		_rigidbody2D.velocity = Vector2.SmoothDamp(
			_rigidbody2D.velocity,
			targetVelocity,
			ref _velocity,
			playerData.movementSmoothing
		);
	}

	public void DoJump() {
		if (_extraJumps > 0) {
			_rigidbody2D.velocity = Vector2.zero;
			_rigidbody2D.AddForce(new Vector2(0f, playerData.jumpForce));

			_extraJumps--;
		}
	}

	private void Flip() {
		_facingRight = !_facingRight;

		transform.Rotate(0f, 180f, 0f);
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
		// Without this condition the event would be fired a few times
		// instead of one, flickering the camera
		if (_isOutOfCameraLimits != newState) {
			_isOutOfCameraLimits = newState;
			OutOfCameraLimits?.Invoke(newState);
		}
	}
}
