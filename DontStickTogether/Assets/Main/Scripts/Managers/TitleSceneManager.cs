using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject PopupRanking;
    public GameObject PopupCredits;


    #region SingleTon
    /* SingleTon */
    private static TitleSceneManager instance;
    public static TitleSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(TitleSceneManager)) as TitleSceneManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "TitleSceneManager";
                    instance = container.AddComponent(typeof(TitleSceneManager)) as TitleSceneManager;
                }
            }

            return instance;
        }
    }

    #endregion

    public void BtnStart()
    {
        // PlayerDataManager 사용여부
        PlayerDataManager.Instance.WillYouLoadData = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BtnContinue()
    {
        // PlayerDataManager 사용여부
        PlayerDataManager.Instance.WillYouLoadData = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BtnRanking(bool OnOff)
    {
        if(OnOff == true)
        {
            PopupRanking.SetActive(true);
            PopupCredits.SetActive(false);
        }
        else
        {
            PopupRanking.SetActive(false);
        }
        

    }

    public void BtnCredit(bool OnOff)
    {
        if (OnOff == true)
        {
            PopupRanking.SetActive(false);
            PopupCredits.SetActive(true);
        }
        else
        {
            PopupCredits.SetActive(false);
        }
        
    }
}
