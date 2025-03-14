using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BKAudioPlayer : MonoBehaviour
{
    int[] index = new int[7] { 2, 2, 0, 0, 4, 4, 2 };


    private void Start()
    {
        AudioSource asForBegin;
        AudioManager.Instance.addAfterLoad += () =>
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            int nowSceneIndex = SceneManager.GetActiveScene().buildIndex;
            asForBegin = AudioManager.Instance.AddChildWithAudioSource(mainCamera, AudioType.BGM, index[nowSceneIndex]);
            asForBegin.loop = true;
            asForBegin.Play();
        };
    }

}
