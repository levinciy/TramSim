using UnityEngine;

public class TramOnRails : MonoBehaviour
{
    [Header("Физика")]
    public float maxSpeed = 15f;
    public float acceleration = 1.5f;
    public float brakePower = 3f;
    public float passiveDeceleration = 0.8f;

    [Header("Маршрут")]
    public RouteNode startNode; // задаётся в инспекторе
    private RouteNode currentNode;

    [Header("Управление")]
    public SwitchMode switchMode = SwitchMode.Neutral; // <-- использует глобальный enum

    private Rigidbody rb;
    private float currentSpeed = 0f;

    public TramDirection currentDirection = TramDirection.Forward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        currentNode = startNode;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Q)) currentDirection = TramDirection.Forward;
        if (Input.GetKeyDown(KeyCode.E)) currentDirection = TramDirection.Reverse;

    // Управление скоростью
        bool isAccelerating = Input.GetKey(KeyCode.W);
        bool isBraking = Input.GetKey(KeyCode.S);

        if (isAccelerating)
            currentSpeed += acceleration * Time.deltaTime;
        else if (isBraking)
            currentSpeed -= brakePower * Time.deltaTime;
        else
            currentSpeed -= passiveDeceleration * Time.deltaTime;

        // Скорость не может быть отрицательной (направление задаётся отдельно)
        currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

        // Управление режимом стрелок (временно)
        if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
        if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
        if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;
    }

    void FixedUpdate(){
        if (currentNode == null || currentSpeed <= 0.01f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // --- Определяем направление движения ---
        Vector3 moveDirection;
        RouteNode nextNode = null;

        if (currentDirection == TramDirection.Forward)
        {
            // Вперёд: к следующему узлу (по логике стрелок)
            nextNode = currentNode.GetNextNode(switchMode);
            moveDirection = transform.forward; // смотрим вперёд
        }
        else // Reverse
        {
            // Назад: к предыдущему узлу
            nextNode = currentNode.previous;
            moveDirection = -transform.forward; // едем задом, но смотрим вперёд
        }

        // --- Поворот к цели (только для вперёд!) ---
        // При реверсе: НЕ поворачиваем трамвай — водитель смотрит вперёд!
        if (currentDirection == TramDirection.Forward)
        {
            Vector3 targetDir = (currentNode.transform.position - transform.position).normalized;
            targetDir.y = 0;

            if (targetDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);
            }
        }
        // При реверсе: оставляем текущую ориентацию (как есть) — нет поворота!

        // --- Движение ---
        Vector3 flatMove = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
        rb.linearVelocity = flatMove * currentSpeed;

        // --- Проверка достижения узла ---
        float distanceToCurrent = Vector3.Distance(transform.position, currentNode.transform.position);

        // Достижено — переходим к следующему/предыдущему узлу
        if (distanceToCurrent < 5f)
        {
            if (nextNode != null)
            {
                // Обновляем индикатор (если нужен)
                // currentNode.UpdateIndicator(switchMode); // опционально

                currentNode = nextNode;
                Debug.Log($"Переход: {currentDirection} → {currentNode.nodeName}");
            }
            else
            {
                Debug.Log("Конец маршрута (нет следующего/предыдущего узла).");
                currentNode = null;
            }
        }
    }
}