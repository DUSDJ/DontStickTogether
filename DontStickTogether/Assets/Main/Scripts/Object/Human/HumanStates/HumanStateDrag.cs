using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateDrag : IState<Human>
{

    Human parent;
    IEnumerator coroutine;

    private bool IsDragging;
    private Vector3 pointDragStart; // 드래그 이동거리 비례해서 뭔가 하고 싶을 때


    IEnumerator UpdateCoroutine()
    {
        // Action Animation
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Idle")
            {                
                parent.anim.ResetTrigger("Idle");
            }
        }
        foreach (AnimatorControllerParameter param in parent.anim.parameters)
        {
            if (param.name == "Action")
            {
                parent.anim.SetTrigger("Action");
            }
        }

        while (true)
        {
            if (IsDragging == true)
            {                
                if(InputManager.Instance.InputTest == true)
                {
                    Vector2 MousePosition = Input.mousePosition;
                    MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);
                    parent.transform.position = MousePosition;
                }
                else
                {
                    Vector2 pos = InputManager.Instance.GetTouchPosition(parent.MyTouch);

                    Debug.Log("Pos : " + pos);

                    parent.transform.position = pos;
                }
            }

            yield return null;
        }

    }

    public void EnterState(Human t)
    {
        parent = t;
        if (parent.gameObject.activeSelf == false) return;

        IsDragging = true;
        pointDragStart = parent.transform.position;

        coroutine = UpdateCoroutine();
        parent.StartCoroutine(coroutine);
    }

    public void ExitState()
    {
        if (coroutine != null) parent.StopCoroutine(coroutine);

        IsDragging = false;
    }
}
