using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FanStatewalk : HumanStateWalk
{
    Fan fanParent;
    Human parent;
    IEnumerator coroutine;

    List<Effect> effects = new List<Effect>();

    IEnumerator UpdateCoroutine()
    {
        // Walk Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Walk")
            {
                parent.anim.SetBool("Walk", true);
            }
        }

        while (true)
        {
            // Collector가 도착했는지

            if (fanParent.IsTargetingCenter() == true)
            {
                // 현재 스테이트에 걸려있는 이펙트 제거
                for (int i = 0; i < effects.Count; i++)
                {
                    effects[i].Clean();
                }

                effects.Clear();

                // Center로 간다.
                WalkToCenter();

            }
            else
            {
                // Effect : Heart
                if (effects.Count == 0)
                {
                    //Effect_MusicNote
                    Effect e = EffectManager.Instance.SetPool("Effect_MusicNote");
                    Vector3 Adder = new Vector3(0, 0.3f, 0);
                    e.SetEffect(parent.transform, Adder, 100.0f);
                    effects.Add(e);
                }

                // Distance 이내로 들어간다.
                WalkToCollector();

            }

            yield return null;
        }
    }

    public void WalkToCenter()
    {
        Transform t = GameManager.Instance.Center;

        if (t == null) return;

        float movement = parent.MoveSpeed * Time.deltaTime;

        Vector3 positionDifference = t.position - parent.transform.position;
        float distance = positionDifference.magnitude;
        Vector3 direction = positionDifference.normalized;
        direction.z = 0;

        if (direction.x < 0)
        {
            Flip(false);
        }
        else
        {
            Flip(true);
        }

        if (distance < 0.1f)
        {
            return;
        }

        parent.transform.Translate(direction * movement);
    }

    public void WalkToCollector()
    {
        Transform t = fanParent.MyCollector.transform;

        if (t == null) return;

        float movement = parent.MoveSpeed * Time.deltaTime;

        Vector3 positionDifference = t.position - parent.transform.position;
        float distance = positionDifference.magnitude;
        Vector3 direction = positionDifference.normalized;
        direction.z = 0;

        if (direction.x < 0)
        {
            Flip(false);
        }
        else
        {
            Flip(true);
        }

        if (distance <= fanParent.MyCollector.MinDistanceOfFans)
        {
            return;
        }

        parent.transform.Translate(direction * movement);
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

        // Walk Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Walk")
            {
                parent.anim.SetBool("Walk", false);
            }
        }

        // 현재 스테이트에 걸려있는 이펙트 제거
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].Clean();
        }

        effects.Clear();
    }
}

