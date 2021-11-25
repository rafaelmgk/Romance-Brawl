using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PlayerBehaviourTest2 : NetworkBehaviour
{
  public CharacterController2DTest controller;
  public Animator animator;

  public float runSpeed = 40f;

  float horizontalMove = 0f;
  bool crouch = false;

  public Transform attackPoint;
  public Vector2 attackRange = new Vector2(0.0f, 0.0f);
  public LayerMask enemyLayers;
  public int attackDirection;
  public int attackDamage;

  public int firstAtkPower = 10;

  public Rigidbody2D hitBox;
  public int health = 0;

  public NetworkConnectionToClient enemyConnection;

  private bool _canCheckForBounds = true;

  void Update()
  {
    if (!isLocalPlayer) return;

    AttackDamage();
    if (Input.GetKeyDown(KeyCode.Z))
    {
      Attack();
    }


    horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

    animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

    if (Input.GetButtonDown("Jump"))
    {
      controller.Jump();
      animator.SetBool("IsJumping", true);
    }
    if (Input.GetButtonDown("Crouch"))
    {
      crouch = true;
      animator.SetBool("IsCrouching", true);
    }
    else if (Input.GetButtonUp("Crouch"))
    {
      crouch = false;
      animator.SetBool("IsCrouching", false);
    }
  }


  void Attack()
  {
    animator.SetTrigger("Attack");
    Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, enemyLayers);
    foreach (Collider2D enemy in hitEnemies)
    {
      if (enemy != gameObject.GetComponent<Collider2D>())
        StartCoroutine(HandleDamage(enemy.gameObject, attackDamage, firstAtkPower));
    }
  }

  private IEnumerator HandleDamage(GameObject enemy, int attackDamage, int firstAtkPower)
  {
    CmdServerRemoveAuthority(enemy);
    enemy.GetComponent<PlayerBehaviour>().CmdServerTakeDamage(enemy, attackDamage, firstAtkPower);
    CmdServerAssignAuthority(enemy);
    yield return null;
  }

  [Command(requiresAuthority = false)]
  public void CmdServerTakeDamage(GameObject enemy, int dmgAndDirection, int power)
  {
    enemy.GetComponent<PlayerBehaviour>().CmdTargetTakeDamage(
      enemy.GetComponent<NetworkIdentity>().connectionToClient, dmgAndDirection, power
    );
  }

  [TargetRpc]
  public void CmdTargetTakeDamage(NetworkConnection target, int dmgAndDirection, int power)
  {
    health += power;
    hitBox.velocity = new Vector2(dmgAndDirection * health, health / 5);
  }

  [Command]
  public void CmdServerRemoveAuthority(GameObject target)
  {
    target.GetComponent<PlayerBehaviour>().CmdRemoveAuthority(
      target.GetComponent<NetworkIdentity>().connectionToClient, target
    );
  }

  [TargetRpc]
  public void CmdRemoveAuthority(NetworkConnection target, GameObject targetObj)
  {
    targetObj.GetComponent<NetworkTransform>().clientAuthority = false;
    targetObj.GetComponent<NetworkRigidbody2D>().clientAuthority = false;
  }

  [Command]
  public void CmdServerAssignAuthority(GameObject target)
  {
    target.GetComponent<PlayerBehaviour>().CmdAssignAuthority(
      target.GetComponent<NetworkIdentity>().connectionToClient, target
    );
  }

  [TargetRpc]
  public void CmdAssignAuthority(NetworkConnection target, GameObject targetObj)
  {
    targetObj.GetComponent<NetworkTransform>().clientAuthority = true;
    targetObj.GetComponent<NetworkRigidbody2D>().clientAuthority = true;
  }

  public void AttackDamage()
  {
    if (Input.GetAxisRaw("Horizontal") != 0)
    {
      attackDirection = (int)Input.GetAxisRaw("Horizontal");
    }
    attackDamage = 1 * attackDirection;
  }

  void FixedUpdate()
  {
    if (!isLocalPlayer) return;
    if (Input.anyKey) controller.Move(horizontalMove * Time.fixedDeltaTime, crouch);

    if (_canCheckForBounds) CheckWorldBoundaries();
  }

  private void CheckWorldBoundaries()
  {
    DataManager dataManager = GameObject.FindGameObjectWithTag("Data").GetComponent<DataManager>();
    DataManager.WorldBounds worldBounds = dataManager.worldBounds;

    if (transform.position.x < worldBounds.leftBound || transform.position.x > worldBounds.rightBound ||
      transform.position.y < worldBounds.downBound || transform.position.y > worldBounds.upBound)
      StartCoroutine(WaitAndRespawn(gameObject));
  }

  private IEnumerator WaitAndRespawn(GameObject player)
  {
    _canCheckForBounds = false;

    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    player.GetComponent<PlayerBehaviourTest2>().health = 0;
    player.transform.position = new Vector3(0, 2, 0);

    yield return new WaitForSeconds(1f);
    // player.SetActive(true);
    _canCheckForBounds = true;
  }

  void OnDrawGizmosSelected()
  {
    if (attackPoint == null) return;

    Gizmos.DrawWireCube(attackPoint.position, attackRange);
  }
}
