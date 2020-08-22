using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityScript.Steps;

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

        GameInIt();
    }

    /// <summary>
    /// 치트키
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameClear();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            MaxBioHazard = 100000;
            NowBioHazard -= 10000;
        }
    }


    IEnumerator StartingTimer(int time)
    {
        float t = 0;

        /* GameOver Flag UI Set */
        UIManager.Instance.FlagSet(true);

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
        NowBioHazard = 0;

        Dictionary<string, object> levelData = LevelManager.Instance.GetLevelCSV();

        if (levelData == null)
        {
            Debug.LogWarning("모든 레벨이 클리어됨. 다음 레벨이 없음.");
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

    public void GameClean()
    {
        SoundManager.Instance.StopMainBGM();
        SoundManager.Instance.StopGameOverBGM();

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

        GameInIt();
    }

    public void GameOver()
    {
        GameClean();

        UIManager.Instance.GameOver(true);
    }
    #endregion


}
