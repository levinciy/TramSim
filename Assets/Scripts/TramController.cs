using UnityEngine;
using UnityEngine.InputSystem;

public class TramController : MonoBehaviour
{
    // 🔹 Эти поля будут видны в Inspector!
    public InputActionReference accelerateAction;
    public InputActionReference brakeAction;
    public InputActionReference steerLeftAction;
    public InputActionReference steerRightAction;
    //public InputActionReference steerAction;

    public float acceleration = 2f;
    public float maxSpeed = 15f;
    public float brakePower = 5f;
    public float turnSpeed = 30f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isSteeringLeft = false;
    private bool isSteeringRight = false;
    //private Vector2 steerInput = Vector2.zero;
    //private float horizontalInput = 0f;

    void Start()
{
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;

    // 🔹 Активируем все действия!
    if (accelerateAction != null) accelerateAction.action.Enable();
    if (brakeAction != null) brakeAction.action.Enable();
    if (steerLeftAction != null) steerLeftAction.action.Enable();
    if (steerRightAction != null) steerRightAction.action.Enable();
    // if (steerAction != null){
    //     steerAction.action.Enable();
    //     steerAction.action.performed += OnSteerPerformed;
    // }

    Debug.Log("Input actions enabled.");
}

void OnDestroy()
{
    // 🔹 Деактивируем при уничтожении
    if (accelerateAction != null) accelerateAction.action.Disable();
    if (brakeAction != null) brakeAction.action.Disable();
    if (steerLeftAction != null) steerLeftAction.action.Disable();
    if (steerRightAction != null) steerRightAction.action.Disable();
    // if (steerAction != null){
    //if (steerAction != null) steerAction.action.Disable();
}
/*
    // private void OnAccelerateStarted(InputAction.CallbackContext ctx)
    // {
    //     // Начало нажатия W
    // }

    // private void OnAccelerateCanceled(InputAction.CallbackContext ctx)
    // {
    //     // Отпускание W — но мы будем управлять скоростью в Update
    // }

    // private void OnBrakeStarted(InputAction.CallbackContext ctx)
    // {
    //     // Начало нажатия S
    // }

    // private void OnBrakeCanceled(InputAction.CallbackContext ctx)
    // {
    //     // Отпускание S
    // }

    // private void OnSteerPerformed(InputAction.CallbackContext ctx)
    // {
    //     horizontalInput = ctx.ReadValue<float>();
    // }
    // private void OnSteerPerformed(InputAction.CallbackContext ctx)
    // {
    //     steerInput = ctx.ReadValue<Vector2>();
    // }
    // public void OnSteerLeft(InputAction.CallbackContext context)
    // {
    //     isSteeringLeft = context.ReadValueAsButton();
    // }

    // public void OnSteerRight(InputAction.CallbackContext context)
    // {
    //     isSteeringRight = context.ReadValueAsButton();
    // }
*/
    void Update()
    {
        // --- Управление скоростью ---
        bool isAccelerating = accelerateAction?.action.IsPressed() == true;
        bool isBraking = brakeAction?.action.IsPressed() == true;

        if (isAccelerating)
            currentSpeed += acceleration * Time.deltaTime;
        else if (isBraking)
            currentSpeed -= brakePower * Time.deltaTime;
        else
            currentSpeed *= 0.95f; // трение

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed / 3f, maxSpeed);

        // --- Поворот ---
        // float turn = 0f;
        // if (currentSpeed > 0.1f)
        //     turn = horizontalInput * turnSpeed * Time.deltaTime;

        // Debug.Log("Horizontal Input: " + horizontalInput + " | Turn: " + turn);
        // transform.Rotate(0, turn, 0);
        // float horizontalInput = steerInput.x; // ← именно так!
        // float turn = 0f;
        // if (currentSpeed > 0.1f)
        //     turn = horizontalInput * turnSpeed * Time.deltaTime;

        // Debug.Log("Horizontal Input: " + horizontalInput + " | Turn: " + turn);
        // transform.Rotate(0, turn, 0);

        bool isSteeringLeft = steerLeftAction?.action.IsPressed() == true;
        bool isSteeringRight = steerRightAction?.action.IsPressed() == true;

        float horizontalInput = 0f;
        if (isSteeringLeft) horizontalInput = -1f;
        if (isSteeringRight) horizontalInput = +1f;

        float turn = 0f;
        if (currentSpeed > 0.1f)
            turn = horizontalInput * turnSpeed * Time.deltaTime;
        Debug.Log("Horizontal Input: " + horizontalInput + " | Turn: " + turn);
        transform.Rotate(0, turn, 0);

        // --- Движение ---
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        rb.linearVelocity = forward * currentSpeed;
        //Debug.Log("Speed: " + currentSpeed.ToString("F2") + " | Position: " + transform.position);
    }
}