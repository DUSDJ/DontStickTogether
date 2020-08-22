using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insane : Human
{
    
    public override void Awake()
    {
        base.Awake();

        HumanState.HumanStateWalk = new HumanStateWalk();
    }
}
