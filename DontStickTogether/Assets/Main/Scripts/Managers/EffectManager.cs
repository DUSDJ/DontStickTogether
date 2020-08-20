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

    }


    #region Pooling
    public void IncreasePool(string key, int num)
    {
        Debug.Log("IncreasePool() : " + gameObject.name);

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
            Debug.LogWarning("없는 이름으로 Effect 생성 : " + key);
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
            Debug.LogWarning("없는 이름으로 Effect 생성 : " + msg);
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
            Debug.LogWarning("없는 이름으로 Effect 생성 : " + msg);
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

