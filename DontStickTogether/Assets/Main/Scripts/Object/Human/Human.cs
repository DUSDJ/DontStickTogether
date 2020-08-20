using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Human : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    #region Values

    public float MoveSpeed;

    [Header("FlipX 참일때 오른쪽을 보는가?")]
    public bool RightIsFlipTrue;

    [HideInInspector]
    public bool Dragable;

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


    public virtual void InitHuman(Vector3 position)
    {
        Dragable = true;

        transform.position = position;

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

    #endregion



    #region Drag & Drop (일단 멀티터치 배제)

    public virtual void DragOn()
    {

    }

    public virtual void DragOff()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
        // Drag 중이 아니면 취소
        if (NowState != HumanState.HumanStateDrag)
        {
            return;
        }

        DragOff();
        

        // IdleState로 전환
        ChangeState(HumanState.HumanStateIdle);

    }




    #endregion





}
