using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : MonoBehaviour
{
 
   float speedX = 20f;
   public Transform position;
   public Rigidbody2D test;
    // Update is called once per frame
    void Update()
    {
        
    }
 
    public void testing()
    {
       test.velocity = transform.right * speedX;
    }

    
}
