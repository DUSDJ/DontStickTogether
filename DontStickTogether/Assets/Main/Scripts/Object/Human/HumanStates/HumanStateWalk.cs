using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateWalk : IState<Human>
{

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
            // Pattern Insane
            if(parent.PaternRandom == true)
            {

                if(parent.LeftDurationOfInsaneWalk > 0)
                {
                    InsaneWalk();
                }
                else
                {
                    InsaneStart();
                }
            }
            // Pattern Normal
            else
            {
                Walk();
            }

            yield return null;
        }

        
    }

    public void InsaneStart()
    {
        if(parent.NumOfInsaneWalk > 0)
        {
            // 일정 크기 원의 외곽에 포인트 생성

            Vector2 edgePoint = UnityEngine.Random.insideUnitCircle.normalized * parent.RadiusOfInsaneWalk;

            parent.PositionOfInsaneWalk = edgePoint;

            parent.NumOfInsaneWalk -= 1;
            parent.LeftDurationOfInsaneWalk = parent.DuratoinOfInsaneWalk;

            // Effect : Harasing            
            Effect e = EffectManager.Instance.SetPool("Effect_Harasing");
            Vector3 Adder = new Vector3(0, 0.3f, 0);
            e.SetEffect(parent.transform, Adder, parent.LeftDurationOfInsaneWalk);
            effects.Add(e);
        }
        else
        {
            Walk();
        }
        
    }

    public void InsaneWalk()
    {
        Vector3 t = parent.PositionOfInsaneWalk;

        parent.LeftDurationOfInsaneWalk -= Time.deltaTime;

        // Move to t
        float movement = parent.MoveSpeed * Time.deltaTime;

        Vector3 positionDifference = t - parent.transform.position;
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

        if (distance < 0.05f)
        {
            return;
        }

        parent.transform.Translate(direction * movement);

    }

    public void Walk()
    {
        Transform t = parent.Target.transform;

        if (t == null) return;

        float movement = parent.MoveSpeed * Time.deltaTime;

        Vector3 positionDifference = t.position - parent.transform.position;
        float distance = positionDifference.magnitude;
        Vector3 direction = positionDifference.normalized;
        direction.z = 0;

        if(direction.x < 0)
        {
            Flip(false);
        }
        else
        {
            Flip(true);
        }

        if(distance < 0.1f)
        {
            return;
        }

        parent.transform.Translate(direction * movement);
    }

    public void Flip(bool TrueIsRight)
    {
        if(TrueIsRight == true)
        {
            // Right
            parent.spr.flipX = parent.RightIsFlipTrue;
        }
        else
        {
            // Left
            parent.spr.flipX = !parent.RightIsFlipTrue;
        }

    }

    public virtual void EnterState(Human t)
    {
        parent = t;
        if (parent.gameObject.activeSelf == false) return;

        coroutine = UpdateCoroutine();
        parent.StartCoroutine(coroutine);
    }

    public virtual void ExitState()
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

