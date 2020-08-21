using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

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

    #region GameLogics

    private int humansInCircle;
    public int HumansInCircle
    {
        get
        {
            return humansInCircle;
        }
        set
        {
            humansInCircle = value;
            UIManager.Instance.UpdateHumanInCircleCount(value);

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
        GameInIt();
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

        // Count Add

        HumansInCircle += 1;

    }

    public void RemoveHuman(Human h)
    {
        // IdleState 변환
        h.ChangeState(h.HumanState.HumanStateIdle);

        // Count Add
        HumansInCircle -= 1;

    }

    #region GameState Management

    public void GameInIt()
    {
        UIManager.Instance.GameClear(false);

        NowBioHazard = 0;

        ScriptableLevel sl = LevelManager.Instance.GetLevel();

        if(sl == null)
        {
            Debug.LogWarning("모든 레벨이 클리어됨. 다음 레벨이 없음.");
            return;
        }

        ClearTime = sl.ClearTime;

        if(ClearTime <= 0)
        {
            ClearTime = 30.0f;
        }
        
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
        if(StartingCoroutine != null)
        {
            StopCoroutine(StartingCoroutine);
        }
        if(MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
        }

        HumanManager.Instance.StopSpawn();
        HumanManager.Instance.CleanHuman();
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
