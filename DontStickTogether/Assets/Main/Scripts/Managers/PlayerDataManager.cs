using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    [Header("세이브경로 데스크톱용 Assets로 설정")]
    public bool ChangeSavePathForTest = false;

    [Header("게임 시작시 세이브데이터 리셋할지")]
    public bool DontLoadData = false;

    [HideInInspector]public bool WillYouLoadData = false;

    #region SingleTon
    /* SingleTon */
    private static PlayerDataManager instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(PlayerDataManager)) as PlayerDataManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "PlayerDataManager";
                    instance = container.AddComponent(typeof(PlayerDataManager)) as PlayerDataManager;
                }
            }

            return instance;
        }
    }

    #endregion



    public PlayerData LoadPD;
    public PlayerData SavePD;

    #region Inner Class

    [System.Serializable]
    public class PlayerData
    {
        public int Level; // 최대 어디까지 진행했는지.

        public float BGMVolume;
        public float SoundEffectVolume;
    }

    #endregion


    

    private void Awake()
    {
        #region SingleTone

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        #endregion


        SavePD = new PlayerData();

        LoadPD = new PlayerData();
        LoadPD.BGMVolume = 1.0f;
        LoadPD.SoundEffectVolume = 1.0f;
        LoadPD.Level = 1;

        SceneManager.sceneLoaded += LevelLoadAction;


    }

    private void Start()
    {
       
    }

    public void LevelLoadAction(Scene scene, LoadSceneMode mode)
    {
        LoadPlayerDataFromJson();
    }


    public void ResetData()
    {
        LevelManager.Instance.NowLevel = 1;
        SavePD.Level = 1;
        LoadPD.Level = 1;

        SaveData();
    }


    #region Update Save Data

    /// <summary>
    /// 게임 클리어해서 레벨+1 이후 저장해야함.
    /// </summary>
    public void UpdateSaveData()
    {
        if(LoadPD.Level <= LevelManager.Instance.NowLevel)
        {
            SavePD.Level = LevelManager.Instance.NowLevel;

            SaveData();
        }
    }

    public void UpdateVolumeData()
    {
        SavePD.BGMVolume = SoundManager.Instance.BGMVolume;
        SavePD.SoundEffectVolume = SoundManager.Instance.SoundEffectVolume;

        SaveData();
    }

    #endregion

    #region To Json, From Json

    /// <summary>
    /// 경로에 JSON 파일로 저장
    /// </summary>
    [ContextMenu("To Json Data")]
    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(SavePD, true);

        // 세이브 경로
        string mobilePath = Path.Combine(Application.persistentDataPath, "DSTPlayerData.json");
        string deskTopPath = Path.Combine(Application.dataPath, "DSTPlayerData.json");

        string path = mobilePath;

        // 테스트경로 : 데스크톱용
        if (ChangeSavePathForTest)
        {
            path = deskTopPath;
        }
        File.WriteAllText(path, jsonData);
    }


    /// <summary>
    /// 경로에서 Json 파일 읽기
    /// </summary>
    [ContextMenu("From Json Data")]
    public bool LoadPlayerDataFromJson()
    {
        // 세이브 경로
        string mobilePath = Path.Combine(Application.persistentDataPath, "DSTPlayerData.json");
        string deskTopPath = Path.Combine(Application.dataPath, "DSTPlayerData.json");

        string path = mobilePath;

        if (ChangeSavePathForTest)
        {
            path = deskTopPath;
        }

        if(DontLoadData == true)
        {
            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(path);
            LoadPD = JsonUtility.FromJson<PlayerData>(jsonData);

            return true;
        }

        catch (FileNotFoundException)
        {
            return false;
        }

    }

    #endregion

   

    #region Load Data Apply

    /// <summary>
    /// 불러온 데이터 적용
    /// </summary>
    public void LoadDataApply()
    {
        // 레벨 적용 및 최대치 세이브
        LevelManager.Instance.NowLevel = LoadPD.Level;
        SavePD.Level = LoadPD.Level;

        // 사운드 옵션값 적용
        SoundManager.Instance.BGMVolume = LoadPD.BGMVolume;
        SoundManager.Instance.SoundEffectVolume = LoadPD.SoundEffectVolume;
    }

    public void LoadDataOnlySoundApply()
    {
        // 사운드 옵션값 적용
        SoundManager.Instance.BGMVolume = LoadPD.BGMVolume;
        SoundManager.Instance.SoundEffectVolume = LoadPD.SoundEffectVolume;
    }

    #endregion

}
