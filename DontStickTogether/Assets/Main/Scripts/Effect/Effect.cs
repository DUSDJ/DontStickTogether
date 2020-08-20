using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect : MonoBehaviour
{

    public float Duration;  // 지속시간
    private float BaseDuration; // 기본 시간

    public Transform FollowTarget; // 따라다닐 타겟
    public Vector3 Adder; // Offset

    private SpriteRenderer spr;
    private Animator anim;
    private ParticleSystem ps;

    IEnumerator coroutine;

    private void Awake()
    {
        BaseDuration = Duration;
        Adder = Vector3.zero;

        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        ps = GetComponent<ParticleSystem>();
    }
    public void SetEffect(Vector3 position)
    {
        Duration = BaseDuration;
        transform.position = position;
        Action();
    }

    public void SetEffect(Vector3 position, float duration)
    {
        transform.position = position;
        Duration = duration;
        Action();
    }

    public void SetEffect(Transform t)
    {
        FollowTarget = t;
        Duration = BaseDuration;
        transform.position = FollowTarget.position + Adder;
        Action();
    }


    public void SetEffect(Transform t, float duration)
    {
        FollowTarget = t;
        Duration = duration;
        transform.position = FollowTarget.position + Adder;
        Action();
    }

    public void SetEffect(Transform t, Vector3 adder, float duration)
    {
        Adder = adder;
        SetEffect(t, duration);
    }


    public void Action()
    {
        coroutine = UpdateCoroutine();
        StartCoroutine(coroutine);

    }


    IEnumerator UpdateCoroutine()
    {
        float t = 0;

        while (t < Duration)
        {
            if (FollowTarget != null)
            {
                if (FollowTarget.gameObject.activeSelf == false)
                {
                    Clean();
                }
                else
                {
                    transform.position = FollowTarget.position + Adder;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        Clean();
    }

    public void Clean()
    {
        gameObject.SetActive(false);
        FollowTarget = null;
        Adder = Vector3.zero;
    }
}
