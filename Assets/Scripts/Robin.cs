using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robin : MonoBehaviour
{


  public Transform position;
  public Rigidbody2D robinHitBoX;
  public int health = 0;
  // Update is called once per frame
  void Update()
  {

  }

  public void TakeDamage(int dmgAndDirection, int power)
  {
    health += power;
    robinHitBoX.velocity = transform.right * (dmgAndDirection * health);

  }


}
