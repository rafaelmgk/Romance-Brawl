using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreLayerColisions : MonoBehaviour
{

  void Start()
  {
    Physics2D.IgnoreLayerCollision(6, 3, true);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
