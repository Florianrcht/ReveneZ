using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 

    public static bool GameIsPaused = false;

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;   
        GameIsPaused = true;                
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;   
        GameIsPaused = false;        
    }
}
