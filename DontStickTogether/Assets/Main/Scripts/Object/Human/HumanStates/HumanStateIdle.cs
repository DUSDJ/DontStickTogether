using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HumanStateIdle : IState<Human>
{
    Human parent;
    IEnumerator coroutine;

    Transform target;

    List<Effect> effects = new List<Effect>();

    IEnumerator UpdateCoroutine()
    {
        // Idle Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Action")
            {
                parent.anim.ResetTrigger("Action");
            }
        }
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Idle")
            {
                parent.anim.SetTrigger("Idle");
            }
        }



        /* 가운데를 찾는다? */
        // 센터와 거리 비례하여 시간을 잰다. 이때 Target도 결정
        float AccuratedTime = parent.SearchAndGetTime();

        //Effect_Question
        Effect e = EffectManager.Instance.SetPool("Effect_Question");
        Vector3 Adder = new Vector3(0, 0.3f, 0);
        e.SetEffect(parent.transform, Adder, AccuratedTime);        
        effects.Add(e);

        float t = 0;
        while (t < AccuratedTime)
        {
            t += Time.deltaTime;

            yield return null;
        }

        parent.ChangeState(parent.HumanState.HumanStateWalk);

        yield return null;
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

        // 현재 스테이트에 걸려있는 이펙트 제거

        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].Clean();
        }

        effects.Clear();
    }
}
