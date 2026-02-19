using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // public GameObject pauseMenuUI; 
    // private bool isPaused = false;

    // void Start()
    // {
    //     if (pauseMenuUI != null)
    //         pauseMenuUI.SetActive(false);
    // }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         if (isPaused)
    //             Resume();
    //         else
    //             Pause();
    //     }
    // }

    // public void Resume()
    // {
    //     pauseMenuUI.SetActive(false);
    //     Time.timeScale = 1f; // возобновить время
    //     isPaused = false;
    // }

    // public void ReturnToMainMenu()
    // {
    //     Time.timeScale = 1f; // важно: вернуть время перед загрузкой!
    //     SceneManager.LoadScene("MainMenu");
    // }

    // private void Pause()
    // {
    //     pauseMenuUI.SetActive(true);
    //     Time.timeScale = 0f; // остановить всё: физику, анимации, Update
    //     isPaused = true;
    // }


    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance.currentMenu == UIManager.ActiveMenu.Pause)
            {
                Resume();
            }
            else if (UIManager.Instance.currentMenu == UIManager.ActiveMenu.None)
            {
                Pause();
            }
            // Иначе — игнорируем (открыто другое меню)
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        UIManager.Instance.SetMenu(UIManager.ActiveMenu.Pause);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        UIManager.Instance.SetMenu(UIManager.ActiveMenu.None);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}