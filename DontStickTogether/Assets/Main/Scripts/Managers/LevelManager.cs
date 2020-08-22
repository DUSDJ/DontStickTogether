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

    #region 레벨 그래픽 관련

    public Dictionary<string, GameObject> CenterDic;
    public Dictionary<string, Sprite> BuildingDic;
    public Dictionary<string, Sprite> GroundDic;

    [HideInInspector] public SpriteRenderer[] Buildings;
    [HideInInspector] public SpriteRenderer Ground;

    #endregion


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

        /* Center Data Init */
        CenterDic = new Dictionary<string, GameObject>();
        GameObject[] centerDatas = Resources.LoadAll<GameObject>("Center/");
        for (int i = 0; i < centerDatas.Length; i++)
        {
            CenterDic.Add(centerDatas[i].name, centerDatas[i]);
        }

        /* Building Data Init */
        BuildingDic = new Dictionary<string, Sprite>();
        Sprite[] buildingDatas = Resources.LoadAll<Sprite>("Building/");
        for (int i = 0; i < buildingDatas.Length; i++)
        {
            BuildingDic.Add(buildingDatas[i].name, buildingDatas[i]);
        }

        /* Ground Data Init */
        GroundDic = new Dictionary<string, Sprite>();
        Sprite[] groundDatas = Resources.LoadAll<Sprite>("Ground/");
        for (int i = 0; i < groundDatas.Length; i++)
        {
            GroundDic.Add(groundDatas[i].name, groundDatas[i]);
        }

        /* Building Object Init */
        Transform[] buildings = HumanManager.Instance.Buildings;
        Buildings = new SpriteRenderer[buildings.Length];
        for (int i = 0; i < buildings.Length; i++)
        {
            Buildings[i] = buildings[i].GetComponent<SpriteRenderer>();
        }

        /* Ground Object Init */        
        Ground = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
    }

    public void SetLevel()
    {
        // Get Level
        ScriptableLevel sl = GetLevel();

        // Ground Set
        string groundKey = Enum.GetName(typeof(EnumGround), sl.Ground);
        if (GroundDic.ContainsKey(groundKey))
        {
            Ground.sprite = GroundDic[groundKey];
        }
        else
        {
            Debug.LogError("groundKey 잘못되어있습니다. Resources/Ground 폴더의 스프라이트 이름과 Enum 이름이 일치하는지 확인하세요.");
        }

        // Center Set
        string centerKey = Enum.GetName(typeof(EnumCenter), sl.Center);

        if (CenterDic.ContainsKey(centerKey))
        {
            GameObject c = Instantiate<GameObject>(CenterDic[centerKey]);

            if (GameManager.Instance.Center != null)
            {
                GameManager.Instance.Center.gameObject.SetActive(false);
            }

            GameManager.Instance.Center = c.transform;
        }
        else
        {
            Debug.LogError("centerKey가 잘못되어있습니다. 프리팹 이름과 Enum 이름이 일치하는지 확인하세요.");
        }
                

        // Building Set
        for (int i = 0; i < sl.Builginds.Length; i++)
        {
            // None Check
            if (sl.Builginds[i] == EnumBulding.None)
            {
                Buildings[i].enabled = false;
                continue;
            }
            else
            {
                Buildings[i].enabled = true;
            }

            string buildingKey = Enum.GetName(typeof(EnumBulding), sl.Builginds[i]);
            if (BuildingDic.ContainsKey(buildingKey))
            {
                Buildings[i].sprite = BuildingDic[buildingKey];
            }
            else
            {
                Debug.LogError("buildingKey 잘못되어있습니다. Resources/Building 폴더의 스프라이트 이름과 Enum 이름이 일치하는지 확인하세요.");
            }

        }
        
        


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
