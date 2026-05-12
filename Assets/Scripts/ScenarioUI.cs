using UnityEngine;
using TMPro; // Если есть TextMeshPro, иначе используй UnityEngine.UI.Text
using UnityEngine.UI;

public class ScenarioUI : MonoBehaviour
{
    [Header(" Панели")]
    public GameObject notificationPanel;  
    public GameObject hintPanel;          
    
    [Header(" Тексты")]
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI hintText;
    
    // [Header(" Таймер")]
    // public TextMeshProUGUI timerText;
    // public GameObject timerPanel;
    
    [Header(" Цвета")]
    public Color successColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;
    
    [Header(" Настройки")]
    public float notificationDuration = 3f;
    public float fadeSpeed = 2f;
    
    private float notificationTimer = 0f;
    private bool isShowingNotification = false;

    void Update()
    {
        if (isShowingNotification)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0f)
            {
                HideNotification();
            }
        }
    }


    public void ShowScenarioNotification(string scenarioName, string message, bool isSuccess)
    {
        if (notificationPanel == null || notificationText == null) return;

        notificationPanel.SetActive(true);
        notificationText.text = $"<b>{scenarioName}</b>\n{message}";
        notificationText.color = isSuccess ? successColor : dangerColor;
        
        isShowingNotification = true;
        notificationTimer = notificationDuration;
    }


    public void ShowHint(string hint)
    {
        if (hintPanel == null || hintText == null) return;

        hintPanel.SetActive(true);
        hintText.text = hint;
    }


    public void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }


    // public void ShowTimer(float seconds)
    // {
    //     if (timerPanel == null || timerText == null) return;

    //     timerPanel.SetActive(true);
    //     timerText.text = Mathf.CeilToInt(seconds).ToString();
    //     timerText.color = seconds <= 3f ? dangerColor : warningColor;
    // }


    // public void HideTimer()
    // {
    //     if (timerPanel != null)
    //         timerPanel.SetActive(false);
    // }


    void HideNotification()
    {
        isShowingNotification = false;
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    // public void UpdateTimer(float remainingSeconds)
    // {
    //     if (timerPanel != null && timerPanel.activeSelf)
    //     {
    //         timerText.text = Mathf.CeilToInt(remainingSeconds).ToString();
    //         timerText.color = remainingSeconds <= 3f ? dangerColor : warningColor;
    //     }
    // }
}