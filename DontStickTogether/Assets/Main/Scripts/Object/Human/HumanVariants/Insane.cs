using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insane : Human
{
    public override void InitHuman(Vector3 position, float speed, float attackPoint, string pattern)
    {
        base.InitHuman(position, speed, attackPoint, pattern);
        
        PaternRandom = true;
    }
}
