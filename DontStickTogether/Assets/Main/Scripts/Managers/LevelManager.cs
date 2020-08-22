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

    public int NowLevel = 1;

    #region 레벨 그래픽 관련

    public Dictionary<string, GameObject> CenterDic;
    public Dictionary<string, Sprite> BuildingDic;
    public Dictionary<string, Sprite> GroundDic;

    [HideInInspector] public SpriteRenderer[] Buildings;
    [HideInInspector] public SpriteRenderer Ground;

    #endregion

    #region 

    public List<Dictionary<string, object>> LevelTable;

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


        /* Level Data CSV Init */
        LevelTable = CSVReader.Read("LevelCSV/LevelSheet");

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
        Dictionary<string, object> table = GetLevelCSV();

        // Background Set
        string backgroundKey = table["BackgroundKey"].ToString();
        if (GroundDic.ContainsKey(backgroundKey))
        {
            Ground.sprite = GroundDic[backgroundKey];
        }
        else
        {
            Debug.LogError("backgroundKey 잘못되어있습니다. Resources/Ground 폴더의 스프라이트 이름과 CSV 파일이 일치하는지 확인하세요.");
        }

        // Center Set
        string centerKey = table["CenterKey"].ToString();

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
            Debug.LogError("centerKey가 잘못되어있습니다. 프리팹 이름과 CSV 파일이 일치하는지 확인하세요.");
        }
        
    }


    public Dictionary<string, object> GetLevelCSV()
    {
        // 레벨 초과
        if (NowLevel > LevelTable.Count)
        {
            return null;
        }

        UIManager.Instance.UpdateChapter(int.Parse(LevelTable[NowLevel - 1]["Chapter"].ToString()));
        UIManager.Instance.UpdateStage(int.Parse(LevelTable[NowLevel - 1]["Stage"].ToString()));

        return LevelTable[NowLevel - 1];
    }

}
