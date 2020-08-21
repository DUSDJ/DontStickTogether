using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Scriptable/Level")]
public class ScriptableLevel : ScriptableObject
{

    [Header("버텨야 하는 시간(초)")]
    public float ClearTime;

    /* 컨셉1 : 매 레벨마다 유닛별 생성 시간만 설정*/
    
    [System.Serializable]
    public struct StructSetHuman
    {
        public EnumHuman Type;
        public float MinTime;
        public float MaxTime;

        

        [Range(1.0f, 5.0f)]public float Speed;
    }

    [Header("자동 레벨 디자인 방식")]
    public StructSetHuman[] SetHuman;
    
    
    /* 컨셉2 : 고정된 초에 고정된 유닛들이 나온다.
     * (포지션 지정 가능하게 바꿀 수 있음)*/
    [System.Serializable]
    public struct StructFixedHuman
    {
        public StructSetFixedHuman[] Data; // 누가 어디 생성될 것인지
    }
    [Header("고정 레벨 디자인 방식")]
    public StructFixedHuman[] FixedHuman; // Size가 곧 초 단위

    [System.Serializable]
    public struct StructSetFixedHuman
    {
        public EnumHuman Type;
        [Range(0, 9)] public int BuildingIndex;
        [Range(1.0f, 5.0f)]public float MoveSpeed;
    }
}
