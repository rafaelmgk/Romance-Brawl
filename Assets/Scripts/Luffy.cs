using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luffy : MonoBehaviour
{
 
   
   public Transform position;
   public Rigidbody2D luffyHitBoX;
    // Update is called once per frame
    void Update()
    {
        
    }
 
    public void TakeDamage(int dmgAndDirection)
    {
       luffyHitBoX.velocity = transform.right * dmgAndDirection;
    }

    
}
