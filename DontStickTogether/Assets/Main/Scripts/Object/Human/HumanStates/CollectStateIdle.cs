using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CollectStateIdle : HumanStateIdle
{
    Collector collectorParent;
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



        while (true)
        {
            // 모두 모일 때까지 계속 Idle임.
            if (collectorParent.SummonedAllFans() == true)
            {
                /* 가운데를 찾는다 */
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

                collectorParent.ChangeState(collectorParent.HumanState.HumanStateWalk);
            }
            else
            {
                // 소환중!
                if (collectorParent.LeftTimeOfSummon > 0)
                {
                    collectorParent.LeftTimeOfSummon -= Time.deltaTime;

                    if(effects.Count == 0)
                    {
                        //Effect_MusicNote
                        Effect e = EffectManager.Instance.SetPool("Effect_MusicNote");
                        Vector3 Adder = new Vector3(0, 0.3f, 0);
                        e.SetEffect(parent.transform, Adder, collectorParent.LeftTimeOfSummon);
                        effects.Add(e);
                    }

                }
                // 숫자 검사는 위에서 했으니까 소환하고 추가. 첫 친구는 그냥 바로 소환?
                else
                {
                    // 현재 스테이트에 걸려있는 이펙트 제거
                    for (int i = 0; i < effects.Count; i++)
                    {
                        effects[i].Clean();
                    }
                    effects.Clear();

                    collectorParent.Summon();
                    collectorParent.LeftTimeOfSummon = collectorParent.TimeOfSummon;
                }

                yield return null;
            }
        }

    }


    public override void EnterState(Human t)
    {
        parent = t;
        collectorParent = (Collector)t;

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
