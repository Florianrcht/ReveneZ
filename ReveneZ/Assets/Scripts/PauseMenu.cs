using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 

    public static bool GameIsPaused = false;

    private void Awake()
    {
        pauseMenuUI.SetActive(false);


        
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;

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
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;      
    }

    public void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
