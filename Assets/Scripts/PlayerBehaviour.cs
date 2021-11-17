using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBehaviour : NetworkBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;

	// Update is called once per frame
	void Update() {
		if (!isLocalPlayer) return;
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump")) {
			jump = true;
			animator.SetBool("IsJumping", true);
		}
		if (Input.GetButtonDown("Crouch")) {
			crouch = true;
			animator.SetBool("IsCrouching", true);
		} else if (Input.GetButtonUp("Crouch")) {
			crouch = false;
			animator.SetBool("IsCrouching", false);
		}
	}
	public void OnLanding() {
		animator.SetBool("IsJumping", false);
		jump = false;
	}

	void FixedUpdate() {
		if (!isLocalPlayer) return;
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
	}
}
