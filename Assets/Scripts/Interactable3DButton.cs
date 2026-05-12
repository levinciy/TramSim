using UnityEngine;
using UnityEngine.Events;

public class Interactable3DButton : MonoBehaviour
{

    public string tooltipText = "Кнопка";
    [Header("События")]
    public UnityEvent onPress;
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    
    [Header("Настройки")]
    public bool canPress = true;
    public float pressCooldown = 0.3f;
    private PressableButton PButton;
    
    private bool isHovered;
    private float lastPressTime;


    void Awake()
    {
        // Автоматически ищем PressableButton на этом же объекте (или родителе)
        PButton = GetComponentInParent<PressableButton>();
    }

    public void OnPress()
    {
        if (!canPress || Time.time - lastPressTime < pressCooldown)
            return;
            
        Debug.Log($"🔘 Нажата кнопка: {name}");
        lastPressTime = Time.time;
        //onPress.Invoke();
        PButton.TriggerPress();
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        TooltipUI.Instance?.ShowTooltip(tooltipText);
        //onHoverEnter.Invoke();
    }

    public void OnHoverExit()
    {

        isHovered = false;
        TooltipUI.Instance?.HideTooltip();
        //onHoverExit.Invoke();
    }
}