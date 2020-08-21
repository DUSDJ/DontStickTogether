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
    private BoxCollider2D box;

    #endregion


    private void Awake()
    {
        /* State Set */

        /* Component Set */
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();


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

    private void OnTriggerExit2D(Collider2D collision)
    {
        /* 만들고 보니까 Exit는 알 바가 아님... */
        /*
        if (collision.CompareTag("Human"))
        {
            Human h = collision.GetComponent<Human>();

            // Drag 중일때만 처리?
            if (h.NowState != h.HumanState.HumanStateDrag)
            {
                return;
            }

            GameManager.Instance.RemoveHuman(h);
        }
        */
    }

    #endregion


    #region Actions

    // Center가 State로 인해 행할 특수한 액션?
    public virtual void Action()
    {

    }



    #endregion

}
