using UnityEngine;
using TMPro;

public class DriverEvaluator : MonoBehaviour
{
 public GameSettings gameSettings;
 public MistakeSignal signal;
 private int penalties = 0;
 private System.Collections.Generic.List<string> violations = new System.Collections.Generic.List<string>();
 [Header ("Настройки")]
 public int maxPenalties = 10;
 public float violationCooldown = 2f;
 private float lastViolationTime = -10f; 
 [Header("UI")]
 public TMP_Text penaltyText;
 public TMP_Text violationText;


 public bool IsFailed() => penalties >= maxPenalties;


 public enum ReportMode
{
    Normal,
    GameOver
}
private ReportMode currentReportMode = ReportMode.Normal;

 void Start()
    {
        UpdatePenaltyUI();
        if (violationText != null)
            violationText.gameObject.SetActive(false);
    }
 
//     void Update()
// {

//     if (reportPanel.activeSelf){
//         if(currentReportMode == ReportMode.Normal){

//             if (Input.GetKeyDown(KeyCode.R)){
//                 HideReport();
//             }

//         }
//     }
//     else{
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             ShowReport();
//         }
//     }
// }

 public void addPenalty(string reason){

    if (gameSettings.isScoringDisabled)
        return;

    float currentTime = Time.time;
    if (currentTime - lastViolationTime < violationCooldown)
    {
        return;
    }

    //if (penalties >= maxPenalties)
    if (penalties >= (gameSettings?.maxPenalties ?? maxPenalties))
    {
        Debug.Log("Превышен лимит нарушений.");
        ShowGameOver();

    }
    penalties++;
    violations.Add(reason);


    UpdatePenaltyUI();
    signal?.triggerViolation();
  

 }

 private void UpdatePenaltyUI()
    {
        if (penaltyText != null)
            penaltyText.text = $"Штрафы: {penalties}/{maxPenalties}";
    }

    


    [Header("UI Отчёт")]
public GameObject reportPanel; 
public TMP_Text violationsListText; 

// public void ShowReport()
// {
//     if(currentReportMode!=ReportMode.Normal){
//         gameOverPanel.SetActive(false);
//     }
//     string report = GetViolationReport();
//     violationsListText.text = report;
//     reportPanel.SetActive(true);

//     Time.timeScale = 0f; 
// }

// public void HideReport()
// {
//     if(currentReportMode==ReportMode.Normal){
//         reportPanel.SetActive(false);
//         Time.timeScale = 1f;
//     }

//     else{
//         reportPanel.SetActive(false);
//         gameOverPanel.SetActive(true);

//     }
// }

public string GetViolationReport()
    {
        if (violations.Count == 0)
            return "Нарушений нет.";

        string report = $"Всего нарушений: {violations.Count}\n";
        for (int i = 0; i < violations.Count; i++)
        {
            report += $"{i + 1}. {violations[i]}\n";
        }
        return report;
    }

[Header("Game Over")]
public GameObject gameOverPanel;
public TMP_Text gameOverMessageText;

public void ReturnToMainMenu(){
    Time.timeScale = 1f;
    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
}

// public void ShowGameOver(){
//     currentReportMode = ReportMode.GameOver;
//     gameOverMessageText.text = "Превышено допустимое число нарушений!";
//     gameOverPanel.SetActive(true);
//     Time.timeScale=0f;
// }

void Update()
    {
        // Только если отчёт открыт в режиме Normal
        if (UIManager.Instance.currentMenu == UIManager.ActiveMenu.Report && 
            currentReportMode == ReportMode.Normal)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                HideReport();
            }
        }
        // Открытие отчёта — только из игры
        else if (UIManager.Instance.currentMenu == UIManager.ActiveMenu.None)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ShowReport();
            }
        }
    }

    public void ShowReport()
    {
        if (currentReportMode == ReportMode.GameOver)
        {
            gameOverPanel.SetActive(false);
        }
        violationsListText.text = GetViolationReport();
        reportPanel.SetActive(true);
        UIManager.Instance.SetMenu(UIManager.ActiveMenu.Report);
    }

    public void HideReport()
    {
        reportPanel.SetActive(false);
        if (currentReportMode == ReportMode.GameOver)
        {
            gameOverPanel.SetActive(true);
            UIManager.Instance.SetMenu(UIManager.ActiveMenu.GameOver);
        }
        else
        {
            UIManager.Instance.SetMenu(UIManager.ActiveMenu.None);
        }
    }

    public void ShowGameOver()
    {
        currentReportMode = ReportMode.GameOver;
        gameOverMessageText.text = "Превышено допустимое число нарушений!";
        gameOverPanel.SetActive(true);
        UIManager.Instance.SetMenu(UIManager.ActiveMenu.GameOver);
    }

}
