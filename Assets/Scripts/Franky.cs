using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Franky : MonoBehaviour
{


  public Transform position;
  public Rigidbody2D frankyHitBoX;
  public int health = 0;
  // Update is called once per frame
  void Update()
  {

  }

  public void TakeDamage(int dmgAndDirection, int power)
  {
    health += power;
    frankyHitBoX.velocity = transform.right * (dmgAndDirection * health);

  }


}
