using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brook : MonoBehaviour
{


  public Transform position;
  public Rigidbody2D brookHitBoX;
  public int health = 0;
  // Update is called once per frame
  void Update()
  {

  }

  public void TakeDamage(int dmgAndDirection, int power)
  {
    health += power;
    brookHitBoX.velocity = transform.right * (dmgAndDirection * health);

  }


}
