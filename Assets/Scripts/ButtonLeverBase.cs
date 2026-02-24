using UnityEngine;

public abstract class ButtonLeverBase : MonoBehaviour
{
    [Header("Общие настройки")]
    public Transform leverHandle;
    public float rotationSpeed = 5f;

    protected Quaternion startRotation;
    protected Quaternion targetRotation;

    void Start()
    {
        if (leverHandle == null)
        {
            Debug.LogError("⚠️ leverHandle не назначен!");
            return;
        }

        startRotation = leverHandle.localRotation;
        UpdateTargetRotation();
    }

    void Update()
    {
        UpdateTargetRotation();
        if (leverHandle != null)
            leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Абстрактный метод — должен быть реализован в наследнике
    protected abstract void UpdateTargetRotation();

    // Вызывается извне (например, при изменении логики)
    public void Refresh()
    {
        UpdateTargetRotation();
    }
}