using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasueMenu : MonoBehaviour
{
    public static bool GameIsPause = false;
    public GameObject PauseMenu;

    void Start()
    {
        GameIsPause = false;
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }
    void Resume() 
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPause = false;
    }
    void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPause = true;
    }




}
