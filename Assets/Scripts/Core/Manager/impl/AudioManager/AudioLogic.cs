using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLogic : MonoBehaviour
{
    public AudioSource audioSource;

    #region 设置属性重定向

    public void SettingOfAudio(float volume)
    {
        audioSource.volume = volume;
    }

    public void SettingOfAudio(bool loop)
    {
        audioSource.loop = loop;
    }

    public void SettingOfAudio(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public void SettingOfAudio(float volume, bool loop)
    {
        audioSource.volume = volume;
        audioSource.loop = loop;
    }

    public void SettingOfAudio(float volume, AudioClip clip)
    {
        audioSource.volume = volume;
        audioSource.clip = clip;
    }

    public void SettingOfAudio(bool loop, AudioClip clip)
    {
        audioSource.loop = loop;
        audioSource.clip = clip;
    }

    public void SettingOfAudio(float volume, bool loop, AudioClip clip)
    {
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.clip = clip;
    }

    #endregion

    public void PlayAudio()
    {
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}
