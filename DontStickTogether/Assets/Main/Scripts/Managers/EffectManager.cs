using System;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(EffectManager)) as EffectManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "EffectManager";
                    instance = container.AddComponent(typeof(EffectManager)) as EffectManager;
                }
            }

            return instance;
        }
    }

    #endregion


    public Dictionary<string, List<GameObject>> List;
    public Dictionary<string, GameObject> DataDic;

    [HideInInspector]public GameObject ClickEffect;
    [HideInInspector]public List<GameObject> ClickEffectList;

    [HideInInspector]public GameObject TrailEffect;
    [HideInInspector]public List<GameObject> TrailEffectList;

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


        DataDic = new Dictionary<string, GameObject>();

        GameObject[] datas = Resources.LoadAll<GameObject>("Effect/");
        for (int i = 0; i < datas.Length; i++)
        {
            string key = datas[i].name;
            DataDic.Add(key, datas[i]);
        }

        List = new Dictionary<string, List<GameObject>>();
        foreach (var item in DataDic)
        {
            List<GameObject> tempList = new List<GameObject>();
            GameObject go = Instantiate<GameObject>(item.Value);
            go.transform.SetParent(transform, false);
            go.SetActive(false);

            tempList.Add(go);
            List.Add(item.Key, tempList);
        }

        // 클릭이펙트는 특별취급
        ClickEffectList = new List<GameObject>();
        ClickEffect = Resources.Load<GameObject>("UI/MouseTouch_Custom");


        // 트레일 이펙트도 특별취급
        TrailEffectList = new List<GameObject>();
        TrailEffect = Resources.Load<GameObject>("UI/Trail_Custom");
    }


    #region TrailEffect Pooling

    public void IncreaseTrailPool(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(TrailEffect);
            TrailEffectList.Add(go);
            go.SetActive(false);
        }
    }

    public Transform SetTrailPool()
    {
        while (true)
        {
            for (int i = 0; i < TrailEffectList.Count; i++)
            {
                GameObject g = TrailEffectList[i];

                if (g.gameObject.activeSelf == false)
                {
                    g.gameObject.SetActive(true);

                    return g.transform;
                }

            }

            IncreaseTrailPool(1);
        }
    }

    #endregion

    #region ClickEffect Pooling

    public void IncreaseClickPool(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(ClickEffect);
            go.transform.SetParent(UIManager.Instance.PlayerCanvas.transform, false);
            ClickEffectList.Add(go);
            go.SetActive(false);
        }
    }

    public void SetClickPool(Vector3 position)
    {
        while (true)
        {
            for (int i = 0; i < ClickEffectList.Count; i++)
            {
                GameObject g = ClickEffectList[i];

                if (g.gameObject.activeSelf == false)
                {
                    g.gameObject.SetActive(true);
                    g.transform.position = new Vector2(position.x, position.y);
                    g.GetComponent<Animation>().Play();

                    return;
                }

            }

            IncreaseClickPool(1);
        }
    }

    #endregion


    #region Pooling
    public void IncreasePool(string key, int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(DataDic[key]);
            go.transform.SetParent(transform, false);
            List[key].Add(go);
            go.SetActive(false);
        }
    }


    public void SetPool(string key, Vector3 position)
    {
        if (DataDic.ContainsKey(key) == false)
        {
           return;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Effect e = List[key][i].GetComponent<Effect>();

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);
                    e.SetEffect(position);

                    return;
                }

            }

            IncreasePool(key, 2);
        }
    }


    /// <summary>
    /// SetPool With Duration Setting
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="position"></param>
    public void SetPool(string msg, Vector3 position, float duration)
    {
        if (DataDic.ContainsKey(msg) == false)
        {
            return;
        }

        while (true)
        {
            for (int i = 0; i < List[msg].Count; i++)
            {
                Effect e = List[msg][i].GetComponent<Effect>();

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);
                    e.SetEffect(position, duration);

                    return;
                }

            }

            IncreasePool(msg, 2);
        }
    }

    /// <summary>
    /// SetPool, No SetEffect & Return Effect
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="position"></param>
    public Effect SetPool(string msg)
    {
        if (DataDic.ContainsKey(msg) == false)
        {
            //Debug.LogWarning("없는 이름으로 Effect 생성 : " + msg);
            return null;
        }

        while (true)
        {
            for (int i = 0; i < List[msg].Count; i++)
            {
                Effect e = List[msg][i].GetComponent<Effect>();

                if (e.gameObject.activeSelf == false)
                {
                    e.gameObject.SetActive(true);

                    return e;
                }

            }

            IncreasePool(msg, 2);
        }
    }



    #endregion


    public void AllClean()
    {
        foreach (var item in List)
        {
            foreach (var e in item.Value)
            {
                e.GetComponent<Effect>().Clean();
            }
        }
    }

}

