using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AudioType
{
    BGM,//此处是示例，具体根据项目需求来定义
    Effect,
}

/* BGM0 关卡背景音乐
 * BGM1 关卡音乐1
 * BGM2 开始界面
 * BGM3 游戏失败
 * Effect0 按钮音效
 * Effect1 无人机音效
 * Effect2 气泡附着
 * Effect3 气泡生成
 * Effect4 泡泡破裂
 * Effect5 穿过染色云
 * Effect6 纸箱受损
 * Effect7 蜂刺
 */

/// <summary>
/// 音效管理器
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region 单例部分（可删除用自动挂载类替代）

    //用于(线程安全)加锁得到对象
    protected static readonly object lockObj = new object();

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("尝试在编辑器模式下访问单例实例。");
                    return null;
                }

                //动态创建 动态挂载
                //在场景上创建一个空物体，然后挂载脚本
                GameObject obj = new GameObject();
                //通过得到T脚本的类名 在改名 可以在编辑器中明确的看到单例模式的脚本
                obj.name = typeof(AudioManager).ToString();
                //动态挂载 单例模式脚本
                instance = obj.AddComponent<AudioManager>();
                //过场景时不销毁 保证单例模式
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    #endregion

    #region 管理器属性

    //音效列表
    public Dictionary<AudioType, AudioList> audioClipDict = new Dictionary<AudioType, AudioList>();
    //音效对象携带的子对象
    public Dictionary<GameObject, GameObject> ObjsOfAudioSource = new Dictionary<GameObject, GameObject>();
    //音效对象携带的子对象上的音效组件对应的原音量
    public Dictionary<GameObject, Dictionary<AudioSource , float>> VolumeOfObjs = new Dictionary<GameObject, Dictionary<AudioSource, float>>();

    private string nameAdd = "_AudioSource";//音效对象的名字

    private string path = "Audio/";
    public bool isLoaded = false;

    private int TypeCount = 2;


    private float allVolume = 1;//总音量
    private float bgmVolume = 1;//背景音乐音量
    private float effectVolume = 1;//音效音量

    #region Getters & Setters

    public float AllVolume
    {
        get => allVolume;
        set => allVolume = value;
    }

    public float BgmVolume
    {
        get => bgmVolume;
        set => bgmVolume = value;
    }

    public float EffectVolume
    {
        get => effectVolume;
        set => effectVolume = value;
    }

    private float BgmVolumeMuti => allVolume * bgmVolume;

    private float EffectVolumeMuti => allVolume * effectVolume;

    public delegate void AddAfterLoad();
    public AddAfterLoad addAfterLoad;

    #endregion

    #endregion

    #region 组件引用

    private GameObject MainCamera;//背景音乐添加到主相机上

    #endregion

    #region 生命周期

    private void Awake()
    {
        //初始化音效列表
        InitAudioList();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (isLoaded)
        {
            addAfterLoad?.Invoke();
            addAfterLoad = null;
        }
    }

    #endregion

    #region 音量调节

    public void AudjestVolume(float volume)
    {
        allVolume = volume;
        foreach (KeyValuePair<GameObject, GameObject> pair in this.ObjsOfAudioSource)
        {
            if (!VolumeOfObjs.ContainsKey(pair.Value)) continue;
            try
            {
                if (!pair.Value.gameObject.activeSelf) continue;
            }
            catch
            {
                continue;
            }
            AudioSource[] AS = pair.Value.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource source in AS)
            {
                if (VolumeOfObjs[pair.Value].ContainsKey(source))
                {
                    source.volume = VolumeOfObjs[pair.Value][source] * allVolume;
                }
            }
        }
    }

    #endregion

    #region 音效管理部分
    /// <summary>
    /// 为对象添加携带音效组件的子对象
    /// </summary>
    /// <param name="fatherobj"></param>
    /// <param name="audioType"></param>
    /// <param name="index"></param>
    public AudioSource AddChildWithAudioSource(GameObject fatherObj, AudioType audioType, int index, float OrginVolume = 1)
    {
        GameObject sonObj = GameObject.Find(fatherObj.name + nameAdd);
        if (sonObj == null)
        {
            sonObj = new GameObject(fatherObj.name + nameAdd);
            sonObj.transform.SetParent(fatherObj.transform);
            ObjsOfAudioSource.Add(fatherObj, sonObj);
            if (!VolumeOfObjs.ContainsKey(sonObj))
            {
                VolumeOfObjs.Add(sonObj, new Dictionary<AudioSource, float>());
            }
        }

        AudioSource[] audioClips = fatherObj.GetComponentsInChildren<AudioSource>();
        foreach (var audioClip in audioClips)
        {
            if (audioClip.clip == audioClipDict[audioType].GetClip(index))
            {
                return audioClip;
            }
        }

        AudioSource audioSource = sonObj.AddComponent<AudioSource>();
        audioSource.volume = OrginVolume;
        VolumeOfObjs[sonObj].Add(audioSource, OrginVolume);
        audioSource.clip = audioClipDict[audioType].GetClip(index);

        return audioSource;
    }

    #endregion

    #region 音效加载部分

    /// <summary>
    /// 加载音效
    /// </summary>
    public void InitAudioList()
    {
        StartCoroutine(LoadAudioClip());
    }

    /// <summary>
    /// 音效加载协程
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerator LoadAudioClip()
    {
        for (int i = 0; i < TypeCount; i++)
        {
            AudioType type = (AudioType)i;
            audioClipDict.Add(type, new AudioList(type));
            path = "Audio/" + type.ToString() + "/" + type.ToString();
            int index = 0;
            while (true) //循环加载音效
            {
                AudioClip clip = Resources.Load<AudioClip>(path + index);
                if (clip == null)
                {
                    break;
                }
                Debug.Log("加载音效：" + clip.name);
                audioClipDict[type].AddAudioClip(clip);
                index++;
                yield return clip;
            }
        }
        isLoaded = true;
    }

    #endregion
}
