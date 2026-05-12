using UnityEngine;

public class RotarySwitch : ButtonLeverBase
{

    public string tooltipText = "Переключатель";
    public string[] positionNames;
    //[Header("Тип переключателя")]
    public enum SwitchType { Arrows, Gear, Custom, TurnSignals }
    public SwitchType switchType;
    
    //[Header("Позиции")]
    public int numberOfPositions = 3;
    public int initialPosition = 1;
    public float startAngle = -45f;
    public float endAngle = 45f;
    
   // [Header("Ось вращения")]
    public Vector3 rotationAxis = Vector3.up; // 🎯 По умолчанию Y (0,1,0)
    
   // [Header("Клавиши")]
    public KeyCode[] positionKeys;
    
    [Header("Ссылка на трамвай")]
    public TramOnRails tramController;

    // События
    public System.Action<SwitchMode> OnArrowsChanged;
    public System.Action<TramDirection> OnGearChanged;
    public System.Action<int> OnCustomChanged;
    public System.Action<TurnSignalState> OnTurnSignalChanged;

    private int currentPosition;
    private float currentAngleOffset;
    private Quaternion baseRotation; 

    protected override void OnInit()
    {
        currentPosition = Mathf.Clamp(initialPosition, 0, numberOfPositions - 1);
        
        if (numberOfPositions <= 1)
        {
            currentAngleOffset = startAngle;
        }
        else
        {
            float anglePerPosition = (endAngle - startAngle) / (numberOfPositions - 1);
            currentAngleOffset = startAngle + (currentPosition * anglePerPosition);
        }
        
        // 🎯 Сохраняем базовое вращение
        baseRotation = leverHandle.localRotation;
        
        UpdateTargetRotation();
        ApplyPosition();
    }

    protected override void OnUpdate()
    {
        HandleKeyboardInput();
    }

    protected override void UpdateTargetRotation()
    {
        // 🎯 Вращаем вокруг заданной оси
        targetRotation = baseRotation * Quaternion.AngleAxis(currentAngleOffset, rotationAxis);
    }

    public void SetPosition(int position)
    {
        position = Mathf.Clamp(position, 0, numberOfPositions - 1);
        
        // if (position != currentPosition)
        // {
            currentPosition = position;
            
            if (numberOfPositions <= 1)
            {
                currentAngleOffset = startAngle;
            }
            else
            {
                float anglePerPosition = (endAngle - startAngle) / (numberOfPositions - 1);
                currentAngleOffset = startAngle + (currentPosition * anglePerPosition);
            }
            
            UpdateTargetRotation();
            ApplyPosition();
            
            Debug.Log($"🔀 {switchType}: позиция {currentPosition}, угол {currentAngleOffset:F1}°");
        // }
    }

    void ApplyPosition()
    {
        switch (switchType)
        {
            case SwitchType.Arrows:
                SwitchMode mode = (SwitchMode)currentPosition;
                OnArrowsChanged?.Invoke(mode);
                if (tramController != null)
                    tramController.switchMode = mode;
                break;
                
            case SwitchType.Gear:
                TramDirection dir = (currentPosition == 0) ? TramDirection.Reverse : TramDirection.Forward;
                OnGearChanged?.Invoke(dir);
                if (tramController != null)
                    tramController.currentDirection = dir;
                break;
                
            case SwitchType.TurnSignals:
                TurnSignalState signal = (TurnSignalState)currentPosition;
                OnTurnSignalChanged?.Invoke(signal);
                tramController.turnSignal = signal;
                break;
            
            case SwitchType.Custom:
                OnCustomChanged?.Invoke(currentPosition);
                break;

            

        }

        string positionName = (positionNames != null && currentPosition < positionNames.Length) 
            ? positionNames[currentPosition] 
            : $"Позиция {currentPosition + 1}";
    
        string fullTooltip = $"{tooltipText}: {positionName}";
        if (TooltipUI.Instance != null && isHovered)
        {
            TooltipUI.Instance.UpdateTooltip(fullTooltip);
        }
    }

    void HandleKeyboardInput()
    {
        if (positionKeys != null)
        {
            for (int i = 0; i < positionKeys.Length && i < numberOfPositions; i++)
            {
                if (Input.GetKeyDown(positionKeys[i]))
                {
                    SetPosition(i);
                }
            }
        }
    }

    // === ДЛЯ МЫШИ ===
    private float dragStartAngle;
    //private Vector2 lastMousePos;

    public void StartDrag()
    {
        dragStartAngle = currentAngleOffset;
        //lastMousePos = Input.mousePosition;
        Debug.Log($"🖱️ StartDrag: начальный угол {dragStartAngle}");
    }

    public void HandleDrag(float mouseX, float mouseY)
    {
        // Определяем основное направление движения мыши
        float delta = Mathf.Abs(mouseX) > Mathf.Abs(mouseY) ? mouseX : -mouseY;
        
        // 🎯 НАКОПЛЕНИЕ: прибавляем изменение к текущему углу, а не к стартовому
        float sensitivity = 1.0f; // Теперь 1.0 будет работать адекватно
        currentAngleOffset += delta * sensitivity;
        currentAngleOffset = Mathf.Clamp(currentAngleOffset, startAngle, endAngle);
        
        // Обновляем вращение
        targetRotation = baseRotation * Quaternion.AngleAxis(currentAngleOffset, rotationAxis);
        
        // Обновляем визуальный индекс позиции
        if (numberOfPositions > 1)
        {
            float anglePerPosition = (endAngle - startAngle) / (numberOfPositions - 1);
            currentPosition = Mathf.RoundToInt((currentAngleOffset - startAngle) / anglePerPosition);
            currentPosition = Mathf.Clamp(currentPosition, 0, numberOfPositions - 1);
        }
    }

    public void EndDrag()
    {
        Debug.Log($"🖱️ EndDrag: текущий угол {currentAngleOffset}");
        
        if (numberOfPositions > 1)
        {
            
            float anglePerPosition = (endAngle - startAngle) / (numberOfPositions - 1);
            int nearestPos = Mathf.RoundToInt((currentAngleOffset - startAngle) / anglePerPosition);
            nearestPos = Mathf.Clamp(nearestPos, 0, numberOfPositions - 1);
            Debug.Log(nearestPos);
            Debug.Log( currentPosition);
            SetPosition(nearestPos);
        }
    }

    private bool isHovered = false;

    public void OnHoverEnter()
    {
        isHovered = true;
        string positionName = (positionNames != null && currentPosition < positionNames.Length) 
            ? positionNames[currentPosition] 
            : $"Позиция {currentPosition + 1}";
        
        TooltipUI.Instance?.ShowTooltip($"{tooltipText}: {positionName}");
    }

    public void OnHoverExit()
    {
        isHovered = false;
        TooltipUI.Instance?.HideTooltip();

    }

    

}