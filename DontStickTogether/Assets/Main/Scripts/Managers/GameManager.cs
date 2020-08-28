using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region SingleTon
    /* SingleTon */
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "GameManager";
                    instance = container.AddComponent(typeof(GameManager)) as GameManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Values

    [Header("Stage3만진행")]
    public bool StartStage3 = false;

    [Header("시작 대기 시간")]
    public int StartTime;

    private float clearTime = 10f;
    public float ClearTime
    {
        get
        {
            return clearTime;
        }
        set
        {
            if(value <= 0)
            {
                value = 0;
                clearTime = value;
                UIManager.Instance.FlagUpdate(clearTime);

                GameClear();
            }
            else
            {
                clearTime = value;
                UIManager.Instance.FlagUpdate(clearTime);
            }
        }
    }

    [Header("오염 최대치")]
    public float MaxBioHazard = 100f;

    private float nowBioHazrad = 0f;
    public float NowBioHazard
    {
        get
        {
            return nowBioHazrad;
        }
        set
        {
            if(value >= MaxBioHazard)
            {
                value = MaxBioHazard;
                nowBioHazrad = value;
                // UI Update
                UIManager.Instance.UpdateBioHazard();
                // Game Over
                GameOver();
            }

            nowBioHazrad = value;
            // UI Update
            UIManager.Instance.UpdateBioHazard();

            
        }
    }

    private bool ClearAllStage = false;

    #endregion



    #region Coroutines

    IEnumerator StartingCoroutine;
    IEnumerator MainCoroutine;

    #endregion


    #region CheatKey

    bool cheatInvincible = false;

    public void CheatClear()
    {
        GameClear();
    }

    public void CheatInvincible()
    {
        if(cheatInvincible == false)
        {
            MaxBioHazard = 1000000;
            NowBioHazard -= 10000;
            cheatInvincible = true;
            UIManager.Instance.UpdateCheatInvincible(true);
        }
        else
        {
            MaxBioHazard = 100;
            NowBioHazard = 0;
            cheatInvincible = false;
            UIManager.Instance.UpdateCheatInvincible(false);
        }
    }

    #endregion

    public Transform Center;


    private void Awake()
    {
        #region SingleTone

        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }

        #endregion
    }

    
    private void Start()
    {
        ClearAllStage = false;

        UIManager.Instance.GameClear(false);

        PlayerDataManager.Instance.LoadDataApply();

        if (PlayerDataManager.Instance.WillYouLoadData == false)
        {
            LevelManager.Instance.NowLevel = 1;
        }

        if(StartStage3 == true && LevelManager.Instance.NowLevel < 11)
        {
            LevelManager.Instance.NowLevel = 11;
        }

        // 초기값 혹은 로드값 저장
        PlayerDataManager.Instance.UpdateSaveData();
        PlayerDataManager.Instance.UpdateVolumeData();

        GameInIt();
    }

    public void GameSetting()
    {
        /* GameOver Main Timer UI Set */
        UIManager.Instance.MainTimerSet(true);

        HumanManager.Instance.StartSpawn();
        SoundManager.Instance.StartMainBGM();


        if (MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
        }
        MainCoroutine = MainTimerCoroutine();
        StartCoroutine(MainCoroutine);
    }

    IEnumerator StartingTimer()
    {
        /* Timer UI Set*/
        UIManager.Instance.StartTimerSet(true);

        // 애니메이션 콜백 받아서 GameSetting()

        yield return null;        
    }

    IEnumerator MainTimerCoroutine()
    {
        while (true)
        {
            ClearTime -= Time.deltaTime;

            yield return null;
        }
    }

    public void AddHuman(Human h)
    {
        // ArriveState 변환
        h.ChangeState(h.HumanState.HumanStateArrive);

    }

    public void RemoveHuman(Human h)
    {
        // IdleState 변환
        h.ChangeState(h.HumanState.HumanStateIdle);

    }

    #region GameState Management

   

    public void GameInIt()
    {
        if(ClearAllStage == true)
        {
            SceneManager.LoadScene(0);
            return;
        }

        UIManager.Instance.GameClear(false);

        NowBioHazard = 0;

        Dictionary<string, object> levelData = LevelManager.Instance.GetLevelCSV();

        if (levelData == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        // Set Clear Time
        ClearTime = float.Parse(levelData["ClearTime"].ToString());
        if (ClearTime <= 0)
        {
            ClearTime = 30.0f;
        }

        // Set Level Environments
        LevelManager.Instance.SetLevel();


        StartLevel();
    }


    public void StartLevel()
    {

        if (StartingCoroutine != null)
        {
            StopCoroutine(StartingCoroutine);
        }
        StartingCoroutine = StartingTimer();
        StartCoroutine(StartingCoroutine);
    }

    public void GameStop(bool OnOff)
    {
        if (OnOff == true)
        {
            Time.timeScale = 0f;
            SoundManager.Instance.PauseMainBGM();
        }
        else
        {
            Time.timeScale = 1.0f;
            SoundManager.Instance.UnPauseMainBGM();
        }
        
    }

    public void GameClean()
    {
        InputManager.Instance.TouchDic.Clear();

        SoundManager.Instance.StopMainBGM();

        if (StartingCoroutine != null)
        {
            StopCoroutine(StartingCoroutine);
        }
        if(MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
        }

        UIManager.Instance.GameOver(false);

        HumanManager.Instance.StopSpawn();
        HumanManager.Instance.CleanHuman();
    }

    public void GameRestart()
    {
        GameClean();

        GameInIt();

    }

    public void GameClear()
    {
        GameClean();

        // 클리어 연출부
        UIManager.Instance.GameClear(true);
        LevelManager.Instance.NowLevel += 1;

        // 저장 및 엔딩 검사        
        if (LevelManager.Instance.NowLevel > 15)
        {
            ClearAllStage = true;
            PlayerDataManager.Instance.ResetData();
        }
        else
        {
            PlayerDataManager.Instance.UpdateSaveData();
        }
    }


    public void GameOver()
    {
        GameClean();

        UIManager.Instance.GameOver(true);
    }

    
    #endregion


}
