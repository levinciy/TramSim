using UnityEngine;

public class TurnSignalVisuals : MonoBehaviour
{
    [Header("Ссылка на трамвай")]
    public TramOnRails tramController;

    [Header("Задние фары")]
    public Renderer leftRearLight;   // Левая задняя фара
    public Renderer rightRearLight;  // Правая задняя фара
    
    [Header("Материалы фар")]
    public Material lightOffMaterial;  // Выключенная фара (темная)
    public Material lightOnMaterial;   // Включенная фара (с эмиссией)
    
    [Header("Звук поворотника")]
    public AudioClip clickSound;
    public AudioSource audioSource;
    
    [Header("Настройки")]
    public float blinkInterval = 0.5f;  // Интервал мигания (сек)
    public float soundInterval = 0.5f;  // Интервал звука (сек)
    
    private TurnSignalState currentState = TurnSignalState.Neutral;
    private bool isLightOn = false;
    private float blinkTimer = 0f;
    private float soundTimer = 0f;
    private bool isSoundPlaying = false;

    void Start()
    {
        if (tramController == null)
            tramController = GetComponentInParent<TramOnRails>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        if (tramController == null) return;

        TurnSignalState newState = tramController.turnSignal;

        // Если состояние изменилось
        if (newState != currentState)
        {
            currentState = newState;
            isLightOn = false;
            blinkTimer = 0f;
            soundTimer = 0f;
            
            // Сбрасываем все фары
            UpdateLights();
        }

        // Если поворотник включен (не нейтраль)
        if (currentState != TurnSignalState.Neutral)
        {
            // Мигание
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                isLightOn = !isLightOn;
                UpdateLights();
            }

            // Звук
            soundTimer += Time.deltaTime;
            if (soundTimer >= soundInterval && clickSound != null)
            {
                soundTimer = 0f;
                PlayClickSound();
            }
        }
        else
        {
            // Выключаем звук, если поворотник выключен
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void UpdateLights()
    {
        // Левая фара
        if (leftRearLight != null)
        {
            bool shouldLight = (currentState == TurnSignalState.Left && isLightOn);
            leftRearLight.material = shouldLight ? lightOnMaterial : lightOffMaterial;
        }

        // Правая фара
        if (rightRearLight != null)
        {
            bool shouldLight = (currentState == TurnSignalState.Right && isLightOn);
            rightRearLight.material = shouldLight ? lightOnMaterial : lightOffMaterial;
        }
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

    // Публичный метод для принудительного обновления (если нужно)
    public void ForceUpdate()
    {
        currentState = tramController.turnSignal;
        UpdateLights();
    }
}