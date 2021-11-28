using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public abstract class PlayerBehaviour : NetworkBehaviour
{
  public CharacterController2D controller;
  public Animator animator;

  public float runSpeed = 40f;

  float horizontalMove = 0f;
  public bool crouch = false;

  public Transform attackPoint;
  public Vector2 attackRange = new Vector2(0.0f, 0.0f);
  public LayerMask enemyLayers;
  public int attackDirection;

  public int firstAtkPower = 10;

  public Rigidbody2D hitBox;
  public int health = 0;

  public NetworkConnectionToClient enemyConnection;

  private bool _canCheckForBounds = true;

  [SyncVar] public int playerNumber;

  private bool _canAttack = true;

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

  private Vector2 _movementVector;

  // private PlayerInput playerInput;
  // private InputAction movementAction, jumpAction, basicAtkAction;

  // private void Awake() {
  // 	playerInput = GetComponent<PlayerInput>();
  // 	movementAction = playerInput.actions["Movement"];
  // 	jumpAction = playerInput.actions["Jump"];
  // 	basicAtkAction = playerInput.actions["Basic Atk"];
  // }

  // private void Start() {
  // 	movementAction.started += Movement;
  // 	movementAction.performed += Movement;
  // 	movementAction.canceled += Movement;

  // 	jumpAction.started += Jump;
  // 	jumpAction.performed += Jump;
  // 	jumpAction.canceled += Jump;

  // 	basicAtkAction.started += BasicAtk;
  // 	basicAtkAction.performed += BasicAtk;
  // 	basicAtkAction.canceled += BasicAtk;
  // }

  private void Start()
  {
    if (!isLocalPlayer)
      Destroy(GetComponent<PlayerInput>());
    else
      GetComponent<PlayerInput>().enabled = true;
  }

  public void Movement(InputAction.CallbackContext context)
  {
    if (!isLocalPlayer) return;
    _movementVector = context.ReadValue<Vector2>();

    if ((context.started || context.performed) && _movementVector.y < 0)
    {
      Physics2D.IgnoreLayerCollision(3, 8, true);
      crouch = true;
      animator.SetBool("IsCrouching", true);
    }
    else if (context.canceled)
    {
      Physics2D.IgnoreLayerCollision(3, 8, false);
      crouch = false;
      animator.SetBool("IsCrouching", false);
    }
  }

  public void Jump(InputAction.CallbackContext context)
  {
    if (!isLocalPlayer) return;
    if (context.started)
    {
      animator.SetBool("IsJumping", true);
      Physics2D.IgnoreLayerCollision(3, 8, true);
      controller.Jump();
    }
    else if (context.canceled)
      Physics2D.IgnoreLayerCollision(3, 8, false);
  }

  public void BasicAtk(InputAction.CallbackContext context)
  {
    if (!isLocalPlayer) return;
    if (context.started && _canAttack)
    {
      Attack(attackPoint, attackRange, "Attack");
      StartCoroutine(WaitForAttackAgain());
    }
  }

  private IEnumerator WaitForAttackAgain()
  {
    yield return new WaitForSeconds(0.5f);
    _canAttack = true;
  }


  void Update()
  {
    if (!isLocalPlayer) return;
    AttackDirection();

    horizontalMove = _movementVector.x * runSpeed;

    animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
  }


  void Attack(Transform attackPoint, Vector2 attackRange, string animation)
  {
    _canAttack = false;

    animator.SetTrigger(animation);
    Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, enemyLayers);
    foreach (Collider2D enemy in hitEnemies)
    {
      if (enemy != gameObject.GetComponent<Collider2D>())
        AskServerForTakeDamage(enemy.gameObject, attackDirection, firstAtkPower);
    }
  }

  private void AskServerForTakeDamage(GameObject enemy, int attackDirection, int firstAtkPower)
  {
    enemy.GetComponent<PlayerBehaviour>().CmdAskServerForTakeDamage(enemy, attackDirection, firstAtkPower);
  }

  [Command(requiresAuthority = false)]
  public void CmdAskServerForTakeDamage(GameObject enemy, int attackDirection, int power)
  {
    enemy.GetComponent<PlayerBehaviour>().TrgtTakeDamage(
      enemy.GetComponent<NetworkIdentity>().connectionToClient, attackDirection, power
    );
  }

  [TargetRpc]
  public void TrgtTakeDamage(NetworkConnection target, int attackDirection, int power)
  {
    int atkPower = power;
    int crouchMultiplier = 1;
    if (crouch)
    {
      crouchMultiplier = 0;
      atkPower = 0;
    }

    health += atkPower;

    // hitBox.velocity = new Vector2(crouchMultiplier * attackDirection * health, crouchMultiplier * health / 3.5f);
    hitBox.AddForce(new Vector2(crouchMultiplier * attackDirection * health, crouchMultiplier * health / 3.5f), ForceMode2D.Impulse);
  }

  public void AttackDirection()
  {
    if (_movementVector.x != 0)
      attackDirection = (int)_movementVector.x;
  }

  void FixedUpdate()
  {
    if (!isLocalPlayer) return;

    if (Keyboard.current.anyKey.IsPressed() || AreAnyGamepadButtonsPressed())
      controller.Move(horizontalMove * Time.fixedDeltaTime, crouch);

    if (_canCheckForBounds)
      CheckWorldBoundaries();
    CheckCameraLimits();
  }

  private bool AreAnyGamepadButtonsPressed()
  {
    if (Gamepad.current == null) return false;

    for (int i = 0; i < Gamepad.current.allControls.Count; i++)
    {
      if (Gamepad.current.allControls[i].IsPressed()) return true;
    }

    return false;
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
