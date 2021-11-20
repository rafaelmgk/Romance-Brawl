using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PlayerBehaviour : NetworkBehaviour {
	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;

	public Transform attackPoint;
	public float attackRange = 0.5f;
	public LayerMask enemyLayers;
	public int attackDirection;
	public int attackDamage;

	public int firstAtkPower = 10;

	public Rigidbody2D hitBox;
	public int health = 0;

	void Update() {
		if (!isLocalPlayer) return;

		AttackDamage();
		if (Input.GetKeyDown(KeyCode.Z)) {
			Attack();
		}


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

	void Attack() {
		animator.SetTrigger("Attack");
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
		foreach (Collider2D enemy in hitEnemies) {
			if (enemy != gameObject.GetComponent<Collider2D>())
				enemy.GetComponent<PlayerBehaviour>().TakeDamage(attackDamage, firstAtkPower);
		}
	}

	public void TakeDamage(int dmgAndDirection, int power) {
		health += power;
		hitBox.velocity = new Vector3(dmgAndDirection * health, health / 5, 0);
	}

	public void AttackDamage() {
		if (Input.GetAxisRaw("Horizontal") != 0) {
			attackDirection = (int)Input.GetAxisRaw("Horizontal");
		}
		attackDamage = 1 * attackDirection;
	}

	void FixedUpdate() {
		if (!isLocalPlayer) return;
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
	}

	void OnDrawGizmosSelected() {
		if (attackPoint == null) return;

		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}
