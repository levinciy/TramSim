using UnityEngine;
using System.Collections;

public class TrafficLight : MonoBehaviour {

    public enum LightColors
    {
        Red,
        Yellow,
        Green
    }
    [Header("Времена работы (секунды)")]
    public float greenDuration = 10f;
    public float yellowDuration = 3f;
    public float redDuration = 10f;

    [Header("Визуал")]
    public Renderer redLight;
    public Renderer yellowLight;
    public Renderer greenLight;

    public Material lightOnMaterialRed;  // Материал с эмиссией/свечением
    public Material lightOnMaterialYellow; 
    public Material lightOnMaterialGreen; 
    public Material lightOffMaterial; // Матовый/выключенный материал

    [Header("Состояние")]
    public LightColors CurrentLight ;
    public bool IsRed => CurrentLight == LightColors.Red;

    // Событие для других скриптов (например, трамвая)
    public System.Action<LightColors> OnLightChanged;

    private Coroutine cycleCoroutine;

    void Start()
    {
        // if (lightOnMaterial == null || lightOffMaterial == null)
        // {
        //     Debug.LogWarning($"[TrafficLight] Не назначены материалы on/off на {name}. Использую цвет как fallback.");
        // }
        StartCycle();
    }

    void OnDisable()
    {
        if (cycleCoroutine != null) StopCoroutine(cycleCoroutine);
    }

    public void StartCycle()
    {
        if (cycleCoroutine != null) StopCoroutine(cycleCoroutine);
        cycleCoroutine = StartCoroutine(LightCycle());
    }

    private IEnumerator LightCycle()
    {
        while (true)
        {
            SetLight(LightColors.Green);
            yield return new WaitForSeconds(greenDuration);

            SetLight(LightColors.Yellow);
            yield return new WaitForSeconds(yellowDuration);

            SetLight(LightColors.Red);
            yield return new WaitForSeconds(redDuration);
        }
    }

    private void SetLight(LightColors color)
    {
        CurrentLight = color;
        UpdateVisuals();
        OnLightChanged?.Invoke(color);
    }

    private void UpdateVisuals()
    {
        // Простой вариант: переключение материалов
        if (redLight)   redLight.material = (CurrentLight == LightColors.Red) ? lightOnMaterialRed : lightOffMaterial;
        if (yellowLight) yellowLight.material = (CurrentLight == LightColors.Yellow) ? lightOnMaterialYellow : lightOffMaterial;
        if (greenLight) greenLight.material = (CurrentLight == LightColors.Green) ? lightOnMaterialGreen : lightOffMaterial;
    }

    [Header("Позиция стоп-линии")]
    // Смещение стоп-линии от центра светофора (вперёд по оси Z светофора)
    public float stopLineOffset = 2f; 

    public Vector3 StopLinePosition => transform.position ;

    // public void ForceSetLight(LightColors color)
    // {
    //     SetLight(color);
    // }

    public void ForceSetLight(LightColors color)
    {
        // Останавливаем автоматический цикл
        if (cycleCoroutine != null) StopCoroutine(cycleCoroutine);
        
        // Устанавливаем цвет и обновляем визуал
        SetLight(color);
        cycleCoroutine = StartCoroutine(LightCycleFromColor(color));
        
        //Debug.Log($"🚦 Светофор {name} принудительно переключён на {color}");
    }

    private IEnumerator LightCycleFromColor(LightColors startColor)
    {
        // Определяем, с какого цвета продолжать
        if (startColor == LightColors.Green)
        {
            yield return new WaitForSeconds(greenDuration);
            SetLight(LightColors.Yellow);
        }
        
        if (startColor == LightColors.Yellow || startColor == LightColors.Green)
        {
            yield return new WaitForSeconds(yellowDuration);
            SetLight(LightColors.Red);
        }
        
        // Дальше обычный цикл
        while (true)
        {
            yield return new WaitForSeconds(redDuration);
            SetLight(LightColors.Green);
            yield return new WaitForSeconds(greenDuration);
            SetLight(LightColors.Yellow);
            yield return new WaitForSeconds(yellowDuration);
            SetLight(LightColors.Red);
        }
    }
}

