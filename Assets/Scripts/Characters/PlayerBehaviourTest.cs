using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PlayerBehaviourTest : NetworkBehaviour
{
  public Animator animator;
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
  public float speed;
  public float jumpForce;
  public float moveInput;
  private Rigidbody2D m_Rigidbody2D;
  private bool facingRinght = true;
  private bool isGrounded;
  public Transform groundCheck;
  public float checkRadius;
  public LayerMask whatIsGround;
  public int extraJumps;

  private void Awake()
  {
    m_Rigidbody2D = GetComponent<Rigidbody2D>();
  }
  void Update()
  {
    if (!isLocalPlayer) return;

    AttackDamage();
    if (Input.GetKeyDown(KeyCode.Z))
    {
      Attack();
    }
    if (isGrounded == true)
    {
      extraJumps = 1;
      animator.SetBool("IsJumping", false);
    }
    if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
    {
      m_Rigidbody2D.velocity = Vector2.up * jumpForce;
      extraJumps--;
    }
    else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
    {
      m_Rigidbody2D.velocity = Vector2.up * jumpForce;
    }
    if (Input.GetButtonDown("Crouch"))
    {
      animator.SetBool("IsCrouching", true);
    }
    else if (Input.GetButtonUp("Crouch"))
    {
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
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    moveInput = Input.GetAxisRaw("Horizontal") * speed;
    m_Rigidbody2D.velocity = new Vector2(moveInput * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
    if (facingRinght == false && moveInput > 0)
    {
      Flip();
    }
    else if (facingRinght == true && moveInput < 0)
    {
      Flip();
    }

    animator.SetFloat("Speed", Mathf.Abs(moveInput));

    if (Input.GetButtonDown("Jump"))
    {
      animator.SetBool("IsJumping", true);
    }


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
    player.GetComponent<PlayerBehaviour>().health = 0;
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
  void Flip()
  {
    facingRinght = !facingRinght;
    Vector3 Scaler = transform.localScale;
    Scaler.x *= -1;
    transform.localScale = Scaler;
  }
}
