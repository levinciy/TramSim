using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    
    [Header("UI")]
    public GameObject settingsPanel;
    public GameObject pauseMenuUI; 
    public Toggle disableScoringToggle;
    public Slider maxPenaltiesSlider;
    public TMP_InputField maxPenaltiesInput;

    [Header("Значения")]
    public bool isScoringDisabled = false;
    public int maxPenalties = 10;

    private bool savedScoringDisabled = false;
    private int savedMaxPenalties = 10;
    private const string KEY_SCORING_DISABLED = "ScoringDisabled";
    private const string KEY_MAX_PENALTIES = "MaxPenalties";

    void Start()
    {
        LoadSavedSettings();
    }

    // Открыть настройки → загрузить СОХРАНЁННЫЕ значения в UI
    // public void OpenSettings()
    // {
    //     if (pauseMenuUI != null)
    //         pauseMenuUI.SetActive(false);
        
    //     // Восстанавливаем UI из СОХРАНЁННЫХ значений
    //     UpdateUIFromSaved();

    //     settingsPanel.SetActive(true);
    //     pauseMenuUI.SetActive(false);
    //     Time.timeScale = 0f;
    // }

    // // Закрыть без применения
    // public void CloseSettings()
    // {
    //     settingsPanel.SetActive(false);
    //     if (pauseMenuUI != null)
    //         pauseMenuUI.SetActive(true);
    // }


    public void OpenSettings()
    {
        // Скрываем паузу
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        UpdateUIFromSaved();
        settingsPanel.SetActive(true);
        UIManager.Instance.SetMenu(UIManager.ActiveMenu.Settings);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            UIManager.Instance.SetMenu(UIManager.ActiveMenu.Pause);
        }
        else
        {
            UIManager.Instance.SetMenu(UIManager.ActiveMenu.None);
        }
    }

    // Применить → сохранить текущие значения как "официальные"
    public void ApplySettings()
    {
        // Берём значения из UI
        savedScoringDisabled = disableScoringToggle.isOn;
        if (int.TryParse(maxPenaltiesInput.text, out int inputVal))
        {
            savedMaxPenalties = Mathf.Clamp(inputVal, 1, 20);
        }
        else
        {
            savedMaxPenalties = Mathf.RoundToInt(maxPenaltiesSlider.value);
        }

        // Сохраняем
        PlayerPrefs.SetInt(KEY_SCORING_DISABLED, savedScoringDisabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_MAX_PENALTIES, savedMaxPenalties);
        PlayerPrefs.Save();

        Debug.Log($"✅ Настройки применены: Оценка = {!savedScoringDisabled}, Макс. ошибок = {savedMaxPenalties}");
    }

    // Обновить UI из СОХРАНЁННЫХ значений
    private void UpdateUIFromSaved()
    {
        disableScoringToggle.isOn = savedScoringDisabled;
        maxPenaltiesSlider.value = savedMaxPenalties;
        maxPenaltiesInput.text = savedMaxPenalties.ToString();
    }

    // Загрузить сохранённые настройки при старте
    private void LoadSavedSettings()
    {
        savedScoringDisabled = PlayerPrefs.GetInt(KEY_SCORING_DISABLED, 0) == 1;
        savedMaxPenalties = PlayerPrefs.GetInt(KEY_MAX_PENALTIES, 10);
    }

    // --- Синхронизация UI ---
    public void OnSliderChanged(float value)
    {
        int val = Mathf.RoundToInt(value);
        maxPenaltiesInput.text = val.ToString();
        // Не сохраняем — только синхронизируем UI
    }

    public void OnInputFieldChanged(string text)
    {
        if (int.TryParse(text, out int val))
        {
            val = Mathf.Clamp(val, 1, 20);
            maxPenaltiesSlider.value = val;
            maxPenaltiesInput.text = val.ToString(); // нормализуем ввод
        }
        else
        {
            // Если ввод некорректный — восстанавливаем последнее валидное
            maxPenaltiesInput.text = Mathf.RoundToInt(maxPenaltiesSlider.value).ToString();
        }
    }

    // Для внешнего доступа (например, DrivingEvaluator)
    public bool IsScoringEnabled() => !savedScoringDisabled;
    public int GetMaxPenalties() => savedMaxPenalties;
}