using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : PlayerController
{
  private Vector2 chopper1Range = new Vector2(0.5f, 0.5f);
  private Vector2 chopper2Range = new Vector2(0.8f, 0.8f);


  private int chopper1Power = 3;
  private int chopper2Power = 6;
  private void Awake()
  {
    attack1Range = chopper1Range;
    attack2Range = chopper2Range;
    atk1Power = chopper1Power;
    atk2Power = chopper2Power;
  }
}
