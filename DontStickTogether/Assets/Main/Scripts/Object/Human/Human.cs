using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Human : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, ITouchable
{

    #region Values

    public float MoveSpeed;

    [Header("FlipX 참일때 오른쪽을 보는가?")]
    public bool RightIsFlipTrue;

    [Header("애니메이션 기반 액션인지?")]
    public bool AttackWithAnimation = false;

    [Header("공격 쿨다운: 기본1초")]
    public float AttackCoolDown = 1.0f;

    [Header("공격력(오염증가량)")]
    public float AttackPoint = 1.0f;

    [Header("얼마나 멀리 던져지는지(이동거리 비례 추가량)")]
    public float PushRate = 0.1f;

    [HideInInspector]
    public bool Dragable;

    //[HideInInspector]
    public bool PaternRandom = false;

    public Touch MyTouch;

    #endregion

    #region Component

    public SpriteRenderer spr;
    public Animator anim;
    public CircleCollider2D circle;

    #endregion


    #region GameLogics

    [HideInInspector]
    public Transform Target;

    #endregion


    #region StateMachine
    
    public IState<Human> NowState;

    public struct StructHumanState
    {
        public HumanStateIdle HumanStateIdle;
        public HumanStateWalk HumanStateWalk;
        public HumanStateDrag HumanStateDrag;
        public HumanStateArrive HumanStateArrive;
    }
    public StructHumanState HumanState;

    public void ChangeState(IState<Human> nextState)
    {
        if (NowState != null)
        {
            NowState.ExitState();

        }

        if (NowState == nextState)
        {
            return;
        }

        NowState = nextState;

        gameObject.name = nextState.GetType().Name;

        nextState.EnterState(this);
    }

    #endregion



    #region Set Human

    public virtual void Awake()
    {
        /* State Set */
        HumanState.HumanStateIdle = new HumanStateIdle();
        HumanState.HumanStateWalk = new HumanStateWalk();
        HumanState.HumanStateDrag = new HumanStateDrag();
        HumanState.HumanStateArrive = new HumanStateArrive();

        /* Component Set */
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();

    }

    /// <summary>
    /// 최종 - CSV 기반
    /// </summary>
    /// <param name="position"></param>
    /// <param name="speed"></param>
    /// <param name="attackPoint"></param>
    public virtual void InitHuman(Vector3 position, float speed, float attackPoint, string pattern)
    {
        Dragable = true;

        transform.position = position;

        if (speed < 1)
        {
            speed = 1;
        }

        if (attackPoint < 1)
        {
            attackPoint = 1;
        }

        if (pattern.Equals("Random")) 
        {
            PaternRandom = true;
        }
        else // Normal
        {
            PaternRandom = false;
        }

        MoveSpeed = speed;
        AttackPoint = attackPoint;


        /*
         * 애니메이션 속도
         * 1 = 1
         * 5 = 3
         */

        anim.speed = 0.5f + (speed * 0.5f);

        NowState = null;
        ChangeState(HumanState.HumanStateIdle);
    }

    #endregion


    #region Actions

    /// <summary>
    /// Human이 센터를 찾는 함수. 기본은 거리 + 0.5초
    /// </summary>
    /// <returns></returns>
    public virtual float SearchAndGetTime()
    {
        Transform target = GameManager.Instance.Center;
        float distance = (target.position - transform.position).magnitude;
        float AccuratedTime = distance + 0.5f;

        this.Target = target;

        return AccuratedTime;
    }


    /// <summary>
    /// ArriveState 또는 Animation Callback으로 호출
    /// </summary>
    public virtual void Attack()
    {
        // 기본 공격
        GameManager.Instance.NowBioHazard += AttackPoint;
    }   

    #endregion



    #region Drag & Drop (일단 멀티터치 배제)

    public virtual void DragOn()
    {
        SoundManager.Instance.SetPool("Pick");
    }

    public virtual void DragOff()
    {
        SoundManager.Instance.SetPool("Pick");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (InputManager.Instance.InputTest == false)
        {
            return;
        }

        // Drag 불가능한 상태면 취소 : Dragable false
        if (Dragable == false)
        {
            return;
        }

        DragOn();

        ChangeState(HumanState.HumanStateDrag);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(InputManager.Instance.InputTest == false)
        {
            return;
        }

        // Drag 중이 아니면 취소
        if (NowState != HumanState.HumanStateDrag)
        {
            return;
        }

        DragOff();
        

        // IdleState로 전환
        ChangeState(HumanState.HumanStateIdle);
        
    }

    public void TouchBegan(Touch t)
    {
        // Drag 불가능한 상태면 취소 : Dragable false
        if (Dragable == false)
        {
            return;
        }

        MyTouch = t;

        DragOn();

        ChangeState(HumanState.HumanStateDrag);
    }

    public void TouchMove(Touch t)
    {
        MyTouch = t;
    }

    public void TouchEnded(Touch t)
    {
        // Drag 중이 아니면 취소
        if (NowState != HumanState.HumanStateDrag)
        {
            return;
        }

        // 이 손가락이 아니면 취소
        if(t.fingerId != MyTouch.fingerId)
        {
            return;
        }

        DragOff();


        // IdleState로 전환
        ChangeState(HumanState.HumanStateIdle);
    }




    #endregion





}
