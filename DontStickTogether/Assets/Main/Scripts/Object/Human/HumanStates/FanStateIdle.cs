using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FanStateIdle : HumanStateIdle
{
    Fan fanParent;
    Human parent;
    IEnumerator coroutine;

    List<Effect> effects = new List<Effect>();

    IEnumerator UpdateCoroutine()
    {
        // Idle Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Idle")
            {
                parent.anim.SetTrigger("Idle");
            }
        }

        float AccuratedTime;

        // To Center
        if (fanParent.IsTargetingCenter() == true)
        {
            // 거리 비례하여 시간을 잰다. Target도 설정.
            AccuratedTime = fanParent.SearchAndGetTime();
        }
        // To Collector
        else
        {
            // 거리 비례하여 시간을 잰다. Target도 설정.
            AccuratedTime = fanParent.SearchAndGetTimeFromCollector();
        }

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

        parent.ChangeState(fanParent.HumanState.HumanStateWalk);

        yield return null;
    }

    public override void EnterState(Human t)
    {
        parent = t;
        fanParent = (Fan)t;

        if (parent.gameObject.activeSelf == false) return;

        coroutine = UpdateCoroutine();
        parent.StartCoroutine(coroutine);
    }

    public override void ExitState()
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
