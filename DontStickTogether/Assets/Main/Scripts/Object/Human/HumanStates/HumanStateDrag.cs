using DG.Tweening;
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
    private float dragTime;

    private Transform trail;

    IEnumerator UpdateCoroutine()
    {
        dragTime = 0f;

        // Effect : 트레일 시작
        trail = EffectManager.Instance.SetTrailPool();
        trail.position = Vector3.zero;
        trail.SetParent(parent.transform, false);
        trail.GetComponent<TrailRenderer>().Clear();
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
                dragTime += Time.deltaTime;

                // Effect : 트레일 도중 -> 트레일 오브젝트 회전
                trail.transform.position = parent.transform.position;

                Vector3 pointDragging = parent.transform.position;
                Vector3 diffrence = pointDragging - pointDragStart;
                Vector3 direction = diffrence.normalized;
                LookTarget(trail);


                Debug.Log(dragTime);
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

    public void LookTarget(Transform t)
    {
        Vector3 ShootPoint = parent.transform.position;

        float AngleRad = Mathf.Atan2(ShootPoint.y - pointDragStart.y, ShootPoint.x - pointDragStart.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        t.rotation = Quaternion.Euler(0, 0, AngleDeg + (-90));

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

        // Effect : Trail 완료
        trail.SetParent(EffectManager.Instance.transform, false);
        trail.gameObject.SetActive(false);

        // 튕겨나간다. (distance 0.15f 이상일 때만)
        Vector3 pointDragEnd = parent.transform.position;

        Vector3 diffrence = pointDragEnd - pointDragStart;
        Vector3 direction = diffrence.normalized;
        float distance = diffrence.magnitude;

        if(distance < 0.15f)
        {
            return;
        }

        // dragTime에 반비례해서 dircetion으로 이동 보간
        // 보너스값은 0.2초를 넘어갈수록 줄어든다. 최대 1f 추가 0에 가까울수록 1.5, 아니면 0
        float power = Mathf.Lerp(0f, 1f, 0.2f / dragTime);

        Vector3 endValue = pointDragEnd + direction * (distance + power) * parent.PushRate;

        // distance에 비례해서 direction으로 이동 보간
        parent.transform.DOMove(endValue, 0.12f).SetEase(Ease.OutBounce);
    }
}
