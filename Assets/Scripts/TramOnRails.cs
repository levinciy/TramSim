

using UnityEngine;

public class TramOnRails : MonoBehaviour
{

//     [Header("Управление скоростью (целевая скорость)")]
//     public float speedChangeRate = 1.5f;   // Изменение скорости в секунду при удержании W/S
//     public float emergencyBrakePower = 6f; // Мощность экстренного тормоза (с инерцией)

//     private bool isEmergencyBraking = false;

//     [Header("Физика")]
//     public float maxSpeed = 15f;
//     public float acceleration = 2f;        // Как быстро разгоняемся до targetSpeed
//     public float brakePower = 4f;          // Как быстро тормозим
//     public float passiveDeceleration = 0.5f; // Естественное замедление

//     // [Header("Управление скоростью (целевая скорость)")]
//     // public float speedStep = 2f;           // На сколько меняется targetSpeed за нажатие
//     // public float emergencyBrakePower = 8f; // Мощность экстренного тормоза

//     [Header("Маршрут")]
//     public RouteNode startNode;
//     private RouteNode currentNode;

//     [Header("Управление")]
//     public SwitchMode switchMode = SwitchMode.Neutral;
//     public TramDirection currentDirection = TramDirection.Forward;

//     private Rigidbody rb;
//     private float currentSpeed = 0f;
//     private float targetSpeed = 0f;        // 🔑 НОВАЯ ПЕРЕМЕННАЯ: целевая скорость
    
//     private bool isChangingDirection = false;
//     private TramDirection requestedDirection;

//     // Для зон остановки
//     private bool isInStopZone = false;
//     private float stopZoneEndDistance = 0f;
//     private string currentStopName = "";
//     private bool hasStoppedInZone = false;
//     private float totalDistanceTraveled = 0f;
//     private Vector3 lastPosition;

//     public DriverEvaluator evaluator;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.freezeRotation = true;
//         rb.useGravity = false;
        
//         currentNode = startNode;
//         lastPosition = transform.position;
        
//         InvokeRepeating(nameof(CheckSpeedLimit), 1f, 1f);

//         if (trafficLights != null)
//         {
//             foreach (var light in trafficLights)
//             {
//                 approachingOnRed[light] = false;
//                 hasCrossedOnRed[light] = false;
//             }
//         }
//     }

//     void Update()
//     {
//         // === 1. Переключение направления (реверс) ===
//         if (Input.GetKeyDown(KeyCode.Q) && currentDirection != TramDirection.Forward)
//         {
//             requestedDirection = TramDirection.Forward;
//             isChangingDirection = true;
//         }
//         if (Input.GetKeyDown(KeyCode.E) && currentDirection != TramDirection.Reverse)
//         {
//             requestedDirection = TramDirection.Reverse;
//             isChangingDirection = true;
//         }

//         // === 2. Управление целевой скоростью ===
//         HandleSpeedInput();

//         // === 3. Применение направления (когда скорость ~0) ===
//         if (isChangingDirection && currentSpeed <= 0.1f)
//         {
//             currentDirection = requestedDirection;
//             isChangingDirection = false;
            
//             if (currentDirection == TramDirection.Reverse && currentNode.previous != null)
//             {
//                 RouteNode nextNode = currentNode.GetNextNode(switchMode);
//                 if (nextNode != null)
//                     currentNode = nextNode;
//             }
//             Debug.Log($"Направление: {currentDirection}, Узел: {currentNode.nodeName}");
//         }

//         // === 4. Управление стрелками ===
//         if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
//         if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
//         if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;
//     }

//     // === НОВАЯ МЕТОДИКА: обработка ввода скорости ===
//     private void HandleSpeedInput()
// {
//     // === Экстренный тормоз (удержание) ===
//     if (Input.GetKey(KeyCode.Space))
//     {
//         isEmergencyBraking = true;
//         targetSpeed = 0f; // Цель — остановка
//         return;
//     }
//     else
//     {
//         isEmergencyBraking = false;
//     }

//     // === Плавное изменение целевой скорости при удержании ===
//     if (Input.GetKey(KeyCode.W))
//     {
//         // Увеличиваем целевую скорость плавно
//         targetSpeed += speedChangeRate * Time.deltaTime;
//         targetSpeed = Mathf.Min(targetSpeed, maxSpeed); // не превышаем макс
//     }
    
//     if (Input.GetKey(KeyCode.S))
//     {
//         // Уменьшаем целевую скорость плавно
//         targetSpeed -= speedChangeRate * Time.deltaTime;
//         targetSpeed = Mathf.Max(targetSpeed, 0f); // не уходим в минус
//     }
// }

//     void FixedUpdate()
//     {
//         // --- Подсчёт пройденного расстояния (для остановок) ---
//         float segment = Vector3.Distance(transform.position, lastPosition);
//         totalDistanceTraveled += segment;
//         lastPosition = transform.position;

//         // === 5. Плавное стремление к целевой скорости ===
//         AdjustSpeedToTarget();

//         // === 6. Ограничение скорости ===
//         currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

//          CheckTrafficLightViolations();

//         // === 7. Определяем целевой узел ===
//         RouteNode targetNode = null;
//         Vector3 railDirection = Vector3.zero;

//         if (currentDirection == TramDirection.Forward)
//         {
//             targetNode = currentNode.GetNextNode(switchMode);
//             if (targetNode != null)
//                 railDirection = targetNode.transform.position - currentNode.transform.position;
//         }
//         else // Reverse
//         {
//             targetNode = currentNode.previous;
//             if (targetNode != null)
//                 railDirection = currentNode.transform.position - targetNode.transform.position;
//         }

//         if (targetNode == null)
//         {
//             rb.linearVelocity = Vector3.zero;
//             return;
//         }

//         // === 8. Направление к цели ===
//         Vector3 toTarget = targetNode.transform.position - transform.position;
//         toTarget.y = 0;
//         if (toTarget == Vector3.zero) return;

//         // === 9. Поворот модели (под одну кабину) ===
//         Vector3 desiredForward = (currentDirection == TramDirection.Forward) 
//             ? toTarget.normalized
//             : -toTarget.normalized;

//         Quaternion targetRot = Quaternion.LookRotation(desiredForward);
//         transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);

//         // === 10. Движение ===
//         float directionMultiplier = (currentDirection == TramDirection.Forward) ? 1f : -1f;
//         rb.linearVelocity = transform.forward * currentSpeed * directionMultiplier;

//         // === 11. Проверка достижения узла ===
//         float distanceToTarget = Vector3.Distance(transform.position, targetNode.transform.position);

//         if (distanceToTarget < 5f)
//         {
//             currentNode = targetNode;
//             Debug.Log($"✓ Достигнут: {currentNode.nodeName} ({currentDirection})");

//             if (currentDirection == TramDirection.Forward && currentNode.isStopNode)
//             {
//                 isInStopZone = true;
//                 stopZoneEndDistance = totalDistanceTraveled + currentNode.stopZoneLength;
//                 currentStopName = currentNode.stopName;
//                 hasStoppedInZone = false;
//             }
//         }

//         // === 12. Выход из зоны остановки ===
//         if (isInStopZone && totalDistanceTraveled >= stopZoneEndDistance)
//         {
//             if (!hasStoppedInZone)
//                 evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
            
//             isInStopZone = false;
//             currentStopName = "";
//             hasStoppedInZone = false;
//         }

//         // === 13. Фиксация остановки в зоне ===
//         if (isInStopZone && currentSpeed <= 0.5f)
//         {
//             hasStoppedInZone = true;
//         }
//     }

//     // === НОВАЯ МЕТОДИКА: плавная подстройка скорости к целевой ===
//     private void AdjustSpeedToTarget()
//     {
//         // === Экстренное торможение (с инерцией) ===
//         if (isEmergencyBraking)
//         {
//             // Сильное, но плавное торможение
//             currentSpeed -= emergencyBrakePower * Time.fixedDeltaTime;
//             currentSpeed = Mathf.Max(currentSpeed, 0f); // не уходим в минус
//             return;
//         }

//         // === Обычное стремление к целевой скорости ===
//         if (currentSpeed < targetSpeed)
//         {
//             // Разгон: плавно увеличиваем скорость
//             currentSpeed += acceleration * Time.fixedDeltaTime;
//             currentSpeed = Mathf.Min(currentSpeed, targetSpeed);
//         }
//         else if (currentSpeed > targetSpeed)
//         {
//             // Торможение: плавно уменьшаем скорость
//             currentSpeed -= brakePower * Time.fixedDeltaTime;
//             currentSpeed = Mathf.Max(currentSpeed, targetSpeed);
//         }
//         // Если currentSpeed == targetSpeed — держим скорость (инерция уже учтена в brakePower/acceleration)
//     }

//         private void CheckSpeedLimit()
//         {
//             if (currentNode == null || evaluator == null) return;
//             if (currentNode.hasSpeedLimit && currentSpeed > currentNode.maxSpeed + 0.5f)
//             {
//                 evaluator.addPenalty($"Превышение скорости: {currentSpeed:F1} > {currentNode.maxSpeed:F1}");
//             }
//         }

 [Header("Физика движения")]
    public float maxSpeed = 15f;
    
    // Ускорение (когда рычаг вперёд)
    [Range(0f, 5f)] public float acceleration = 2.5f;
    
    // Торможение (когда рычаг назад, но не до упора)
    [Range(0f, 6f)] public float serviceBrakePower = 3.5f;
    
    // Экстренное торможение (рычаг полностью назад)
    [Range(0f, 10f)] public float emergencyBrakePower = 8f;
    
    // Выкат в нейтрали (ОЧЕНЬ медленное замедление)
    [Range(0f, 0.5f)] public float coastingDeceleration = 0.15f;
    
    // Сопротивление качению (минимальное, чтобы трамвай не катился вечно)
    [Range(0f, 0.2f)] public float rollingResistance = 0.05f;

    [Header("Управление рычагом")]
    public float leverSensitivity = 3f;        // Скорость движения рычага
    public float leverReturnSpeed = 2f;        // Возврат в нейтраль (если нужно)
    public float deadZone = 0.05f;             // Мёртвая зона вокруг нейтрали
    
    [Header("Пороги")]
    public float brakeThreshold = -0.1f;       // Когда начинается торможение
    public float emergencyThreshold = -0.8f;   // Когда начинается экстренный тормоз
    public float powerThreshold = 0.1f;        // Когда начинается ускорение

    [Header("Маршрут")]
    public RouteNode startNode;
    private RouteNode currentNode;

    [Header("Управление")]
    public SwitchMode switchMode = SwitchMode.Neutral;
    public TramDirection currentDirection = TramDirection.Forward;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    
    // Позиция рычага: -1 (полный тормоз) ... 0 (нейтраль) ... +1 (полная тяга)
    [HideInInspector] public float leverPosition = 0f;
    
    private bool isChangingDirection = false;
    private TramDirection requestedDirection;

    // Для зон остановки
    private bool isInStopZone = false;
    private float stopZoneEndDistance = 0f;
    private string currentStopName = "";
    private bool hasStoppedInZone = false;
    private float totalDistanceTraveled = 0f;
    private Vector3 lastPosition;

    public DriverEvaluator evaluator;

    public RotarySwitch arrowSwitch;
    public RotarySwitch gearSwitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        
        currentNode = startNode;
        lastPosition = transform.position;
        
        InvokeRepeating(nameof(CheckSpeedLimit), 1f, 1f);


        if (arrowSwitch != null)
        {
            arrowSwitch.OnArrowsChanged += (mode) => {
                switchMode = mode;
                Debug.Log($"🚋 Стрелки переключены: {mode}");
            };
        }
        
        if (gearSwitch != null)
        {
            gearSwitch.OnGearChanged += (direction) => {
                // Для передачи нужна особая логика с остановкой
                if (currentSpeed <= 0.1f)
                {
                    currentDirection = direction;
                    Debug.Log($"🚋 Передача: {direction}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Нельзя менять передачу на ходу!");
                    // Вернуть переключатель в исходное положение
                    if (gearSwitch != null)
                    {
                        int currentPos = (currentDirection == TramDirection.Forward) ? 1 : 0;
                        gearSwitch.SetPosition(currentPos);
                    }
                }
            };
        }


        if (trafficLights != null)
        {
            approachingOnRed = new System.Collections.Generic.Dictionary<TrafficLight, bool>();
            hasCrossedOnRed = new System.Collections.Generic.Dictionary<TrafficLight, bool>();
            
            foreach (var light in trafficLights)
            {
                approachingOnRed[light] = false;
                hasCrossedOnRed[light] = false;
            }
        }


    }

    void Update()
    {
        // === 1. Переключение направления ===
        if (Input.GetKeyDown(KeyCode.Q) && currentDirection != TramDirection.Forward)
        {
            requestedDirection = TramDirection.Forward;
            isChangingDirection = true;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentDirection != TramDirection.Reverse)
        {
            requestedDirection = TramDirection.Reverse;
            isChangingDirection = true;
        }

        // === 2. Управление рычагом (НОВАЯ ЛОГИКА) ===
        HandleLeverInput();

        // === 3. Применение направления ===
        if (isChangingDirection && currentSpeed <= 0.1f)
        {
            currentDirection = requestedDirection;
            isChangingDirection = false;
            
            if (currentDirection == TramDirection.Reverse && currentNode.previous != null)
            {
                RouteNode nextNode = currentNode.GetNextNode(switchMode);
                if (nextNode != null)
                    currentNode = nextNode;
            }
        }

        // === 4. Стрелки ===
        // if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
        // if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
        // if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;
    }

    // === НОВАЯ ЛОГИКА: управление рычагом ===
    private void HandleLeverInput()
    {
        // Кнопка 1: Рычаг вперёд (ускорение)
        if (Input.GetKey(KeyCode.W))
        {
            leverPosition += leverSensitivity * Time.deltaTime;
        }
        
        // Кнопка 2: Рычаг назад (торможение)
        if (Input.GetKey(KeyCode.S))
        {
            leverPosition -= leverSensitivity * Time.deltaTime;
        }
        
        // Кнопка 3: Возврат в нейтраль
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            leverPosition = 0f;
        }
        
        // Кнопка 4: Полный назад (экстренный тормоз)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leverPosition = -1f;
        }

        // Ограничиваем диапазон [-1, 1]
        leverPosition = Mathf.Clamp(leverPosition, -1f, 1f);
    }

    void FixedUpdate()
    {
        // --- Подсчёт расстояния ---
        float segment = Vector3.Distance(transform.position, lastPosition);
        totalDistanceTraveled += segment;
        lastPosition = transform.position;

        // === 5. Применение физики на основе положения рычага ===
        ApplyPhysics();

        // === 6. Ограничение скорости ===
        currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

         CheckTrafficLightViolations();

        // === 7-13. Остальная логика движения (маршрут, повороты, остановки) ===
        // ... (оставь свой существующий код без изменений) ...
        
        // Определяем целевой узел
        RouteNode targetNode = null;
        Vector3 railDirection = Vector3.zero;

        if (currentDirection == TramDirection.Forward)
        {
            targetNode = currentNode.GetNextNode(switchMode);
            if (targetNode != null)
                railDirection = targetNode.transform.position - currentNode.transform.position;
        }
        else
        {
            targetNode = currentNode.previous;
            if (targetNode != null)
                railDirection = currentNode.transform.position - targetNode.transform.position;
        }

        if (targetNode == null)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 toTarget = targetNode.transform.position - transform.position;
        toTarget.y = 0;
        if (toTarget == Vector3.zero) return;

        Vector3 desiredForward = (currentDirection == TramDirection.Forward) 
            ? toTarget.normalized
            : -toTarget.normalized;

        Quaternion targetRot = Quaternion.LookRotation(desiredForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);

        float directionMultiplier = (currentDirection == TramDirection.Forward) ? 1f : -1f;
        rb.linearVelocity = transform.forward * currentSpeed * directionMultiplier;

        float distanceToTarget = Vector3.Distance(transform.position, targetNode.transform.position);

        if (distanceToTarget < 5f)
        {
            currentNode = targetNode;
            CheckRouteChoice(currentNode);
            if (currentDirection == TramDirection.Forward && currentNode.isStopNode)
            {
                isInStopZone = true;
                stopZoneEndDistance = totalDistanceTraveled + currentNode.stopZoneLength;
                currentStopName = currentNode.stopName;
                hasStoppedInZone = false;
            }
        }

        if (isInStopZone && totalDistanceTraveled >= stopZoneEndDistance)
        {
            if (!hasStoppedInZone)
                evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
            
            isInStopZone = false;
            currentStopName = "";
            hasStoppedInZone = false;
        }

        if (isInStopZone && currentSpeed <= 0.5f)
        {
            hasStoppedInZone = true;
        }

        
    }

    // === НОВАЯ ФИЗИКА: расчёт ускорения/торможения ===
    private void ApplyPhysics()
    {
        // Мёртвая зона вокруг нейтрали
        if (Mathf.Abs(leverPosition) < deadZone)
        {
            leverPosition = 0f;
        }

        if (leverPosition > powerThreshold)
        {
            // === УСКОРЕНИЕ (рычаг вперёд) ===
            // Чем дальше от нейтрали, тем сильнее ускорение
            float powerLevel = Mathf.InverseLerp(powerThreshold, 1f, leverPosition);
            float effectiveAcceleration = acceleration * powerLevel;
            
            currentSpeed += effectiveAcceleration * Time.fixedDeltaTime;
        }
        else if (leverPosition < brakeThreshold)
        {
            // === ТОРМОЖЕНИЕ (рычаг назад) ===
            if (leverPosition <= emergencyThreshold)
            {
                // Экстренный тормоз (полный назад)
                currentSpeed -= emergencyBrakePower * Time.fixedDeltaTime;
            }
            else
            {
                // Сервисное торможение (прогрессивное)
                float brakeLevel = Mathf.InverseLerp(brakeThreshold, emergencyThreshold, leverPosition);
                float effectiveBrake = serviceBrakePower * (1f - brakeLevel);
                
                currentSpeed -= effectiveBrake * Time.fixedDeltaTime;
            }
            
            currentSpeed = Mathf.Max(0f, currentSpeed);
        }
        else
        {
            // === НЕЙТРАЛЬ (выкат) ===
            // Очень медленное естественное замедление
            currentSpeed -= coastingDeceleration * Time.fixedDeltaTime;
            
            // Дополнительное сопротивление качению (чтобы не катился вечно)
            if (currentSpeed > 0.1f)
            {
                currentSpeed -= rollingResistance * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed = 0f;
            }
            
            currentSpeed = Mathf.Max(0f, currentSpeed);
        }
    }

    private void CheckSpeedLimit()
    {
        if (currentNode == null || evaluator == null) return;
        if (currentNode.hasSpeedLimit && currentSpeed > currentNode.maxSpeed + 0.5f)
        {
            evaluator.addPenalty($"Превышение скорости: {currentSpeed:F1} > {currentNode.maxSpeed:F1}");
        }
    }

    // Публичные методы
    public float GetLeverPosition() => leverPosition;
    public float GetCurrentSpeed() => currentSpeed;

        // === Публичные методы для UI ===
        // public float GetCurrentSpeed() => currentSpeed;
        // public float GetTargetSpeed() => targetSpeed;
        // public void SetTargetSpeed(float value) => targetSpeed = Mathf.Clamp(value, 0f, maxSpeed);

    //     [Header("Анимация рычага")]
    //     public Transform leverPivot; // Перетащи сюда объект LeverPivot из иерархии
    //     public float maxLeverAngle = 45f; // Насколько сильно отклоняется рычаг (в градусах)

    //     void LateUpdate()
    //     {
    //         UpdateLeverVisuals();
    //     }

    //     void UpdateLeverVisuals()
    //     {
    //         if (leverPivot == null) return;

    //         // 1. Вычисляем нормализованную скорость (от 0 до 1)
    //         //float targetSpeed = GetCurrentSpeed();
    //         float normalizedSpeed = targetSpeed / maxSpeed;

    //         // 2. Считаем угол: если скорость 0, угол 0. Если макс, угол maxLeverAngle
    //         float angle = normalizedSpeed * maxLeverAngle;

    //         // 3. Вращаем ПУСТОЙ ОБЪЕКТ (Pivot), а не сам рычаг!
    //         // Предполагаем, что рычаг отклоняется вперёд (по оси X).
    //         // Если вращается не туда, поменяй X на Z или убери минус.
    //         leverPivot.localEulerAngles = new Vector3(-angle, 0, 0); 
    // }
     [Header("Визуал рычага")]
    public Transform leverPivot;          // Пустой объект-точка вращения
    public float forwardAngle = -45f;     // Угол при полной тяге (leverPosition = +1.0)
    public float backwardAngle = 45f;     // Угол при полном тормозе (leverPosition = -1.0)

    // Вызывай это в LateUpdate, чтобы визуал обновлялся после физики
    void LateUpdate()
    {
        UpdateLeverVisuals();
    }

    void UpdateLeverVisuals()
    {
        if (leverPivot == null) return;

        // leverPosition: от -1.0 (тормоз) до +1.0 (тяга)
        // Нормализуем в диапазон [0, 1] для Lerp:
        float t = (leverPosition + 1f) * 0.5f;
        
        // Интерполируем угол между тормозом и тягой
        float currentAngle = Mathf.Lerp(backwardAngle, forwardAngle, t);

        // Применяем вращение (обычно по локальной оси X)
        leverPivot.localEulerAngles = new Vector3(currentAngle, 0, 0);
    }

    [Header("Светофоры")]
    public TrafficLight[] trafficLights;

    // Словари для отслеживания состояния пересечения каждого светофора
    private System.Collections.Generic.Dictionary<TrafficLight, bool> approachingOnRed = new();
    private System.Collections.Generic.Dictionary<TrafficLight, bool> hasCrossedOnRed = new();

    void CheckTrafficLightViolations()
    {
        if (trafficLights == null) return;

        Vector3 moveDir = (currentDirection == TramDirection.Forward) ? transform.forward : -transform.forward;

        foreach (var light in trafficLights)
        {
            if (!approachingOnRed.ContainsKey(light)) continue;

            Vector3 toStopLine = light.StopLinePosition - transform.position;
            float distance = toStopLine.magnitude;
            bool isAhead = Vector3.Dot(moveDir, toStopLine.normalized) > 0; // Линия впереди?

            //Debug.Log($"🚦 [{light.name}] Дист: {distance:F2}м | Впереди: {isAhead} | Красный: {light.IsRed} | Подъехал: {approachingOnRed[light]} | Проехал: {hasCrossedOnRed[light]}");

            // 1. Сброс флагов: если зелёный или мы уехали далеко
            if (!light.IsRed || distance > 15f)
            {

                // if (approachingOnRed[light] || hasCrossedOnRed[light])
                //     Debug.Log($"  🟢 Сброс флагов для {light.name}");
                approachingOnRed[light] = false;
                hasCrossedOnRed[light] = false;
            }

            // 2. Фиксируем подъезд на красный
            if (isAhead && distance < 8f && light.IsRed)
            {
                // if (!approachingOnRed[light])
                //     Debug.Log($"  🔴 Зафиксирован подъезд на красный: {light.name}");
                //Debug.Log("approachingOnRed[light] = true;");
                approachingOnRed[light] = true;
            }

            // 3. Фиксируем пересечение: если линия оказалась позади, 
            //    но мы подъезжали к ней на красный
            if (!isAhead && approachingOnRed[light] && !hasCrossedOnRed[light])
            {
                hasCrossedOnRed[light] = true;
                evaluator?.addPenalty($"Проезд на красный: {light.name}");
                Debug.Log($"🚨 Нарушение: Проезд на красный ({light.name})");
            }
        }
    }

    public RouteNode GetTargetNode()
    {
        // Логика дублирует то, что у тебя в FixedUpdate, 
        // но возвращает объект сразу для использования извне
        if (currentDirection == TramDirection.Forward)
        {
            return currentNode.GetNextNode(switchMode);
        }
        else
        {
            return currentNode.previous;
        }
    }

    public int currentRouteId = 3;


    public RouteNode GetGuidanceTarget()
    {
        if (currentNode == null) return null;
        if (currentDirection == TramDirection.Reverse) return currentNode.previous;


        RouteNode[] exits = new RouteNode[] 
        { 
            currentNode.defaultNext, 
            currentNode.rightNext, 
            currentNode.leftNext 
        };


        foreach (var exit in exits)
        {
            if (exit != null && (exit.routeId == 0 || exit.routeId == currentRouteId))
            {
                return exit; 
            }
        }

        return currentNode.GetNextNode(switchMode);
    }

    public bool IsCorrectRoute(RouteNode node)
    {
        if (node == null) return false;
        
        return node.routeId == 0 || node.routeId == currentRouteId;
    }


    public void CheckRouteChoice(RouteNode chosenNode)
    {
        if (chosenNode != null && !IsCorrectRoute(chosenNode))
        {
            Debug.LogWarning($" Выбран неправильный маршрут! Узел: {chosenNode.name}");
            evaluator?.addPenalty($"Неверный маршрут: {chosenNode.nodeName}");
        }
    }

    //public enum TurnSignalState { Left, Neutral, Right }
    public TurnSignalState turnSignal = TurnSignalState.Neutral;

}