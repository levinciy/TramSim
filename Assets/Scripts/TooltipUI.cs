using UnityEngine;
using TMPro; // Или используй UnityEngine.UI.Text если нет TextMeshPro

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("UI Элементы")]
    public GameObject tooltipPanel;      // Панель с подсказкой
    public TextMeshProUGUI tooltipText;  // Текст подсказки
    
    [Header("Настройки")]
    public float fadeSpeed = 5f;
    public Vector3 startPosition = new Vector3(20, 20, 0); // Левый нижний угол
    public Vector3 hiddenPosition = new Vector3(20, -100, 0); // Спрятан

    private string currentText = "";
    private bool isVisible = false;

    void Awake()
    {
        Instance = this;
        
        if (tooltipPanel == null)
            tooltipPanel = gameObject;
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (tooltipPanel != null)
        {
            // Плавное появление/исчезновение
            Vector3 targetPos = isVisible ? startPosition : hiddenPosition;
            tooltipPanel.transform.localPosition = Vector3.Lerp(
                tooltipPanel.transform.localPosition,
                targetPos,
                Time.deltaTime * fadeSpeed
            );
            
            // Показываем/скрываем панель
            if (Vector3.Distance(tooltipPanel.transform.localPosition, startPosition) < 5f)
                tooltipPanel.SetActive(true);
            else if (!isVisible)
                tooltipPanel.SetActive(false);
        }
    }

    public void ShowTooltip(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        currentText = text;
        if (tooltipText != null)
            tooltipText.text = text;
        
        isVisible = true;
    }

    public void HideTooltip()
    {
        isVisible = false;
        currentText = "";
    }

    public void UpdateTooltip(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            currentText = text;
            if (tooltipText != null)
                tooltipText.text = text;
        }
    }
}