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

    public Transform[] Buildings;

    #endregion

    #region Coroutines

    /* Single Spawn */
    IEnumerator coroutine;

    /* Multi Spawn */
    List<IEnumerator> coroutineList = new List<IEnumerator>();

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

    
    public void CoroutineListClear()
    {
        if(coroutineList.Count > 0)
        {
            for (int i = 0; i < coroutineList.Count; i++)
            {
                if (coroutineList[i] != null)
                {
                    StopCoroutine(coroutineList[i]);
                }
            }
        }

        coroutineList.Clear();
    }

    public void StartSpawn()
    {
        if (LevelManager.Instance.UseFixedLevel == false)
        {
            /* 레벨디자인 컨셉 1 : 매 레벨마다 각 유닛 생성 시간만 설정*/
            // 각 Set별 스폰을 멀티로 돌린다.
            ScriptableLevel sl = LevelManager.Instance.GetLevel();

            // 코루틴 Clean
            CoroutineListClear();
            for (int i = 0; i < sl.SetHuman.Length; i++)
            {
                IEnumerator c = MultipleSpawnCoroutine(sl.SetHuman[i]);
                coroutineList.Add(c);
                StartCoroutine(c);
            }
        }
        else
        {
            /* 레벨디자인 컨셉 2 : 고정된 초에 고정된 유닛들이 나온다.*/
            // Fixed스폰을 하나 돌린다.
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = FixedSpawnCoroutine();
            StartCoroutine(coroutine);
        }

    }

    public void StopSpawn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        CoroutineListClear();
    }


    /// <summary>
    /// 각 타입마다 생성기, 빌딩은 랜덤
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    IEnumerator MultipleSpawnCoroutine(ScriptableLevel.StructSetHuman set)
    {
        float t = 0;
        float SpawnTime = UnityEngine.Random.Range(set.MinTime, set.MaxTime);

        while (true)
        {
            if (t < SpawnTime)
            {
                t += Time.deltaTime;
            }
            else
            {
                t -= SpawnTime;

                SpawnTime = UnityEngine.Random.Range(set.MinTime, set.MaxTime);
                int BuildingIndex = UnityEngine.Random.Range(0, Buildings.Length-1);

                SetPool(set.Type, set.Speed, set.AttackPoint, BuildingIndex);
            }

            yield return null;
        }

    }

    /// <summary>
    /// 고정방식
    /// </summary>
    /// <returns></returns>
    IEnumerator FixedSpawnCoroutine()
    {
        LevelManager lm = LevelManager.Instance;

        float t = 0;
        int fixedTime = 0;

        // 0초 생성
        ScriptableLevel sl = lm.GetLevel();

        ScriptableLevel.StructSetFixedHuman[] datas = sl.FixedHuman[fixedTime].Data;

        if(datas.Length > 0)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                EnumHuman eh = datas[i].Type;
                int buildingIndex = datas[i].BuildingIndex;
                float speed = datas[i].MoveSpeed;
                float attackPoint = datas[i].AttackPoint;

                SetPool(eh, speed, attackPoint, buildingIndex);
            }
        }

        while (true)
        {
            // 1초마다 생성
            if(t >= 1.0f)
            {
                fixedTime += 1;
                t -= 1;

                sl = lm.GetLevel();

                // 설정된 레벨이 모드 끝난경우 : 코루틴 종료
                if(fixedTime >= sl.FixedHuman.Length)
                {
                    yield break;
                }

                // 1초 생성
                sl = lm.GetLevel();

                datas = sl.FixedHuman[fixedTime].Data;

                if (datas.Length > 0)
                {
                    for (int i = 0; i < datas.Length; i++)
                    {
                        EnumHuman eh = datas[i].Type;
                        int buildingIndex = datas[i].BuildingIndex;
                        float speed = datas[i].MoveSpeed;
                        float attackPoint = datas[i].AttackPoint;

                        SetPool(eh, speed, attackPoint, buildingIndex);
                    }
                }
            }
            else
            {
                t += Time.deltaTime;
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
            // sorting order
            go.GetComponent<SpriteRenderer>().sortingOrder = ObjectCount;

            List[key].Add(go);
            go.SetActive(false);

            ObjectCount += 1;
            UIManager.Instance.UpdateHumanCount(ObjectCount);
        }

        return true;
    }

    public void SetPool(EnumHuman eh, float speed, float attackPoint, int buildingIndex)
    {
        // enum to string (key)
        string key = Enum.GetName(typeof(EnumHuman), eh);

        SetPool(key, speed, attackPoint, buildingIndex);
    }

    public void SetPool(EnumHuman eh, float speed, int buildingIndex)
    {
        // enum to string (key)
        string key = Enum.GetName(typeof(EnumHuman), eh);

        SetPool(key, speed, buildingIndex);
    }

    public void SetPool(EnumHuman eh, int buildingIndex)
    {
        // enum to string (key)
        string key = Enum.GetName(typeof(EnumHuman), eh);

        SetPool(key, buildingIndex);
    }

    public void SetPool(string key, float speed, float attackPoint, int buildingIndex)
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

                    //h.InitHuman(GetCreationPoint(), speed);
                    h.InitHuman(Buildings[buildingIndex].position, speed, attackPoint);

                    return;
                }
            }

            if (IncreasePool(key, 1) == false)
            {
                return;
            }
        }
    }


    public void SetPool(string key, float speed, int buildingIndex)
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

                    //h.InitHuman(GetCreationPoint(), speed);
                    h.InitHuman(Buildings[buildingIndex].position, speed);

                    return;
                }
            }

            if(IncreasePool(key, 1) == false)
            {
                return;
            }
        }
    }

    public void SetPool(string key, int buildingIndex)
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

                    //h.InitHuman(GetCreationPoint());
                    h.InitHuman(Buildings[buildingIndex].position);

                    return;
                }
            }

            if (IncreasePool(key, 1) == false)
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
