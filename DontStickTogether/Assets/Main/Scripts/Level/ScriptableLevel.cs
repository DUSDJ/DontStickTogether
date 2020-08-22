using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Scriptable/Level")]
public class ScriptableLevel : ScriptableObject
{

    [Header("가운데 프리팹")]
    public EnumCenter Center;

    [Header("건물 정보 : 공백시 빈 공간")]
    public EnumBulding[] Builginds;

    [Header("바닥")]
    public EnumGround Ground;

}
