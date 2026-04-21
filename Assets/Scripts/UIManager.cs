using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public enum ActiveMenu
    {
        None,
        Pause,
        Settings,
        Report,
        GameOver
    }

    public ActiveMenu currentMenu = ActiveMenu.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMenu(ActiveMenu menu)
    {
        currentMenu = menu;
        Time.timeScale = (menu == ActiveMenu.None) ? 1f : 0f;
    }

    public bool IsAnyMenuOpen() => currentMenu != ActiveMenu.None;
}