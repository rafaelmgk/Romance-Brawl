// using UnityEngine;
// using UnityEngine.Events;
// using Mirror;

// public class Phigd : Transmitter {
// 	public PlayerController player;

// 	[Tooltip("A mask determining what is ground to the character.")]
// 	public LayerMask groundLayerMask;

// 	[Tooltip("A position marking where to check if the player is grounded.")]
// 	public Transform groundCheck;

// 	[Tooltip("A position marking where to check for ceilings.")]
// 	public Transform ceilingCheck;

// 	[Tooltip("A collider that will be disabled when crouching.")]
// 	public Collider2D crouchDisableCollider;

// 	private const float GroundCheckRadius = .2f;
// 	private const float CeilingRadius = .2f;

// 	private bool _isGrounded;
// 	private bool _facingRight = true;
// 	private float _airTime = 0;
// 	private Vector3 _velocity = Vector3.zero;
// 	private Rigidbody2D _rigidbody2D;
// 	[SyncVar] private int _extraJumps;


// 	[System.Serializable]
// 	private class BoolEvent : UnityEvent<bool> { }

// 	private BoolEvent OnCrouchEvent;
// 	private bool _wasCrouching = false;

// 	private void Awake() {
// 		_rigidbody2D = GetComponent<Rigidbody2D>();

// 		if (OnCrouchEvent == null)
// 			OnCrouchEvent = new BoolEvent();
// 	}

// 	private void FixedUpdate() {
// 		OnLanding();
// 		OnGround();
// 	}

// 	void Update() {

// 	}

// 	private void OnLanding() {
// 		if (_airTime > 0 && _isGrounded == true) {
// 			_extraJumps = 1;
// 			player.InvokeJumpEvent(false);

// 			_airTime = 0;
// 		}
// 	}

// 	private void OnGround() {
// 		_isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayerMask);

// 		if (_isGrounded == true)
// 			_extraJumps = 1;
// 		else
// 			_airTime += Time.deltaTime;
// 	}

// 	public void Move(float move, bool crouch) {
// 		// If crouching, check to see if the character can stand up
// 		if (!crouch) {
// 			// If the character has a ceiling preventing them from standing up, keep them crouching
// 			if (Physics2D.OverlapCircle(ceilingCheck.position, CeilingRadius, groundLayerMask)) {
// 				crouch = true;
// 			}
// 		}

// 		//only control the player if grounded or airControl is turned on
// 		if (_isGrounded || m_AirControl) {

// 			// If crouching
// 			if (crouch) {
// 				if (!_wasCrouching) {
// 					_wasCrouching = true;
// 					OnCrouchEvent.Invoke(true);
// 				}

// 				// Reduce the speed by the crouchSpeed multiplier
// 				move *= m_CrouchSpeed;

// 				// Disable one of the colliders when crouching
// 				if (m_CrouchDisableCollider != null)
// 					m_CrouchDisableCollider.enabled = false;
// 			} else {
// 				// Enable the collider when not crouching
// 				if (m_CrouchDisableCollider != null)
// 					m_CrouchDisableCollider.enabled = true;

// 				if (_wasCrouching) {
// 					_wasCrouching = false;
// 					OnCrouchEvent.Invoke(false);
// 				}
// 			}

// 			if (isClientOnly) {
// 				// Move the character by finding the target velocity
// 				Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
// 				// And then smoothing it out and applying it to the character
// 				_rigidbody2D.velocity = Vector3.SmoothDamp(
// 					_rigidbody2D.velocity,
// 					targetVelocity,
// 					ref _rigidbody2D,
// 					m_MovementSmoothing
// 				);
// 			}
// 			CmdMove(move);

// 			// If the input is moving the player right and the player is facing left...
// 			if (move > 0 && !_facingRight) {
// 				// ... flip the player.
// 				if (isClientOnly) Flip();
// 				CmdFlip();
// 			}
// 			// Otherwise if the input is moving the player left and the player is facing right...
// 			else if (move < 0 && _facingRight) {
// 				// ... flip the player.
// 				if (isClientOnly) Flip();
// 				CmdFlip();
// 			}
// 		}

// 	}
// 	public void Jump() {
// 		if (_extraJumps > 0) {
// 			_extraJumps--;
// 			if (isClientOnly) _rigidbody2D.velocity = Vector2.up * 0;
// 			if (isClientOnly) _rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
// 			CmdJump();
// 		}
// 	}

// 	private void Flip() {
// 		// Switch the way the player is labelled as facing.
// 		_facingRight = !_facingRight;

// 		transform.Rotate(0f, 180f, 0f);
// 	}


// 	[Command(requiresAuthority = false)]
// 	public void CmdMove(float move) {
// 		// Move the character by finding the target velocity
// 		Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
// 		// And then smoothing it out and applying it to the character
// 		_rigidbody2D.velocity = Vector3.SmoothDamp(
// 			_rigidbody2D.velocity,
// 			targetVelocity,
// 			ref _velocity,
// 			m_MovementSmoothing
// 		);
// 	}

// 	[Command(requiresAuthority = false)]
// 	public void CmdJump() {
// 		_rigidbody2D.velocity = Vector2.up * 0;
// 		_rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
// 	}

// 	[Command(requiresAuthority = false)]
// 	public void CmdFlip() {
// 		// Switch the way the player is labelled as facing.
// 		_facingRight = !_facingRight;

// 		transform.Rotate(0f, 180f, 0f);
// 	}
// }
