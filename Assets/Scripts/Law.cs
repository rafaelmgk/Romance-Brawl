using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Law : MonoBehaviour
{


  public Transform position;
  public Rigidbody2D lawHitBoX;
  public int health = 0;
  // Update is called once per frame
  void Update()
  {

  }

  public void TakeDamage(int dmgAndDirection, int power)
  {
    health += power;
    lawHitBoX.velocity = new Vector3(dmgAndDirection * health, health / 5, 0);
  }


}
