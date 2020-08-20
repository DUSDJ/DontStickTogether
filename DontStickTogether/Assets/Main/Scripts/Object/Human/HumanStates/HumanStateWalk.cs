using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateWalk : IState<Human>
{

    Human parent;
    IEnumerator coroutine;

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

            Walk();

            yield return null;
        }

        
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

        // Walk Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Walk")
            {
                parent.anim.SetBool("Walk", false);
            }
        }
    }
}

