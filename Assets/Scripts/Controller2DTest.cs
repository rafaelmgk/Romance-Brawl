using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Controller2DTest : NetworkBehaviour
{
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
  void FixedUpdate()
  {
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);




    moveInput = Input.GetAxis("Horizontal");
    m_Rigidbody2D.velocity = new Vector2(moveInput * speed, m_Rigidbody2D.velocity.y);
    if (facingRinght == false && moveInput > 0)
    {
      Flip();
    }
    else if (facingRinght == true && moveInput < 0)
    {
      Flip();
    }
  }
  void Update()
  {
    if (isGrounded == true)
    {
      extraJumps = 1;
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

  }
  void Flip()
  {
    facingRinght = !facingRinght;
    Vector3 Scaler = transform.localScale;
    Scaler.x *= -1;
    transform.localScale = Scaler;
  }
}