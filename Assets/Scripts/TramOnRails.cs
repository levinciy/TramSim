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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        currentNode = startNode;
    }

    void Update()
    {
        // Управление скоростью
        bool isAccelerating = Input.GetKey(KeyCode.W);
        bool isBraking = Input.GetKey(KeyCode.S);

        if (isAccelerating)
            currentSpeed += acceleration * Time.deltaTime;
        else if (isBraking)
            currentSpeed -= brakePower * Time.deltaTime;
        else
            currentSpeed -= passiveDeceleration * Time.deltaTime;

        currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

        // Управление режимом стрелок (временно)
        if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
        if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
        if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;
    }

    void FixedUpdate()
    {
        if (currentNode == null || currentSpeed <= 0.01f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Движение к текущему узлу
        Vector3 direction = (currentNode.transform.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 8f);
        }

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        rb.linearVelocity = forward * currentSpeed;

        // Проверка достижения узла
        if (Vector3.Distance(transform.position, currentNode.transform.position) < 5f)
        {
            RouteNode nextNode = currentNode.GetNextNode(switchMode);
            if (nextNode != null)
            {
                //currentNode.UpdateIndicator(switchMode);
                currentNode = nextNode;
                Debug.Log($"Перешёл к: {currentNode.nodeName} (режим: {switchMode})");
            }
            else
            {
                Debug.Log("Конец маршрута.");
                currentNode = null;
            }
        }
    }
}