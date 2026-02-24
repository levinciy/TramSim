using UnityEngine;

public class PanelButtonsController : MonoBehaviour{
    [Header("Настройки")]
    public Transform leverHandle; // ссылка на "ПодвижнаяЧасть"
    // public float angleNeutral = 0f;   // например, 0°
    // public float angleLeft = -60f;    // влево — минус
    // public float angleRight = +60f;   // вправо — плюс

    // private float[] angleNeutral= {2.063f, 90f, -41.038f};
    // private float[] angleLeft = {-18.951f, 64.435f,-37.157f};
    // private float[] angleRight = {21.667f, 114.743f, -35.797f};

    public Vector3 angleNeutralOffset = Vector3.zero;   // например, 0,0,0
    public Vector3 angleLeftOffset = new Vector3(0, -45, 0);   // только Y-поворот
    public Vector3 angleRightOffset = new Vector3(0, +45, 0);

    [Header("Связь с логикой")]
    public TramOnRails tramController; // ссылка на трамвай

    private Quaternion startRotation;
    private float rotationSpeed = 5f;
    private Quaternion targetRotation;

    void Start()
    {
        if (leverHandle == null)
        {
            Debug.LogError("⚠️ Не назначена подвижная часть!");
            return;
        }

        startRotation = leverHandle.localRotation;
        UpdateLeverPosition();
    }

    void Update()
    {
        UpdateLeverPosition();
        leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void SetMode(SwitchMode mode)
    {
        // float[] targetAngle = mode switch
        // {
        //     SwitchMode.Neutral => angleNeutral,
        //     SwitchMode.Left => angleLeft,
        //     SwitchMode.Right => angleRight,
        //     _ => angleNeutral
        // };

        // targetRotation = Quaternion.Euler(targetAngle[0], targetAngle[1], targetAngle[2]);
        //leverHandle.localRotation = Quaternion.Euler(targetAngle[0], targetAngle[1], targetAngle[2]);



        Vector3 offset = mode switch
        {
            SwitchMode.Neutral => angleNeutralOffset,
            SwitchMode.Left => angleLeftOffset,
            SwitchMode.Right => angleRightOffset,
            _ => angleNeutralOffset
        };

        // Финальный поворот = базовый + относительный
        targetRotation = startRotation * Quaternion.Euler(offset);
    }

    private void UpdateLeverPosition()
    {
        if (tramController == null) return;

        // Синхронизируем с текущим режимом трамвая
        SetMode(tramController.switchMode);
    }
}