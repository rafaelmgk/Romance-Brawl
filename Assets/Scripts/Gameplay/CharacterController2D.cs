using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class CharacterController2D : NetworkBehaviour {
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	[SyncVar] public int extraJumps;
	public PlayerController player;

	private float _airTime = 0;



	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake() {
		m_Rigidbody2D = GetComponent<Rigidbody2D>();



		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate() {
		OnLanding();
		OnGround();
	}

	void Update() {

	}

	private void OnLanding() {
		if (_airTime > 0 && m_Grounded == true) {
			extraJumps = 1;
			player.InvokeJumpEvent(false);

			_airTime = 0;
		}
	}

	private void OnGround() {
		m_Grounded = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

		if (m_Grounded == true)
			extraJumps = 1;
		else
			_airTime += Time.deltaTime;
	}

	public void Move(float move, bool crouch) {
		// If crouching, check to see if the character can stand up
		if (!crouch) {
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) {
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl) {

			// If crouching
			if (crouch) {
				if (!m_wasCrouching) {
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else {
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching) {
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			if (isClientOnly) {
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			}
			CmdMove(move);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight) {
				// ... flip the player.
				if (isClientOnly) Flip();
				CmdFlip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight) {
				// ... flip the player.
				if (isClientOnly) Flip();
				CmdFlip();
			}
		}

	}
	public void Jump() {
		if (extraJumps > 0) {
			extraJumps--;
			if (isClientOnly) m_Rigidbody2D.velocity = Vector2.up * 0;
			if (isClientOnly) m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			CmdJump();
		}
	}

	private void Flip() {
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}


	[Command(requiresAuthority = false)]
	public void CmdMove(float move) {
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
	}

	[Command(requiresAuthority = false)]
	public void CmdJump() {
		m_Rigidbody2D.velocity = Vector2.up * 0;
		m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
	}

	[Command(requiresAuthority = false)]
	public void CmdFlip() {
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}
}
