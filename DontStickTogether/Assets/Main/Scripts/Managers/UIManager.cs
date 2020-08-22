﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(UIManager)) as UIManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "UIManager";
                    instance = container.AddComponent(typeof(UIManager)) as UIManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Objects

    [System.Serializable]
    public struct StructStartingObject
    {
        public GameObject TimerObject;
        public TextMeshProUGUI TimerText;
    }
    public StructStartingObject StartingObject;

    [System.Serializable]
    public struct StructTestObject
    {
        [Header("테스트용 오브젝트 수 Text")]
        public TextMeshProUGUI HumanCountText;

        [Header("테스트용 원 안의 사람 수 Text")]
        public TextMeshProUGUI HumanInCircleCountText;
    }
    public StructTestObject TestObject;


    public GameObject PlayerCanvas;
    public Image BioHazard;

    public GameObject GameClearFlagObject;
    public TextMeshProUGUI GameClearFlagText;

    public GameObject GameClearObject;
    public GameObject GameOverObject;

    #endregion


    #region Coroutines

    public IEnumerator GameClearCoroutine;

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


        GameOverObject.SetActive(false);
    }

    

    public void GameClear(bool OnOff)
    {
        if (OnOff == true)
        {
            // GameClear 등장 연출부
            //

            if(GameClearCoroutine != null)
            {
                StopCoroutine(GameClearCoroutine);
            }
            GameClearCoroutine = GameClearUpdate(2.0f);
            StartCoroutine(GameClearCoroutine);
        }
        else
        {
            // GameClear 소멸 연출부
            //

            if (GameClearCoroutine != null)
            {
                StopCoroutine(GameClearCoroutine);
            }
            GameClearObject.SetActive(false);
        }
    }

    IEnumerator GameClearUpdate(float duration)
    {
        float t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;

            yield return null;
        }

        GameClearObject.SetActive(false);
    }


    public void GameOver(bool OnOff)
    {
        if (OnOff == true)
        {
            //  등장 연출부

            SoundManager.Instance.StartGameOverBGM();

            PlayerCanvas.SetActive(false);
            GameOverObject.SetActive(true);
        }
        else
        {
            //  소멸 연출부
            //

            PlayerCanvas.SetActive(true);
            GameOverObject.SetActive(false);
        }
        
    }


    public void StartTimerSet(bool OnOff)
    {
        if (OnOff == true)
        {
            // Starting Timer 등장 연출부
            //

            StartingObject.TimerObject.SetActive(true);
        }
        else
        {
            // Starting Timer 소멸 연출부
            //

            StartingObject.TimerObject.SetActive(false);
        }
    }

    public void StartTimerUpdate(int value)
    {
        
        StartingObject.TimerText.text = value.ToString();
    }

    public void FlagSet(bool OnOff)
    {
        if (OnOff == true)
        {
            // Main Timer 등장 연출부
            //

            GameClearFlagObject.SetActive(true);
        }
        else
        {
            // Main Timer 소멸 연출부
            //

            GameClearFlagObject.SetActive(false);
        }
    }

    public void FlagUpdate(float leftTime)
    {        
        GameClearFlagText.text = string.Format("{0:F2}", leftTime);
    }

    public void UpdateHumanCount(int value)
    {
        TestObject.HumanCountText.text = string.Format("Obj : {0}", value);
    }

    public void UpdateHumanInCircleCount(int value)
    {
        TestObject.HumanInCircleCountText.text = string.Format("Obj In Circle : {0}", value);
    }

    public void UpdateBioHazard()
    {
        float max = GameManager.Instance.MaxBioHazard;
        float now = GameManager.Instance.NowBioHazard;

        BioHazard.fillAmount = (now / max);
    }
}
