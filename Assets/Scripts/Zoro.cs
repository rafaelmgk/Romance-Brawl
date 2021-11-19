using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoro : MonoBehaviour
{


  public Transform position;
  public Rigidbody2D zoroHitBoX;
  public int health = 0;
  // Update is called once per frame
  void Update()
  {

  }

  public void TakeDamage(int dmgAndDirection, int power)
  {
    health += power;
    zoroHitBoX.velocity = new Vector3(dmgAndDirection * health, health / 5, 0);

  }


}
