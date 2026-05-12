using UnityEngine;

public class CenterReticleInteractor : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask buttonLayer;
    public float interactDistance = 3f;
    public GameObject reticleUI;
    public UnityEngine.UI.Image reticleImage;
    
    [Header("Настройки")]
    public float clickCooldown = 0.2f;
    
    private Interactable3DButton currentButton;
    private RotarySwitch currentSwitch;
    private float lastClickTime;
    private bool isClicking;
    private bool isDraggingSwitch = false;
    
    // 🎯 Для блокировки камеры во время drag
    private FreeLookCamera freeLookCamera;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        if (reticleUI == null)
            reticleImage = reticleUI.GetComponent<UnityEngine.UI.Image>();
        
        // Ищем камеру с FreeLookCamera
        freeLookCamera = FindObjectOfType<FreeLookCamera>();
    }

    void Update()
    {
        // Пропускаем, если открыто меню
        if (UIManager.Instance != null && UIManager.Instance.IsAnyMenuOpen())
        {
            if (reticleImage != null) reticleImage.color = Color.white;
            return;
        }

        // 🎯 Если тащим переключатель — не обрабатываем луч
        if (isDraggingSwitch)
        {
            HandleSwitchDrag();
            return;
        }

        // Луч из центра экрана
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(center);
        RaycastHit hit;

        bool hitButton = false;

        if (Physics.Raycast(ray, out hit, interactDistance, buttonLayer))
        {
            Interactable3DButton btn = hit.collider.GetComponentInParent<Interactable3DButton>();
            RotarySwitch rotarySwitch = hit.collider.GetComponentInParent<RotarySwitch>();
            
            if (rotarySwitch != null)
            {
                hitButton = true;
                //currentSwitch = rotarySwitch;
                if (rotarySwitch != currentSwitch)
                    {
                        currentSwitch?.OnHoverExit();
                        currentSwitch = rotarySwitch;
                        currentSwitch.OnHoverEnter();
                    }
                
                // Начало drag
                if (Input.GetMouseButtonDown(0))
                {
                    StartSwitchDrag();
                }
            }
            else if (btn != null)
            {
                hitButton = true;
                
                if (btn != currentButton)
                {
                    currentButton?.OnHoverExit();
                    currentButton = btn;
                    currentButton?.OnHoverEnter();
                }

                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && 
                    Time.time - lastClickTime > clickCooldown && !isClicking)
                {
                    isClicking = true;
                    lastClickTime = Time.time;
                    btn.OnPress();
                    Invoke(nameof(ResetClick), 0.1f);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (currentButton != null)
                    {
                        var pressableBtn = currentButton.GetComponentInParent<PressableButton>();
                        pressableBtn?.TriggerRelease();
                    }
                }
            }
        }
        else
        {
            if (currentButton != null)
            {
                currentButton.OnHoverExit();
                currentButton = null;
            }
            if (currentSwitch != null)
            {
                currentSwitch.OnHoverExit();
                currentSwitch = null;
            }
        }

        // Меняем цвет метки
        if (reticleImage != null)
        {

            if (hitButton)
                reticleImage.color = Color.orange;
            else
                reticleImage.color = Color.white;
        }
    }

    // 🎯 Начало drag переключателя
    void StartSwitchDrag()
    {
        if (currentSwitch == null) return;
        
        isDraggingSwitch = true;
        
        // Блокируем камеру
        if (freeLookCamera != null)
            freeLookCamera.enabled = false;
        
        // Блокируем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Скрываем метку
        if (reticleUI != null)
            reticleUI.SetActive(false);
        
        // Сообщаем переключателю
        currentSwitch.StartDrag();
        
        Debug.Log("🖱️ Начал drag переключателя (камера заблокирована)");
    }

    // 🎯 Обработка drag
    void HandleSwitchDrag()
    {
        if (currentSwitch == null)
        {
            EndSwitchDrag();
            return;
        }
        
        // Получаем движение мыши (в заблокированном режиме)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        currentSwitch.HandleDrag(mouseX, mouseY);
        
        // Отпускание мыши
        if (Input.GetMouseButtonUp(0))
        {
            EndSwitchDrag();
        }
    }

    // 🎯 Конец drag
    void EndSwitchDrag()
    {
        if (currentSwitch != null)
        {
            currentSwitch.EndDrag();
        }
        
        isDraggingSwitch = false;
        currentSwitch = null;
        
        // Возвращаем камеру
        if (freeLookCamera != null)
            freeLookCamera.enabled = true;
        
        // Возвращаем курсор
        if (UIManager.Instance == null || !UIManager.Instance.IsAnyMenuOpen())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Показываем метку
        if (reticleUI != null)
            reticleUI.SetActive(true);
        
        Debug.Log("🖱️ Закончил drag переключателя (камера разблокирована)");
    }

    void ResetClick()
    {
        isClicking = false;
    }

    void OnDisable()
    {
        CancelInvoke();
        currentButton = null;
        currentSwitch = null;
        isDraggingSwitch = false;
        
        // Возвращаем камеру
        if (freeLookCamera != null)
            freeLookCamera.enabled = true;
    }
}