using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    public float sensitivity = 2f;
    public float minAngle = -60f, maxAngle = 60f;
    private float verticalAngle = 0f;

    private TramControls input;

    void Awake()
    {
        input = new TramControls();
        input.Camera.Look.performed += ctx => OnLook(ctx.ReadValue<Vector2>());
        input.Camera.ToggleCursor.performed += ctx => ToggleCursor();
    }

    void OnEnable() => input.Camera.Enable();
    void OnDisable() => input.Camera.Disable();

    void OnLook(Vector2 delta)
    {

        if (UIManager.Instance != null && UIManager.Instance.IsAnyMenuOpen())
            return;
        // Горизонталь
        transform.Rotate(Vector3.up * delta.x * sensitivity);

        // Вертикаль
        verticalAngle -= delta.y * sensitivity;
        verticalAngle = Mathf.Clamp(verticalAngle, minAngle, maxAngle);

        Vector3 euler = transform.localEulerAngles;
        euler.x = verticalAngle;
        euler.z = 0f;
        transform.localEulerAngles = euler;
    }

    void ToggleCursor()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsAnyMenuOpen())
            return;
        bool locked = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = locked;
    }

    void Update()
    {
        // 🔑 ПРОВЕРКА: если открыто любое меню — отключаем камеру
        if (UIManager.Instance != null && UIManager.Instance.IsAnyMenuOpen())
        {
            // Освобождаем курсор, чтобы можно было кликать по UI
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return; // Выходим из Update, не обрабатываем ввод
        }

        // Если меню закрыто — курсор должен быть заблокирован
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}