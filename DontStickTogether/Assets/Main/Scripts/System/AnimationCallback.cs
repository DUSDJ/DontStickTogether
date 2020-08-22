using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour
{
    public void AnimEnd()
    {
        gameObject.SetActive(false);
    }

    public void Send(string msg)
    {
        SendMessage(msg);
    }

    public void AnimEndGameStart()
    {
        GameManager.Instance.GameSetting();
        gameObject.SetActive(false);
    }
}
