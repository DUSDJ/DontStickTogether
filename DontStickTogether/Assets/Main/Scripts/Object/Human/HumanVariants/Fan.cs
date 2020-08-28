using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Human
{
    /*
     * Fan은 Tag가 Fan이다.
     * Fan도 탐지하게 Center 코드 수정
     * 대신 조건부임.
     *
     * Fan은 일단 Collector의 곁으로 다가간다.
     * FanStateIdle에서 절대타겟이 Collector
     * 이때, Collector가 ArriveState라면 타겟을 원으로 바꾼다.
     * 
     * FanStateWalk에서 타겟으로만 이동
     * 이때, Collector가 ArriveState라면 타겟을 원으로 바꾼다.
     * 아니라면 다시 절대타겟을 변경.
     * 
    */

    public Collector MyCollector;

    public override void Awake()
    {
        base.Awake();

        //FanStateIdle
        //FanStateWalk
        HumanState.HumanStateIdle = new FanStateIdle();
        HumanState.HumanStateWalk = new FanStateWalk();
        

    }

    public override void InitHuman(Vector3 position, float speed, float attackPoint, string pattern)
    {
        base.InitHuman(position, speed, attackPoint, pattern);

        PaternRandom = false;
    }

    public bool IsTargetingCenter()
    {
        if(MyCollector.NowState == MyCollector.HumanState.HumanStateArrive)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public float SearchAndGetTimeFromCollector()
    {
        Transform target = MyCollector.transform;
        float distance = (target.position - transform.position).magnitude;
        float AccuratedTime = distance + 0.1f;

        this.Target = target;

        return AccuratedTime;
    }

}
