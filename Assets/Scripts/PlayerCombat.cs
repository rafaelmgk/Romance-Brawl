using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

  public Animator animator;

  public Transform attackPoint;
  public float attackRange = 0.5f;
  public LayerMask enemyLayers;
  public int attackDirection;
  public int attackDamage;

  public int firstAtkPower = 10;


  // Update is called once per frame
  void Update()
  {
    AttackDamage();
    if (Input.GetKeyDown(KeyCode.Z))
    {
      Attack();
    }
  }

  void Attack()
  {
    animator.SetTrigger("Attack");
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
    foreach (Collider2D enemy in hitEnemies)
    {
      if (enemy.GetComponent<Chopper>() != null)
      {
        enemy.GetComponent<Chopper>().TakeDamage(attackDamage, firstAtkPower);
        break;
      }

      if (enemy.GetComponent<Luffy>() != null)
      {
        enemy.GetComponent<Luffy>().TakeDamage(attackDamage, firstAtkPower);
        break;
      }
    }

  }

  void OnDrawGizmosSelected()
  {
    if (attackPoint == null)
      return;

    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
  }

  public void AttackDamage()
  {
    if (Input.GetAxisRaw("Horizontal") != 0)
    {
      attackDirection = (int)Input.GetAxisRaw("Horizontal");
    }
    attackDamage = 1 * attackDirection;
  }


}
