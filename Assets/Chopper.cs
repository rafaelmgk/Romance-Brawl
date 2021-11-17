using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : MonoBehaviour
{
 
   
   public Transform position;
   public Rigidbody2D chopperHitBoX;
    // Update is called once per frame
    void Update()
    {
        
    }
 
    public void TakeDamage(int dmgAndDirection)
    {
       chopperHitBoX.velocity = transform.right * dmgAndDirection;
    }

    
}
