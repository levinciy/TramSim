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

    void Start()
    {


        originalLocalPosition = buttonPart.localPosition;
        targetPosition = originalLocalPosition;
    }

    void Update()
    {
        bool shouldPress = Input.GetKey(triggerKey);
        
        if (shouldPress && !isPressed)
        {
            isPressed = true;
            targetPosition = originalLocalPosition + pressedOffset;
        }
        else if (!shouldPress && isPressed)
        {
            isPressed = false;
            targetPosition = originalLocalPosition;
        }

        buttonPart.localPosition = Vector3.Lerp(buttonPart.localPosition, targetPosition, Time.deltaTime * pressSpeed);
    }


}