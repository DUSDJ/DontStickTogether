using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Text TimerText;
    }
    public StructStartingObject StartingObject;

 
    [System.Serializable]
    public struct StructHeaderLeft
    {
        public Text ChapterText;
        public Text StageText;
        public Text PlayerText;

        [Header("상태에 따른 색")]
        public Color Danger;
        public Color Alert;
        public Color Safe;
    }
    public StructHeaderLeft HeaderLeft;


    public GameObject PlayerCanvas;
    public Image BioHazard;
    public Image Vinnette;

    [System.Serializable]
    public struct StructMainTimer
    {

        public GameObject MainTimerObject;
        public Text MainTimerText;
    }
    public StructMainTimer MainTimer;
    

    public GameObject GameClearObject;
    public GameObject GameOverObject;

    #endregion

    #region Cheat

    public Image CheatInvincible;

    public void UpdateCheatInvincible(bool OnOff)
    {
        if(OnOff == true)
        {
            CheatInvincible.color = Color.red;
        }
        else
        {
            CheatInvincible.color = Color.white;
        }
        
    }

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
            PlayerCanvas.SetActive(false);
            GameClearObject.SetActive(true);
        }
        else
        {
            PlayerCanvas.SetActive(true);
            GameClearObject.SetActive(false);
        }
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
        
        StartingObject.TimerText.text = string.Format("0{0}:00",value);
    }

    public void MainTimerSet(bool OnOff)
    {
        if (OnOff == true)
        {
            // Main Timer 등장 연출부            
            MainTimer.MainTimerObject.SetActive(true);
        }
        else
        {
            // Main Timer 소멸 연출부
            //

            MainTimer.MainTimerObject.SetActive(false);
        }
    }

    public void FlagUpdate(float leftTime)
    {

        MainTimer.MainTimerText.text = string.Format("{0:0.00}", leftTime).Replace(".", ":");
    }

    public void UpdateBioHazard()
    {
        float max = GameManager.Instance.MaxBioHazard;
        float now = GameManager.Instance.NowBioHazard;

        float value = (now / max);
        BioHazard.fillAmount = value;

        Color c = Vinnette.color;
        c.a = value;
        Vinnette.color = c;

        // Update Player State
        UpdatePlayerState(value);
    }


    #region Header Left

    public void UpdatePlayerState(float value)
    {
        if (value > 0.7f)
        {
            HeaderLeft.PlayerText.color = HeaderLeft.Danger;
            HeaderLeft.PlayerText.text = "위험";
        }
        else if (value > 0.3f)
        {
            HeaderLeft.PlayerText.color = HeaderLeft.Alert;
            HeaderLeft.PlayerText.text = "경계";
        }
        else
        {
            HeaderLeft.PlayerText.color = HeaderLeft.Safe;
            HeaderLeft.PlayerText.text = "안전";
        }
    }

    public void UpdateChapter(int value)
    {
        HeaderLeft.ChapterText.text = string.Format("{0}", value);
    }

    public void UpdateStage(int value)
    {
        HeaderLeft.StageText.text = string.Format("{0}", value);
    }

    #endregion

    [System.Serializable]
    public struct StructHeaderRight
    {
        public GameObject OptionObject;
        public Slider BGMSoundSlider;
        public Text BGMSoundText;
        public Slider SoundEffectSlider;
        public Text SoundEffectText;
        public GameObject PauseObject;
    }
    public StructHeaderRight HeaderRight;

    #region Header Right

    #region Option

    public void BtnOption()
    {
        GameManager.Instance.GameStop(true);
        HeaderRight.OptionObject.SetActive(true);

        HeaderRight.BGMSoundSlider.value = SoundManager.Instance.BGMVolume;
        HeaderRight.SoundEffectSlider.value = SoundManager.Instance.SoundEffectVolume;
    }

    public void BtnOptionClose()
    {
        GameManager.Instance.GameStop(false);

        PlayerDataManager.Instance.UpdateVolumeData();

        HeaderRight.OptionObject.SetActive(false);
    }

    public void SliderBGMSound()
    {
        SoundManager.Instance.BGMVolume = HeaderRight.BGMSoundSlider.value;
        HeaderRight.BGMSoundText.text = string.Format("{0}", Math.Truncate(HeaderRight.BGMSoundSlider.value * 100));
    }

    public void SliderSoundEffect()
    {
        SoundManager.Instance.SoundEffectVolume = HeaderRight.SoundEffectSlider.value;
        HeaderRight.SoundEffectText.text = string.Format("{0}", Math.Truncate(HeaderRight.SoundEffectSlider.value * 100));        
    }

    #endregion

    #region Pause

    public void BtnPause()
    {
        GameManager.Instance.GameStop(true);
        HeaderRight.PauseObject.SetActive(true);
    }

    public void BtnPauseClose()
    {
        GameManager.Instance.GameStop(false);
        HeaderRight.PauseObject.SetActive(false);
    }

    public void BtnReplay()
    {
        GameManager.Instance.GameRestart();
        
        BtnPauseClose();
    }

    public void BtnTitle()
    {
        BtnPauseClose();

        SceneManager.LoadScene(0);
    }

    #endregion

    #endregion
}
