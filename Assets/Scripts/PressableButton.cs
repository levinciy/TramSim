using UnityEngine;

public class PressableButton : MonoBehaviour
{
    [Header("Настройки")]
    public Transform buttonPart; 
    public Vector3 pressedOffset = new Vector3(0, -0.003f, 0);
    public float pressSpeed = 8f; 

    [Header("Связь")]
    public KeyCode triggerKey = KeyCode.H; 


    private Vector3 originalLocalPosition;
    private Vector3 targetPosition;
    private bool isPressed = false;


    private AudioSource audioSource;
    public AudioClip pressSound;
    private bool isSoundPlaying = false;

    void Start()
    {


        originalLocalPosition = buttonPart.localPosition;
        targetPosition = originalLocalPosition;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true; 
        audioSource.volume = 0.7f;
    }

    void Update()
    {
        bool shouldPress = Input.GetKey(triggerKey);
        
        if (shouldPress && !isPressed)
        {
            isPressed = true;
            targetPosition = originalLocalPosition + pressedOffset;
            StartSound();
        }
        else if (!shouldPress && isPressed)
        {
            isPressed = false;
            targetPosition = originalLocalPosition;
            StopSound();
        }

        buttonPart.localPosition = Vector3.Lerp(buttonPart.localPosition, targetPosition, Time.deltaTime * pressSpeed);
    }


    private void StartSound()
    {
        if (pressSound != null && !isSoundPlaying)
        {
            audioSource.clip = pressSound;
            audioSource.Play();
            isSoundPlaying = true;
        }
    }

    private void StopSound()
    {
        if (isSoundPlaying)
        {
            audioSource.Stop();
            isSoundPlaying = false;
        }
    }


}