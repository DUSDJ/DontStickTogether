using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebel : Human
{
    public float RebelTime = 0.3f;
    public float RebelDistance = 0.5f;
    public float AngryEffectDuration = 1.2f;

    public override void Awake()
    {
        base.Awake();

        HumanState.HumanStateDrag = new RebelStateDrag();

    }

    public void AfterEscape()
    {
        Target = GameManager.Instance.Center;

        ChangeState(HumanState.HumanStateWalk);
    }
}
