using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioList
{
    public AudioType Type;
    public List<AudioClip> AudioClips = new List<AudioClip>();

    public int Count => AudioClips.Count;

    public AudioList(AudioType type)
    {
        Type = type;
    }

    /// <summary>
    /// 添加音效进入列表
    /// </summary>
    /// <param name="clip"></param>
    public void AddAudioClip(AudioClip clip)
    {
        AudioClips.Add(clip);
    }

    /// <summary>
    /// 获取随机音效
    /// </summary>
    /// <returns></returns>
    public AudioClip GetRandomClip()
    {
        if (AudioClips.Count == 0)
        {
            return null;
        }
        return AudioClips[Random.Range(0, AudioClips.Count)];
    }

    /// <summary>
    /// 获取指定索引的音效   
    /// </summary>
    /// <param name="index">对应列表索引</param>
    /// <returns></returns>
    public AudioClip GetClip(int index)
    {
        if (index < 0 || index >= AudioClips.Count)
        {
            return null;
        }
        return AudioClips[index];
    }

    /// <summary>
    /// 清空音效列表
    /// </summary>
    public void ClearClip()
    {
        AudioClips.Clear();
    }

}
