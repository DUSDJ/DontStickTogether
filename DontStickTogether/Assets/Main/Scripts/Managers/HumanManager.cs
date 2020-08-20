using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static HumanManager instance;
    public static HumanManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(HumanManager)) as HumanManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "HumanManager";
                    instance = container.AddComponent(typeof(HumanManager)) as HumanManager;
                }
            }

            return instance;
        }
    }

    #endregion


    #region Pool Lists

    public Dictionary<string, List<GameObject>> List;
    public Dictionary<string, GameObject> DataDic;


    #endregion


    #region Values

    [Header("원 크기")]
    public float Radius = 3.0f;

    [Header("사람 생성 시간 범위")]
    [Range(0f, 10f)]  public float MinTime;
    [Range(0f, 10f)]  public float MaxTime;

    [Header("오브젝트 최대 개수")]
    public int PoolMaxCount;

    private int ObjectCount = 0;

    #endregion

    #region Coroutines

    IEnumerator coroutine;

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


        /* Load Datas */
        DataDic = new Dictionary<string, GameObject>();

        GameObject[] datas = Resources.LoadAll<GameObject>("Human/");
        for (int i = 0; i < datas.Length; i++)
        {
            string key = datas[i].name;
            DataDic.Add(key, datas[i]);
        }

        /* Set Lists of Prefabs */
        List = new Dictionary<string, List<GameObject>>();

        foreach (var item in DataDic)
        {
            List<GameObject> tempList = new List<GameObject>();
            List.Add(item.Key, tempList);
        }
   
    }


    public void StartSpawn()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = SpawnCoroutine();
        StartCoroutine(coroutine);
    }

    public void StopSpawn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator SpawnCoroutine()
    {
        float t = 0;
        float SpawnTime = UnityEngine.Random.Range(MinTime, MaxTime);

        while (true)
        {
            if(t < SpawnTime)
            {
                t += Time.deltaTime;
            }
            else
            {
                t -= SpawnTime;

                SpawnTime = UnityEngine.Random.Range(MinTime, MaxTime);

                // 레벨매니저가 없어서 일단은 아무거나 생성
                SetPool("Human");
            }

            yield return null;
        }

    }

    
    public Vector3 GetCreationPoint()
    {
        var circle = UnityEngine.Random.insideUnitCircle.normalized* Radius;

        return new Vector3(circle.x, circle.y, 0);
    }


    #region Pooling
    public bool IncreasePool(string key, int num)
    {
        // 생성된 오브젝트가 PoolMaxCount 이상이면 늘리지 않는다.
        if (ObjectCount >= PoolMaxCount)
        {
            return false;
        }

        Debug.Log("IncreasePool() : " + gameObject.name);

        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(DataDic[key]);
            List[key].Add(go);
            go.SetActive(false);

            ObjectCount += 1;
            UIManager.Instance.UpdateHumanCount(ObjectCount);
        }

        return true;
    }

    public void SetPool(string key)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogWarning("없는 이름으로 Human 생성 : " + key);
            return;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Human h = List[key][i].GetComponent<Human>();

                if (h.gameObject.activeSelf == false)
                {
                    h.gameObject.SetActive(true);

                    h.InitHuman(GetCreationPoint());

                    return;
                }
            }

            if(IncreasePool(key, 1) == false)
            {
                return;
            }
        }
    }

    #endregion


    public void CleanHuman()
    {
        foreach (var item in List)
        {
            List<GameObject> childList = item.Value;
            for (int i = 0; i < childList.Count; i++)
            {
                if (childList[i].activeSelf == true)
                {
                    childList[i].SetActive(false);
                }
            }
        }
    }
}
