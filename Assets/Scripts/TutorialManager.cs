using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialUI;
    public TMP_Text stepText;
    public Button skipButton;

    [Header("Зоны — пустые GameObjects вдоль дороги")]
    public Transform[] triggerZones; 

    [Header("Сообщения")]
    public string[] stepMessages = {
        "Добро пожаловать! Здесь вы освоите управление трамваем.",
        "Движение вперёд: удерживайте <color=#00FF00>W</color>.",
        "Торможение: нажмите <color=#FF0000>S</color>.",
        "Звонок: удерживайте <color=#FFFF00>H</color>.",
        "Стрелки: <color=#00FFFF>N</color>, <color=#00FFFF>L</color>, <color=#00FFFF>D</color>.",
        "Реверс: <color=#FF8000>E</color>. Остановитесь перед включением!",
        "Остановка: сбросьте скорость до 0 в зоне.",
        "Поздравляем, теперь вы знакомы с основами управления, попробуйте применить"
    };

    private int currentStep = 0;
    private bool isShowingHint = false;
    private float timeShown = 0f;
    private const float HINT_DURATION = 5f;
    private Vector3 end = new Vector3(1000f,1000f,1000f);

    void Start()
    {
        if (tutorialUI != null) tutorialUI.SetActive(false);
        if (skipButton != null) skipButton.onClick.AddListener(NextStep);

        foreach (var zone in triggerZones)
        {
            if (zone != null)
                zone.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (isShowingHint) return;

        for (int i = 0; i < triggerZones.Length; i++)
        {
            if (triggerZones[i] == null) continue;

            float dist = Vector3.Distance(transform.position, triggerZones[i].position);
            if (dist < 2.2f && i == currentStep)
            {
                ShowHint(i);

                //triggerZones[i].gameObject.SetActive(false);
                triggerZones[i].position = end;
                break;
            }
        }
    }

    void ShowHint(int stepIndex)
    {
        if (stepIndex != currentStep) return;

        if (tutorialUI != null)
        {
            stepText.text = stepMessages[stepIndex];
            tutorialUI.SetActive(true);
            isShowingHint = true;
            timeShown = 0f;

            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    void Update()
    {
        if (!isShowingHint) return;

        timeShown += Time.unscaledDeltaTime;

        if (timeShown >= HINT_DURATION || Input.GetKeyDown(KeyCode.Space))
        {
            HideHint();
            if (currentStep < triggerZones.Length - 1)
                currentStep++;
            else
                FinishTutorial();
        }
    }

    void HideHint()
    {
        if (tutorialUI != null) tutorialUI.SetActive(false);
        isShowingHint = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void NextStep()
    {
        HideHint();
        if (currentStep < triggerZones.Length - 1)
            currentStep++;
        else
            FinishTutorial();
    }

    void FinishTutorial()
    {
        Debug.Log("✅ Туториал завершён. Переход к основной сцене...");
        Invoke("LoadMainScene", 1f);
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}