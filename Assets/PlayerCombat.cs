using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;


    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("We hit a enemie");
            enemy.GetComponent<Chopper>().testing();
        }

    }

    void OnDrawGizmosSelected() 
    {
        if (attackPoint == null)
        return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

   
}
