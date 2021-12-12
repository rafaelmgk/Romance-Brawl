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

	protected bool canCheckForWorldBounds = true;

	private const float GroundCheckRadius = .2f;

	private bool _isGrounded;
	private bool _isOutOfCameraLimits;
	private bool _facingRight = true;
	private float _airTime = 0;
	private Vector2 _velocity = Vector2.zero;
	private Rigidbody2D _rigidbody2D;

	// [SyncVar]
	private int _extraJumps = 1;

	private void Awake() {
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		OnLanding();
		OnGround();
		OnAir();

		if (canCheckForWorldBounds)
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
		WorldData worldData = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldData>();
		WorldData.WorldBounds worldBounds = worldData.GenerateWorldBounds();

		if (transform.position.x < worldBounds.leftBound || transform.position.x > worldBounds.rightBound ||
			transform.position.y < worldBounds.downBound || transform.position.y > worldBounds.upBound)
			OutOfWorldBounds?.Invoke();
	}

	private void CheckCameraLimits() {
		WorldData worldData = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldData>();
		WorldData.CameraLimits cameraLimits = worldData.GenerateCameraLimits();

		if (transform.position.x < cameraLimits.leftLimit || transform.position.x > cameraLimits.rightLimit ||
			transform.position.y < cameraLimits.downLimit || transform.position.y > cameraLimits.upLimit)
			ChangeOutOfCameraLimits(true);
		else
			ChangeOutOfCameraLimits(false);
	}

	private void ChangeOutOfCameraLimits(bool newState) {
		if (_isOutOfCameraLimits != newState) {
			_isOutOfCameraLimits = newState;
			OutOfCameraLimits?.Invoke(newState);
		}
	}
}
