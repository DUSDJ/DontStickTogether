using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    private int ObjectCountForSortingOrder = 0;

    public Transform[] Buildings;

    #endregion

    #region Coroutines

    /* Single Spawn */
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
        Dictionary<string, object> table = LevelManager.Instance.GetLevelCSV();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = TableSpawnCoroutine(table);
        StartCoroutine(coroutine);

    }

    public void StopSpawn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

    }


    

    /// <summary>
    /// 컨셉3 : CSV 테이블 기반, 유닛별 Rate 생성기
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    IEnumerator TableSpawnCoroutine(Dictionary<string, object> table)
    {
        float t = 0;
        int SpawnPerSecond = int.Parse(table["SpawnPerSecond"].ToString());
        float SpawnTime = 1.0f;

        // 미리 Rate에 맞게 전부 리스트를 뽑느다.
        int NormalRate = int.Parse(table["NormalRate"].ToString());
        int CollectorRate = int.Parse(table["CollectorRate"].ToString());
        int InsaneRate = int.Parse(table["InsaneRate"].ToString());
        int RebelRate = int.Parse(table["RebelRate"].ToString());
        int BomberRate = int.Parse(table["BomberRate"].ToString());

        List<string> unitBox = new List<string>();

        int num = (int)(NormalRate * PoolMaxCount * 0.1);                
        for (int i = 0; i < num; i++)
        {
            unitBox.Add("Normal_" + table["Chapter"].ToString());
        }

        num = (int)(CollectorRate * PoolMaxCount * 0.1);
        for (int i = 0; i < num; i++)
        {
            unitBox.Add("Collector");
        }

        num = (int)(InsaneRate * PoolMaxCount * 0.1);
        for (int i = 0; i < num; i++)
        {
            unitBox.Add("Insane");
        }

        num = (int)(RebelRate * PoolMaxCount * 0.1);
        for (int i = 0; i < num; i++)
        {
            unitBox.Add("Rebel");
        }

        num = (int)(BomberRate * PoolMaxCount * 0.1);
        for (int i = 0; i < num; i++)
        {
            unitBox.Add("Bomber");
        }

        while (true)
        {
            if(t >= SpawnTime)
            {
                // N 마리 생성
                for (int i = 0; i < SpawnPerSecond; i++)
                {
                    if(unitBox.Count <= 0)
                    {
                        yield break;
                    }

                    int index = UnityEngine.Random.Range(0, unitBox.Count);                    
                    string key = unitBox[index];
                    unitBox.RemoveAt(index);

                    // 이동패턴
                    StringBuilder Pattern = new StringBuilder();
                    Pattern.Append("Normal");
                    if (key.StartsWith("Normal"))
                    {
                        Pattern.Clear();
                        Pattern.Append(table["Pattern"].ToString());
                    }

                    // 능력치, 위치 설정
                    float Speed = float.Parse(table["Speed"].ToString()); // 스피드                
                    float AttackPoint = float.Parse(table["AttackPoint"].ToString()); // 공격력

                    int BuildingIndex = GetBuildingIndex();
                    
                    // Spawn!
                    SetPool(key, Speed, AttackPoint, Pattern.ToString(), BuildingIndex);
                }

                t -= SpawnTime;
            }
            else
            {
                t += Time.deltaTime;
            }

            yield return null;
        }

    }

    public int GetBuildingIndex()
    {
        int rand = UnityEngine.Random.Range(0, Buildings.Length - 1);

        return rand;
    }

    public Vector3 GetCreationPoint()
    {
        var circle = UnityEngine.Random.insideUnitCircle.normalized* Radius;

        return new Vector3(circle.x, circle.y, 0);
    }


    #region Pooling
    public bool IncreasePool(string key, int num)
    {
        // Fan은 오브젝트 한계 영향 안 받음.
        if (key.StartsWith("Fan") == false)
        {
            // 활성화된 오브젝트가 PoolMaxCount 이상이면 늘리지 않는다.
            if (ObjectCount >= PoolMaxCount)
            {
                return false;
            }
        }

        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(DataDic[key]);
            
            List[key].Add(go);
            go.SetActive(false);
            
        }

        return true;
    }

    /// <summary>
    /// 최종 SetPool - CSV 기반
    /// </summary>
    /// <param name="key"></param>
    /// <param name="speed"></param>
    /// <param name="attackPoint"></param>
    /// <param name="pattern"></param>
    /// <param name="buildingIndex"></param>
    public void SetPool(string key, float speed, float attackPoint, string pattern, int buildingIndex)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogError("없는 이름으로 Human 생성 : " + key);
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

                    h.InitHuman(Buildings[buildingIndex].position, speed, attackPoint, pattern);

                    // sorting order
                    h.GetComponent<SpriteRenderer>().sortingOrder = ObjectCount;

                    // 활성화된 오브젝트만 수를 센다.
                    ObjectCount += 1;
                    ObjectCountForSortingOrder += 1;

                    return;
                }
            }

            if (IncreasePool(key, 1) == false)
            {
                return;
            }
        }
    }

    public Fan SetPoolFan(Collector collector)
    {
        Dictionary<string, object> table = LevelManager.Instance.GetLevelCSV();
        string key = "Fan";

        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogError("없는 이름으로 Human 생성 : " + key);
            return null;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                Fan h = List[key][i].GetComponent<Fan>();

                if (h.gameObject.activeSelf == false)
                {
                    h.gameObject.SetActive(true);
                    h.MyCollector = collector;
                    h.InitHuman(collector.GetSummonRadius(), collector.MoveSpeed, collector.AttackPoint, "Normal");

                    // sorting order
                    h.GetComponent<SpriteRenderer>().sortingOrder = ObjectCountForSortingOrder;
                    ObjectCountForSortingOrder += 1;

                    return (Fan)h;
                }
            }

            // false면 에러임
            if (IncreasePool(key, 1) == false)
            {
                return null;
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

        ObjectCount = 0;
    }
}
