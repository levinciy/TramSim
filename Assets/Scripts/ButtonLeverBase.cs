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
        OnInit(); // Вызываем виртуальный метод вместо Start()
        UpdateTargetRotation();
    }

    void Update()
    {
        OnUpdate(); // Виртуальный метод для наследников
        if (leverHandle != null)
            leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Виртуальные методы — наследники могут переопределить
    protected virtual void OnInit() { }
    protected virtual void OnUpdate() { }
    
    // Абстрактный метод — должен быть реализован в наследнике
    protected abstract void UpdateTargetRotation();

    public void Refresh()
    {
        UpdateTargetRotation();
    }
}