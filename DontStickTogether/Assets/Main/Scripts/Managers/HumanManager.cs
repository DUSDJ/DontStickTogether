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

        Debug.Log("SpawnPerSecond : " + SpawnTime);

        // Rate 0 ~ 10
        
        int NormalRate = int.Parse(table["NormalRate"].ToString());
        int CollectorRate = int.Parse(table["CollectorRate"].ToString());
        int InsaneRate = int.Parse(table["InsaneRate"].ToString());
        int RebelRate = int.Parse(table["RebelRate"].ToString());
        int BomberRate = int.Parse(table["BomberRate"].ToString());
        

        /*
        int adder = 0;
        Vector2Int NormalRate = new Vector2Int(adder, (int.Parse(table["NormalRate"].ToString()) + adder));
        adder += NormalRate.y;

        Vector2Int CollectorRate = new Vector2Int(adder, (int.Parse(table["CollectorRate"].ToString()) + adder));
        adder += CollectorRate.y;

        Vector2Int InsaneRate = new Vector2Int(adder, (int.Parse(table["InsaneRate"].ToString()) + adder));
        adder += InsaneRate.y;

        Vector2Int RebelRate = new Vector2Int(adder, (int.Parse(table["RebelRate"].ToString()) + adder));
        adder += RebelRate.y;

        Vector2Int BomberRate = new Vector2Int(adder, (int.Parse(table["BomberRate"].ToString()) + adder));
        */

        while (true)
        {
            if(t >= SpawnTime)
            {
                // N 마리 생성
                for (int i = 0; i < SpawnPerSecond; i++)
                {
                    int dice = UnityEngine.Random.Range(0, 10); // 0 ~ 9                    
                    string key = "Bug";

                    // 이동패턴
                    StringBuilder Pattern = new StringBuilder();
                    Pattern.Append("Normal");

                    if(dice < NormalRate && NormalRate != 0)
                    {
                        // 노말!
                        key = "Normal_" + table["Chapter"].ToString();
                        Pattern.Clear();
                        Pattern.Append(table["Pattern"].ToString());
                    }
                    else if (dice < CollectorRate + NormalRate && CollectorRate != 0)
                    {
                        key = "Collector";
                    }
                    else if (dice < InsaneRate + CollectorRate + NormalRate && InsaneRate != 0)
                    {
                        key = "Insane";
                    }
                    else if (dice < RebelRate + InsaneRate + CollectorRate + NormalRate && RebelRate != 0)
                    {
                        key = "Rebel";
                    }
                    else if (BomberRate != 0)
                    {
                        key = "Bomber";
                    }

                    Debug.Log("Dice : " + dice);
                    Debug.Log(key);

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
        int loopCount = 0;

        while (loopCount < 10)
        {
            int rand = UnityEngine.Random.Range(0, Buildings.Length - 1);

            if (LevelManager.Instance.Buildings[rand].enabled == false)
            {
                loopCount += 1;
                continue;
            }

            return rand;
        }

        return 2;
    }

    public Vector3 GetCreationPoint()
    {
        var circle = UnityEngine.Random.insideUnitCircle.normalized* Radius;

        return new Vector3(circle.x, circle.y, 0);
    }


    #region Pooling
    public bool IncreasePool(string key, int num)
    {
        // 활성화된 오브젝트가 PoolMaxCount 이상이면 늘리지 않는다.
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
                    UIManager.Instance.UpdateHumanCount(ObjectCount);

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

        ObjectCount = 0;
        UIManager.Instance.UpdateHumanCount(ObjectCount);
    }
}
