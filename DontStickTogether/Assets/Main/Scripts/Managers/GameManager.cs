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

    #endregion

    #region GameLogics

    [Header("패배조건 시민 수")]
    [Range(0, 30)]public int HumanInCircleLimit;

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

            if (value >= HumanInCircleLimit)
            {
                // Game Over
                GameOver();
            }

        }
    }

    #endregion



    #region Coroutines

    IEnumerator StartingCoroutine;

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
        if(StartingCoroutine != null)
        {
            StopCoroutine(StartingCoroutine);
        }
        StartingCoroutine = StartingTimer(StartTime);
        StartCoroutine(StartingCoroutine);
    }
    
    IEnumerator StartingTimer(int time)
    {
        float t = 0;

        /* GameOver Flag UI Set */
        UIManager.Instance.FlagSet(true);
        UIManager.Instance.FlagUpdate();

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
    }


    public void AddHuman(Human h)
    {
        // ArriveState 변환
        h.ChangeState(h.HumanState.HumanStateArrive);

        // Count Add

        HumansInCircle += 1;

        // UI Update

        UIManager.Instance.FlagUpdate();
    }

    public void GameOver()
    {
        HumanManager.Instance.StopSpawn();
        HumanManager.Instance.CleanHuman();

        UIManager.Instance.GameOver(true);
    }
}
