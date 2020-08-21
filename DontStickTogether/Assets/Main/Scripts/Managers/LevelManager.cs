using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(LevelManager)) as LevelManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "LevelManager";
                    instance = container.AddComponent(typeof(LevelManager)) as LevelManager;
                }
            }

            return instance;
        }
    }

    #endregion

    [Header("고정레벨을 사용할 것인지?")]
    public bool UseFixedLevel = false;

    [HideInInspector]
    public ScriptableLevel[] Levels;

    public int NowLevel = 1;

    private void Awake()
    {
        #region SingleTone

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        #endregion

        ScriptableLevel[] datas = Resources.LoadAll<ScriptableLevel>("Level/");
        Levels = datas;        

    }

    public ScriptableLevel GetLevel()
    {
        // 레벨 초과
        if(NowLevel - 1 >= Levels.Length)
        {
            return null;
        }

        return Levels[NowLevel - 1];
    }

    
}
