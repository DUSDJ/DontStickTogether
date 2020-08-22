using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{

    #region StateMachine

    public IState<Center> NowState;

    public struct StructCenterState
    {

    }
    public StructCenterState CenterState;

    public void ChangeState(IState<Center> nextState)
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

    #region private

    private SpriteRenderer spr;
    private Animator anim;
    private CapsuleCollider2D capsule;

    #endregion


    private void Awake()
    {
        /* State Set */

        /* Component Set */
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider2D>();


    }

    // 레벨매니저에서 생성할 경우
    public void InitCenter()
	{

    }

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Human"))
        {
            Human h = collision.GetComponent<Human>();
            
            // Drag 중이면 처리 안 함
            if(h.NowState == h.HumanState.HumanStateDrag)
            {
                return;
            }

            GameManager.Instance.AddHuman(h);
        }


        // Fan은 Collector가 Arrive 상태일 때만 Center에 접촉
        if (collision.CompareTag("Fan"))
        {
            Fan f = collision.GetComponent<Fan>();

            // Drag 중이면 처리 안 함
            if (f.NowState == f.HumanState.HumanStateDrag)
            {
                return;
            }

            // Collector 따라다니는 상태면 처리 안 함
            if (f.IsTargetingCenter() == false)
            {
                return;
            }

            GameManager.Instance.AddHuman(f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Human"))
        {
            Human h = collision.GetComponent<Human>();

            // Drag 중이면 처리 안 함
            if (h.NowState == h.HumanState.HumanStateDrag)
            {
                return;
            }

            // 안에 있는데 도착상태가 아닐때
            if(h.NowState != h.HumanState.HumanStateArrive)
            {
                GameManager.Instance.AddHuman(h);
            }
        }
    }

    #endregion


    #region Actions

    // Center가 State로 인해 행할 특수한 액션?
    public virtual void Action()
    {

    }



    #endregion

}
