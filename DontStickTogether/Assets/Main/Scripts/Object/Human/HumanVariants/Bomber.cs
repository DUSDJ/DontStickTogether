using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Human
{
    [Header("공격력 덮어쓰기")]    
    public float BomberAttackPoint = 20.0f;

    [Header("애니메이션 콜백 설정 덮어쓰기")]    
    public bool BomberAttackWithAnimation = true;


    public override void InitHuman(Vector3 position, float speed, float attackPoint, string pattern)
    {
        base.InitHuman(position, speed, attackPoint, pattern);

        spr.sortingLayerName = "SpecialObject";
        AttackPoint = BomberAttackPoint;
        AttackWithAnimation = BomberAttackWithAnimation;
    }
}
