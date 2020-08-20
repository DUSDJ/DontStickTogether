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

        // 도착한 사람이 뭘 할지

        yield return null;
    }

    public void EnterState(Human t)
    {
        parent = t;
        if (parent.gameObject.activeSelf == false) return;

        parent.Dragable = false;

        coroutine = UpdateCoroutine();
        parent.StartCoroutine(coroutine);
    }

    public void ExitState()
    {
        if (coroutine != null) parent.StopCoroutine(coroutine);
    }
}
