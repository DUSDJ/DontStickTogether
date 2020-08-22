using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : Human
{
    [Header("소환할 친구들 숫자")]
    public int NumOfFans;

    [Header("소환할 범위")]
    public float RadiusOfSummon = 1.0f;

    [Header("1명 소환하는 데 걸리는 시간")]
    public float TimeOfSummon = 2.0f;
    [HideInInspector] public float LeftTimeOfSummon = 0f;

    [Header("친구들이 붙을 거리")]
    public float MinDistanceOfFans = 0.2f;

    [HideInInspector]public List<Fan> FanList;

    /*
     * Idle State가 CollectStateIdle이다.
     * CollectStateIdle에서 Fan을 소환한다.
     *
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

    public override void Awake()
    {
        FanList = new List<Fan>();

        base.Awake();

        // CollectStateIdle
        HumanState.HumanStateIdle = new CollectStateIdle();
    }

    public override void InitHuman(Vector3 position, float speed, float attackPoint, string pattern)
    {
        FanList.Clear();

        LeftTimeOfSummon = TimeOfSummon;

        base.InitHuman(position, speed, attackPoint, pattern);
    }

    public void Summon()
    {
        // Fan은 일단 Collect의 능력치를 복사함.
        Fan f = HumanManager.Instance.SetPoolFan(this);
        if(f == null)
        {
            return;
        }

        f.MyCollector = this;
        FanList.Add(f);
    }

    public Vector2 GetSummonRadius()
    {
        Vector2 edgePoint = UnityEngine.Random.insideUnitCircle.normalized * RadiusOfSummon;
        return (Vector2)transform.position + edgePoint;
    }

    public bool SummonedAllFans()
    {
        if(FanList.Count >= NumOfFans)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
