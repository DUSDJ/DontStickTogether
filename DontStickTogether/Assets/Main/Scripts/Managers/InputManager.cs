using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "InputManager";
                    instance = container.AddComponent(typeof(InputManager)) as InputManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Values

    [Header("PC 테스트(마우스입력)")]
    public bool InputTest;

    [Header("손가락 크기?")]
    public float TouchRadius;

    #endregion

    #region Components

    public Camera MainCamera;

    #endregion

    #region Touch List



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

        MainCamera = Camera.main;
    }

    public Dictionary<int, GameObject> TouchDic = new Dictionary<int, GameObject>();

    private void Update()
    {
        if (InputTest == true)
        {
            return;
        }

        if(Time.timeScale == 0)
        {
            return;
        }

        int i = 0;
        while(i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);

            if(t.phase == TouchPhase.Began)
            {
                Vector2 pos = GetTouchPosition(t);

                // Effect : 마우스클릭 이펙트
                EffectManager.Instance.SetClickPool(pos);


                LayerMask targetLayer = 1 << LayerMask.NameToLayer("Touchable");
                Collider2D c = Physics2D.OverlapCircle(pos, TouchRadius, targetLayer);

                if(c != null)
                {
                    TouchDic.Add(t.fingerId, c.gameObject);
                    TouchDic[t.fingerId].SendMessageUpwards("TouchBegan", t);
                    //c.SendMessageUpwards("TouchBegan", t);              
                }
            }
            else if (t.phase == TouchPhase.Moved)
            {
                if (TouchDic.ContainsKey(t.fingerId) && TouchDic[t.fingerId] != null){
                    TouchDic[t.fingerId].SendMessageUpwards("TouchMove", t);
                }                

            }
            else if (t.phase == TouchPhase.Ended)
            {
                if (TouchDic.ContainsKey(t.fingerId))
                {
                    TouchDic[t.fingerId].SendMessageUpwards("TouchEnded", t);
                    TouchDic.Remove(t.fingerId);
                }
            }

            i += 1;
        }
    }


    public Vector2 GetTouchPosition(Touch t)
    {
        Vector2 touchPosition = t.position;
        return MainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0));
    }

}
