using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebelStateDrag : HumanStateDrag
{
    Rebel rebelParent;

    public override IEnumerator UpdateCoroutine()
    {
        dragTime = 0f;

        // Effect : 트레일 시작
        trail = EffectManager.Instance.SetTrailPool();
        trail.position = Vector3.zero;
        trail.SetParent(rebelParent.transform, false);
        trail.GetComponent<TrailRenderer>().Clear();
        // Action Animation
        foreach (AnimatorControllerParameter param in rebelParent.anim.parameters)
        {
            if (param.name == "Idle")
            {
                rebelParent.anim.ResetTrigger("Idle");
            }
        }
        foreach (AnimatorControllerParameter param in rebelParent.anim.parameters)
        {
            if (param.name == "Action")
            {
                rebelParent.anim.SetTrigger("Action");
            }
        }

        while (true)
        {
            if (IsDragging == true)
            {
                dragTime += Time.deltaTime;

                // Escape!
                if(dragTime >= rebelParent.RebelTime)
                {
                    // Effect : Trail 완료
                    trail.SetParent(EffectManager.Instance.transform, false);
                    trail.gameObject.SetActive(false);

                    // 효과음

                    // 이펙트 : 임시로 느낌표...
                    //Effect_Question
                    Effect e = EffectManager.Instance.SetPool("Effect_Angry");
                    Vector3 Adder = new Vector3(0, 0.3f, 0);
                    e.SetEffect(parent.transform, Adder, rebelParent.AngryEffectDuration);
                    

                    //애니메이션
                    foreach (AnimatorControllerParameter param in rebelParent.anim.parameters)
                    {
                        if (param.name == "Escape")
                        {
                            rebelParent.anim.SetTrigger("Escape");
                        }
                    }

                    // 아래쪽으로 튕겨나감
                    Vector3 dragEnd = rebelParent.transform.position;
                    rebelParent.transform.DOMove(dragEnd + Vector3.down * rebelParent.RebelDistance, 0.1f).SetEase(Ease.OutQuad);

                    // 애니메이션 콜백에서 타겟설정하고 걷기로 전환
                    yield break;                    
                }

                // Effect : 트레일 도중 -> 트레일 오브젝트 회전
                trail.transform.position = rebelParent.transform.position;

                Vector3 pointDragging = rebelParent.transform.position;
                Vector3 diffrence = pointDragging - pointDragStart;
                Vector3 direction = diffrence.normalized;
                LookTarget(trail);


                if (InputManager.Instance.InputTest == true)
                {
                    Vector2 MousePosition = Input.mousePosition;
                    MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);
                    rebelParent.transform.position = MousePosition;
                }
                else
                {
                    Vector2 pos = InputManager.Instance.GetTouchPosition(rebelParent.MyTouch);

                    rebelParent.transform.position = pos;
                }
            }

            yield return null;
        }

    }

    public override void EnterState(Human t)
    {
        parent = t;
        rebelParent = (Rebel)t;
        if (rebelParent.gameObject.activeSelf == false) return;

        IsDragging = true;
        pointDragStart = rebelParent.transform.position;

        coroutine = UpdateCoroutine();
        rebelParent.StartCoroutine(coroutine);
    }

    public override void ExitState()
    {
        foreach (AnimatorControllerParameter param in rebelParent.anim.parameters)
        {
            if (param.name == "Escape")
            {
                rebelParent.anim.ResetTrigger("Escape");
            }
        }

        base.ExitState();
        
    }
}
