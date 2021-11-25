using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PlayerBehaviour : NetworkBehaviour
{
  public CharacterController2D controller;
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

  [SyncVar] public int playerNumber;
  public int timer = 1;
  public int time = 1;

  private bool _AmIOutOfLimit
  {
    get
    {
      return _amIOutOfLimit;
    }
    set
    {
      if (_amIOutOfLimit != value)
      {
        _amIOutOfLimit = value;
        OnAmIOutOfLimitsChanged();
      }
    }
  }
  private bool _amIOutOfLimit = false;
  IEnumerator WaitForAtacckAgain()
  {
    yield return new WaitForSeconds(0.5f);
    timer = 1;
  }


  void Update()
  {
    if (!isLocalPlayer) return;
    AttackDamage();


    if (timer == time)
    {
      if (Input.GetKeyDown(KeyCode.Z))
      {
        Attack();
        timer = 0;
        StartCoroutine(WaitForAtacckAgain());
      }
    }




    horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

    animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

    if (Input.GetButtonDown("Jump"))
    {
      Physics2D.IgnoreLayerCollision(3, 8, true);
      controller.Jump();
      animator.SetBool("IsJumping", true);
    }
    if (Input.GetButtonUp("Jump"))
    {
      Physics2D.IgnoreLayerCollision(3, 8, false);
    }
    if (Input.GetButtonDown("Crouch"))
    {
      Physics2D.IgnoreLayerCollision(3, 8, true);
      crouch = true;
      animator.SetBool("IsCrouching", true);
    }
    else if (Input.GetButtonUp("Crouch"))
    {
      Physics2D.IgnoreLayerCollision(3, 8, false);
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
    CheckCameraLimits();
  }

  private void CheckWorldBoundaries()
  {
    WorldData worldData = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldData>();
    WorldData.WorldBounds worldBounds = worldData.GenerateWorldBounds();

    if (transform.position.x < worldBounds.leftBound || transform.position.x > worldBounds.rightBound ||
      transform.position.y < worldBounds.downBound || transform.position.y > worldBounds.upBound)
      StartCoroutine(WaitAndRespawn(gameObject));
  }

  private IEnumerator WaitAndRespawn(GameObject player)
  {
    _canCheckForBounds = false;

    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    player.GetComponent<PlayerBehaviour>().health = 0;
    GameObject map = GameObject.FindWithTag("Map");
    player.transform.position = (map != null ? map.GetComponent<Renderer>().bounds.center : Vector3.zero);

    yield return new WaitForSeconds(1f);
    // player.SetActive(true);
    _canCheckForBounds = true;
  }

  private void CheckCameraLimits()
  {
    WorldData worldData = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldData>();
    WorldData.CameraLimits cameraLimits = worldData.GenerateCameraLimits();

    if (transform.position.x < cameraLimits.leftLimit || transform.position.x > cameraLimits.rightLimit ||
      transform.position.y < cameraLimits.downLimit || transform.position.y > cameraLimits.upLimit)
    {
      _AmIOutOfLimit = true;
    }
    else
    {
      _AmIOutOfLimit = false;
    }
  }

  private void OnAmIOutOfLimitsChanged()
  {
    CmdHandleDataManagerOutOfLimitsDictionary(_AmIOutOfLimit);
  }

  [Command(requiresAuthority = false)]
  private void CmdHandleDataManagerOutOfLimitsDictionary(bool boolean)
  {
    DataManager dataManager = GameObject.FindWithTag("Data").GetComponent<DataManager>();

    if (isServer)
    {
      if (dataManager.arePlayersOutOfLimits.ContainsKey(playerNumber))
        ModifyDataManagerOutOfLimitsDictionary(dataManager, boolean);
      else
        AddToDataManagerOutOfLimitsDictionary(dataManager, boolean);

      return;
    }

    if (dataManager.arePlayersOutOfLimits.ContainsKey(playerNumber))
      CmdModifyDataManagerOutOfLimitsDictionary(dataManager, boolean);
    else
      CmdAddToDataManagerOutOfLimitsDictionary(dataManager, boolean);
  }

  private void AddToDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean)
  {
    dataManager.arePlayersOutOfLimits.Add(playerNumber, boolean);
  }

  private void ModifyDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean)
  {
    dataManager.arePlayersOutOfLimits[playerNumber] = boolean;
  }

  [Command(requiresAuthority = false)]
  private void CmdAddToDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean)
  {
    dataManager.arePlayersOutOfLimits.Add(playerNumber, boolean);
  }

  [Command(requiresAuthority = false)]
  private void CmdModifyDataManagerOutOfLimitsDictionary(DataManager dataManager, bool boolean)
  {
    dataManager.arePlayersOutOfLimits[playerNumber] = boolean;
  }

  private void OnDrawGizmosSelected()
  {
    if (attackPoint == null) return;

    Gizmos.DrawWireCube(attackPoint.position, attackRange);
  }
}
