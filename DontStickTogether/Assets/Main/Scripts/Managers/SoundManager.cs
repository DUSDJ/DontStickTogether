using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "SoundManager";
                    instance = container.AddComponent(typeof(SoundManager)) as SoundManager;
                }
            }

            return instance;
        }
    }

    #endregion

    #region Volume Control

    // Main BGM Volume
    [SerializeField][Range(0f,1f)]
    private float bgmVolume;
    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }

        set
        {
            if (value <= 0)
            {
                value = 0;
            }

            bgmVolume = value;

            MainAudio.volume = bgmVolume;
        }
    }

    // SoundEffect Volume
    [SerializeField][Range(0f, 1f)]
    private float soundEffectVolume;
    public float SoundEffectVolume
    {
        get
        {
            return soundEffectVolume;
        }

        set
        {
            if (value <= 0)
            {
                value = 0;
            }

            soundEffectVolume = value;
            //ApplySoundEffectVolume();
        }
    }

    #endregion

    #region Components

    private AudioSource MainAudio;

    #endregion


    #region Sound List & Object List

    public Dictionary<string, AudioClip> DataDic;
    public Dictionary<string, List<AudioSource>> List;
    //public List<AudioSource> SoundList;

    [Header("사운드 오브젝트 수 한계")]
    public int SoundObjectMaxCount;
    private int SoundObjectCount;

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

        #region Resource Load & List Set

        /* Sound Resource Load */
        DataDic = new Dictionary<string, AudioClip>();

        AudioClip[] datas = Resources.LoadAll<AudioClip>("SoundEffect/");
        for (int i = 0; i < datas.Length; i++)
        {
            string key = datas[i].name;
            DataDic.Add(key, datas[i]);
        }

        /* Set Lists of Objects */
        List = new Dictionary<string, List<AudioSource>>();

        foreach (var item in DataDic)
        {
            List<AudioSource> tempList = new List<AudioSource>();
            List.Add(item.Key, tempList);
        }

        #endregion

        /* Value Set */
        SoundObjectCount = 0;

        /* Component Set */
        MainAudio = GetComponent<AudioSource>();
    }
    

    public void StartMainBGM()
    {
        StopGameOverBGM();

        if (MainAudio.isPlaying == false)
        {
            MainAudio.clip = MainBGM;
            MainAudio.Play();
            MainAudio.loop = true;
        }
    }

    public void PauseMainBGM()
    {
        if (MainAudio.clip == MainBGM
            && MainAudio.isPlaying == true)
        {
            MainAudio.Pause();
        }
    }

    public void UnPauseMainBGM()
    {
        if (MainAudio.clip == MainBGM)
        {
            MainAudio.UnPause();
        }
    }

    public void StopMainBGM()
    {
        if (MainAudio.clip == MainBGM
            && MainAudio.isPlaying == true)
        {
            MainAudio.Stop();            
        }
    }

    public AudioClip MainBGM;
    public AudioClip GameOverBGM;

    public void StartGameOverBGM()
    {
        StopMainBGM();
        
        if (MainAudio.isPlaying == false)
        {
            MainAudio.clip = GameOverBGM;
            MainAudio.Play();
            MainAudio.loop = false;
        }
    }

    public void StopGameOverBGM()
    {
        if (MainAudio.clip == GameOverBGM
            && MainAudio.isPlaying == true)
        {
            MainAudio.Stop();
        }
    }

    #region Pooling
    public bool IncreasePool(string key, int num)
    {
        Debug.Log("IncreasePool() : " + gameObject.name);

        for (int i = 0; i < num; i++)
        {
            if (SoundObjectCount >= SoundObjectMaxCount)
            {
                return false;
            }

            GameObject go = new GameObject(key);
            go.transform.position = Vector3.zero;
            go.transform.SetParent(transform);
            AudioSource audio = go.AddComponent<AudioSource>();
            audio.clip = DataDic[key];
            audio.volume = soundEffectVolume;
            audio.loop = false;

            List[key].Add(audio);

            SoundObjectCount += 1;   
        }

        return true;
    }

    public void SetPool(string key)
    {
        if (DataDic.ContainsKey(key) == false)
        {
            Debug.LogWarning("없는 이름으로 SoundEffect 생성 : " + key);
            return;
        }

        while (true)
        {
            for (int i = 0; i < List[key].Count; i++)
            {
                AudioSource h = List[key][i];
                
                if (h.isPlaying == false)
                {
                    h.Play();

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




    #region Volume Control

    public void SetBgmVolume(float volume)
    {
        BgmVolume = volume;
    }

    public void SetSoundEffectVolume(float volume)
    {
        SoundEffectVolume = volume;
    }

    public void ApplySoundEffectVolume()
    {
        foreach (var item in List)
        {
            List<AudioSource> ChildList = item.Value;

            for (int i = 0; i < ChildList.Count; i++)
            {
                ChildList[i].volume = soundEffectVolume;
            }
        }
    }

    #endregion

}
