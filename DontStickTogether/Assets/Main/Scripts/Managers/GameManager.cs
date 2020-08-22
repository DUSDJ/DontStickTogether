﻿using System.Collections;
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
        UIManager.Instance.GameClear(false);

        if (PlayerDataManager.Instance.WillYouLoadData == true)
        {
            PlayerDataManager.Instance.LoadDataApply();            
        }
        else
        {
            PlayerDataManager.Instance.LoadDataOnlySoundApply();
            LevelManager.Instance.NowLevel = 1;
        }

        // 초기값 혹은 로드값 저장
        PlayerDataManager.Instance.UpdateSaveData();
        PlayerDataManager.Instance.UpdateVolumeData();

        GameInIt();
    }

    IEnumerator StartingTimer(int time)
    {
        float t = 0;

        /* Timer UI Set*/
        UIManager.Instance.StartTimerSet(true);
        UIManager.Instance.StartTimerUpdate(time);

        while (t < time)
        {
            t += Time.deltaTime;

            if(t >= 1)
            {
                t -= 1;
                time -= 1;

                UIManager.Instance.StartTimerUpdate(Mathf.FloorToInt(time));
            }

            yield return null;
        }

        UIManager.Instance.StartTimerUpdate(Mathf.FloorToInt(0));
        UIManager.Instance.StartTimerSet(false);

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
        
        UIManager.Instance.GameClear(false);

        NowBioHazard = 0;

        Dictionary<string, object> levelData = LevelManager.Instance.GetLevelCSV();

        if (levelData == null)
        {
            // 엔딩부분, 일단 타이틀로 돌아감
            Debug.LogWarning("모든 레벨이 클리어됨. 다음 레벨이 없음.");
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
        StartingCoroutine = StartingTimer(StartTime);
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

        // 저장
        PlayerDataManager.Instance.UpdateSaveData();
    }


    public void GameOver()
    {
        GameClean();

        UIManager.Instance.GameOver(true);
    }

    
    #endregion


}
