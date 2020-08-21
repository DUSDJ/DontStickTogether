using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateArrive : IState<Human>
{
    Human parent;
    IEnumerator coroutine;

    IEnumerator UpdateCoroutine()
    {
        // 연출부
        // 임시로 Idle Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Idle")
            {
                parent.anim.SetTrigger("Idle");
            }
        }


        // 컨셉 1 : 1초당 공격을 한다        
        // 컨셉 2 : 애니메이션 콜백을 기다린다.(여기선 아무것도 안 함)
        if (parent.AttackWithAnimation == false)
        {
            float t = 0;

            while (true)
            {
                if (t >= parent.AttackCoolDown)
                {
                    parent.Attack();
                    t -= parent.AttackCoolDown;
                }
                else
                {
                    t += Time.deltaTime;
                }

                yield return null;
            }
        }


    }

    public void EnterState(Human t)
    {
        parent = t;
        if (parent.gameObject.activeSelf == false) return;

        coroutine = UpdateCoroutine();
        parent.StartCoroutine(coroutine);
    }

    public void ExitState()
    {
        if (coroutine != null) parent.StopCoroutine(coroutine);

        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Idle")
            {
                parent.anim.ResetTrigger("Idle");
            }
        }
    }
}
