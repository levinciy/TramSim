// using UnityEngine;

// public class TramOnRails : MonoBehaviour
// {

//     public DriverEvaluator evaluator ;

//     // [Header("Звук")]
//     // public AudioClip hornSound;
//     // private AudioSource audioSource;

//     private bool isInStopZone = false;
//     private float stopZoneStartDistance = 0f; // расстояние от начала маршрута до входа в зону
//     private float stopZoneEndDistance = 0f;   // ... до конца зоны
//     private string currentStopName = "";
//     private bool hasStoppedInZone = false;

//     [Header("Физика")]
//     public float maxSpeed = 15f;
//     public float acceleration = 1.5f;
//     public float brakePower = 3f;
//     public float passiveDeceleration = 0.8f;
//     private TramDirection requestedDirection; 
//     private bool isChangingDirection = false;

//     [Header("Маршрут")]
//     public RouteNode startNode; // задаётся в инспекторе
//     private RouteNode currentNode;
//     private RouteNode lastPassedNode;

//     [Header("Управление")]
//     public SwitchMode switchMode = SwitchMode.Neutral; 

//     private Rigidbody rb;
//     private float currentSpeed = 0f;

//     public TramDirection currentDirection = TramDirection.Forward;

//     private float totalDistanceTraveled = 0f;
//     private Vector3 lastPosition;

//     void Start(){
//         rb = GetComponent<Rigidbody>();
//         rb.freezeRotation = true;
//         rb.useGravity = false;
//         currentNode = startNode;
//         lastPassedNode = startNode;
//         //audioSource = GetComponent<AudioSource>();
//         //hornSound = audioSource.clip;
//         lastPosition = transform.position;
//         InvokeRepeating(nameof(CheckSpeedLimit), 1f, 1f);
//     }

//     void Update(){
//         if (Input.GetKeyDown(KeyCode.Q)){
//             if (currentDirection != TramDirection.Forward)
//             {
//                 requestedDirection = TramDirection.Forward;
//                 isChangingDirection = true;
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.E)){
//             if (currentDirection != TramDirection.Reverse)
//             {
//                 requestedDirection = TramDirection.Reverse;
//                 isChangingDirection = true;
//             }
//         }

//     // Управление скоростью
//         bool isAccelerating = Input.GetKey(KeyCode.W);
//         bool isBraking = Input.GetKey(KeyCode.S);

//         bool canAccelerate = !isChangingDirection;

//         if (isAccelerating && canAccelerate)
//         {
//             currentSpeed += acceleration * Time.deltaTime;
//         }
//         else if (isBraking)
//         {
//             currentSpeed -= brakePower * Time.deltaTime;
//         }
//         else
//         {
//             currentSpeed -= passiveDeceleration * Time.deltaTime;
//         }

//         // Ограничение скорости
//         currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

//         // Когда скорость достигла нуля — применяем новое направление
//         if (isChangingDirection && currentSpeed <= 0.1f)
//         {
//             currentDirection = requestedDirection;
//             isChangingDirection = false;
//             Debug.Log($"Направление изменено на: {currentDirection}");
//         }

//         // Управление режимом стрелок (временно)
//         if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
//         if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
//         if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;


//         // if (Input.GetKeyDown(KeyCode.H)){
//         //     //Debug.Log("Нажата H!");
//         //     PlayHorn();
//         // }
//     }
//     //новые поля для остановки, вполне возмлжно придется менять
//     //private bool wasInStopZone = false;
//     // private bool isInStopZone = false;
//     // private string currentStopName = "";
//     // private bool hasStoppedInZone = false;

//     void FixedUpdate(){

//         float segment = Vector3.Distance(transform.position, lastPosition);
//         totalDistanceTraveled += segment;
//         lastPosition = transform.position;
//         if (currentNode == null || currentSpeed <= 0.01f)
//         {
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//             return;
//         }

//         //отсюда все до 145 строки новое, связанное с системой из 2 узлов, очерчивающих остановку, скорее всего придется дорабатывать

//         // if (!isInStopZone && currentNode.nodeType == RouteNode.NodeType.StopZoneStart)
//         // {
//         //     isInStopZone = true;
//         //     currentStopName = currentNode.nodeName;
//         //     hasStoppedInZone = false;
//         //     Debug.Log($"✅ Вход в зону остановки: {currentStopName}");
//         // }

//         // // --- 2. Отслеживание остановки внутри зоны ---
//         // if (isInStopZone && currentSpeed <= 0.5f)
//         // {
//         //     hasStoppedInZone = true;
//         //     Debug.Log($"✅ Остановка зафиксирована в зоне: {currentStopName}");
//         // }

//         // // --- 3. Выход из зоны ---
//         // if (isInStopZone && currentNode.nodeType == RouteNode.NodeType.StopZoneEnd)
//         // {
//         //     if (!hasStoppedInZone)
//         //     {
//         //         evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
//         //     }
//         //     else
//         //     {
//         //         Debug.Log($"✅ Остановка соблюдена: {currentStopName}");
//         //     }

//         //     // Сброс состояния
//         //     isInStopZone = false;
//         //     currentStopName = "";
//         //     hasStoppedInZone = false;
//         // }

//         // bool isInStopZone = false;
//         // if(currentNode.isStopZone ){
//         //     float distanceToZone = Vector3.Distance(transform.position, currentNode.transform.position);
//         //     if (distanceToZone<currentNode.stopZoneRadius){
//         //         isInStopZone = true;

//         //     }
            
//         // }

//         // if (isInStopZone && !wasInStopZone){
//         //     wasInStopZone = true;
//         //     currentStopName = currentNode.nodeName;
//         //     hasStoppedInZone = false;
//         //     Debug.Log($"✅ Вход в зону остановки: {currentStopName}");
//         // }

//         // if(wasInStopZone && currentSpeed<=0.5f){
//         //     hasStoppedInZone = true;
//         // }

//         // if (wasInStopZone && !isInStopZone){
//         //     if(!hasStoppedInZone){
//         //         evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
//         //     }
//         //     else
//         //     {
//         //         Debug.Log($"✅ Остановка соблюдена: {currentStopName}");
//         //     }

//         //     wasInStopZone = false;
//         //     hasStoppedInZone =false;
//         //     currentStopName = "";
//         // }

//         if (!isInStopZone && currentNode != null && currentNode.isStopNode)
//         {
//             float distanceToNode = Vector3.Distance(transform.position, currentNode.transform.position);
//             if (distanceToNode < 5f) // близко к узлу
//             {
//                 // Активируем зону
//                 isInStopZone = true;
//                 stopZoneStartDistance = totalDistanceTraveled;
//                 stopZoneEndDistance = totalDistanceTraveled + currentNode.stopZoneLength;
//                 currentStopName = currentNode.stopName;
//                 hasStoppedInZone = false;
//                 Debug.Log($"✅ Зона остановки активирована: {currentStopName} (длина: {currentNode.stopZoneLength} м)");
//             }
//         }

//         // Отслеживаем остановку в зоне
//         if (isInStopZone)
//         {
//             if (currentSpeed <= 0.5f)
//             {
//                 hasStoppedInZone = true;
//             }

//             // Выход из зоны
//             if (totalDistanceTraveled >= stopZoneEndDistance)
//             {
//                 if (!hasStoppedInZone)
//                 {
//                     evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
//                 }
//                 else
//                 {
//                     Debug.Log($"✅ Остановка соблюдена: {currentStopName}");
//                 }

//                 isInStopZone = false;
//                 currentStopName = "";
//                 hasStoppedInZone = false;
//             }
//         }

//         // --- Определяем направление движения ---
//         // Vector3 moveDirection;
//         // RouteNode nextNode = null;

//         // if (currentDirection == TramDirection.Forward)
//         // {
//         //     // Вперёд: к следующему узлу (по логике стрелок)
//         //     nextNode = currentNode.GetNextNode(switchMode);
//         //     moveDirection = transform.forward; // смотрим вперёд
//         // }
//         // else // Reverse
//         // {
//         //     // Назад: к предыдущему узлу
//         //     nextNode = currentNode.GetPrevious();
//         //     moveDirection = -transform.forward; // едем задом, но смотрим вперёд
//         // }
//         // Debug.Log(currentNode);
//         // // --- Поворот к цели (только для вперёд!) ---
//         // if (currentDirection == TramDirection.Forward)
//         // {
//         //     Vector3 targetDir = (currentNode.transform.position - transform.position).normalized;
//         //     targetDir.y = 0;

//         //     if (targetDir != Vector3.zero)
//         //     {
//         //         Quaternion targetRot = Quaternion.LookRotation(targetDir);
//         //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);
//         //     }
//         // }

//         // // --- Движение ---
//         // Vector3 flatMove = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
//         // rb.linearVelocity = flatMove * currentSpeed;
//         // //rb.AddForce(flatMove * currentSpeed,ForceMode.Force);

//         // // --- Проверка достижения узла ---
//         // float distanceToCurrent = Vector3.Distance(transform.position, currentNode.transform.position);

//         // // Достижено — переходим к следующему/предыдущему узлу
//         // if (distanceToCurrent < 2f)
//         // {

//         //     // if (currentNode.isStop)
//         //     // {
//         //     //     float speedThreshold = 0.5f; // м/с — считаем "остановкой", если скорость < 0.5
//         //     //     if (currentSpeed > speedThreshold)
//         //     //     {
//         //     //         evaluator?.addPenalty($"Пропуск остановки: {currentNode.nodeName}");
//         //     //     }
//         //     //     else
//         //     //     {
//         //     //         Debug.Log($"Остановка соблюдена: {currentNode.nodeName}");
//         //     //     }
//         //     // }
//         //     if (nextNode != null)
//         //     {
//         //         // currentNode.UpdateIndicator(switchMode); 

//         //         currentNode = nextNode;
//         //         Debug.Log($"Переход: {currentDirection} → {currentNode.nodeName}");
//         //     }
//         //     else
//         //     {
//         //         Debug.Log("Конец маршрута (нет следующего/предыдущего узла).");
//         //         currentNode = null;
//         //     }
//         // }

// // --- 1. Определяем целевой узел ---
// RouteNode targetNode = null;

// if (currentDirection == TramDirection.Forward)
// {
//     targetNode = currentNode.GetNextNode(switchMode);
// }
// else // Reverse
// {
//     targetNode = currentNode.previous;
// }

// if (targetNode == null)
// {
//     rb.linearVelocity = Vector3.zero;
//     return;
// }

// // --- 2. Направление движения: ВСЕГДА к целевому узлу ---
// Vector3 toTarget = targetNode.transform.position - transform.position;
// toTarget.y = 0; // игнорируем высоту
// Vector3 movementDirection = toTarget.normalized;

// // --- 3. Визуальный поворот: ТОЛЬКО при движении вперёд ---
// // При реверсе НЕ меняем вращение — камера остаётся стабильной
// if (currentDirection == TramDirection.Forward && toTarget != Vector3.zero)
// {
//     Quaternion targetRot = Quaternion.LookRotation(toTarget);
//     transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);
// }
// // Если едем назад — вращение не трогаем, но движение всё равно к targetNode!

// // --- 4. Применяем движение ---
// rb.linearVelocity = movementDirection * currentSpeed;

// // --- 5. Проверка достижения узла ---
// float distanceToTarget = Vector3.Distance(transform.position, targetNode.transform.position);
// if (distanceToTarget < 5f)
// {
//     currentNode = targetNode;
//     Debug.Log($"✓ Достигнут: {currentNode.nodeName} ({currentDirection})");
// }

        


        
//     }

//     private void CheckSpeedLimit()
//         {
            
//             if (currentNode == null || evaluator == null){
//                  return;}

//             if (currentNode.hasSpeedLimit && currentSpeed > currentNode.maxSpeed + 0.5f)
//             {
//                 string reason = $"Превышение скорости в '{currentNode.nodeName}': {currentSpeed:F1} > {currentNode.maxSpeed:F1} м/с";
//                 evaluator.addPenalty(reason);
//                 //evaluator.addSpeedingPenalty(currentSpeed, currentNode.maxSpeed, currentNode.nodeName);
                
//             }
//         }

//     // private void PlayHorn(){
   
//     //     audioSource.PlayOneShot(hornSound, 0.8f);
//     //     Debug.Log("Гудок воспроизведён!");
//     // }
// }

using UnityEngine;

public class TramOnRails : MonoBehaviour
{

    [Header("Управление скоростью (целевая скорость)")]
    public float speedChangeRate = 1.5f;   // Изменение скорости в секунду при удержании W/S
    public float emergencyBrakePower = 6f; // Мощность экстренного тормоза (с инерцией)

    private bool isEmergencyBraking = false;

    [Header("Физика")]
    public float maxSpeed = 15f;
    public float acceleration = 2f;        // Как быстро разгоняемся до targetSpeed
    public float brakePower = 4f;          // Как быстро тормозим
    public float passiveDeceleration = 0.5f; // Естественное замедление

    // [Header("Управление скоростью (целевая скорость)")]
    // public float speedStep = 2f;           // На сколько меняется targetSpeed за нажатие
    // public float emergencyBrakePower = 8f; // Мощность экстренного тормоза

    [Header("Маршрут")]
    public RouteNode startNode;
    private RouteNode currentNode;

    [Header("Управление")]
    public SwitchMode switchMode = SwitchMode.Neutral;
    public TramDirection currentDirection = TramDirection.Forward;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;        // 🔑 НОВАЯ ПЕРЕМЕННАЯ: целевая скорость
    
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        
        currentNode = startNode;
        lastPosition = transform.position;
        
        InvokeRepeating(nameof(CheckSpeedLimit), 1f, 1f);

        if (trafficLights != null)
        {
            foreach (var light in trafficLights)
            {
                approachingOnRed[light] = false;
                hasCrossedOnRed[light] = false;
            }
        }
    }

    void Update()
    {
        // === 1. Переключение направления (реверс) ===
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

        // === 2. Управление целевой скоростью ===
        HandleSpeedInput();

        // === 3. Применение направления (когда скорость ~0) ===
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
            Debug.Log($"Направление: {currentDirection}, Узел: {currentNode.nodeName}");
        }

        // === 4. Управление стрелками ===
        if (Input.GetKeyDown(KeyCode.F)) switchMode = SwitchMode.Neutral;
        if (Input.GetKeyDown(KeyCode.A)) switchMode = SwitchMode.Left;
        if (Input.GetKeyDown(KeyCode.D)) switchMode = SwitchMode.Right;
    }

    // === НОВАЯ МЕТОДИКА: обработка ввода скорости ===
    private void HandleSpeedInput()
{
    // === Экстренный тормоз (удержание) ===
    if (Input.GetKey(KeyCode.Space))
    {
        isEmergencyBraking = true;
        targetSpeed = 0f; // Цель — остановка
        return;
    }
    else
    {
        isEmergencyBraking = false;
    }

    // === Плавное изменение целевой скорости при удержании ===
    if (Input.GetKey(KeyCode.W))
    {
        // Увеличиваем целевую скорость плавно
        targetSpeed += speedChangeRate * Time.deltaTime;
        targetSpeed = Mathf.Min(targetSpeed, maxSpeed); // не превышаем макс
    }
    
    if (Input.GetKey(KeyCode.S))
    {
        // Уменьшаем целевую скорость плавно
        targetSpeed -= speedChangeRate * Time.deltaTime;
        targetSpeed = Mathf.Max(targetSpeed, 0f); // не уходим в минус
    }
}

    void FixedUpdate()
    {
        // --- Подсчёт пройденного расстояния (для остановок) ---
        float segment = Vector3.Distance(transform.position, lastPosition);
        totalDistanceTraveled += segment;
        lastPosition = transform.position;

        // === 5. Плавное стремление к целевой скорости ===
        AdjustSpeedToTarget();

        // === 6. Ограничение скорости ===
        currentSpeed = Mathf.Max(0f, Mathf.Min(currentSpeed, maxSpeed));

         CheckTrafficLightViolations();

        // === 7. Определяем целевой узел ===
        RouteNode targetNode = null;
        Vector3 railDirection = Vector3.zero;

        if (currentDirection == TramDirection.Forward)
        {
            targetNode = currentNode.GetNextNode(switchMode);
            if (targetNode != null)
                railDirection = targetNode.transform.position - currentNode.transform.position;
        }
        else // Reverse
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

        // === 8. Направление к цели ===
        Vector3 toTarget = targetNode.transform.position - transform.position;
        toTarget.y = 0;
        if (toTarget == Vector3.zero) return;

        // === 9. Поворот модели (под одну кабину) ===
        Vector3 desiredForward = (currentDirection == TramDirection.Forward) 
            ? toTarget.normalized
            : -toTarget.normalized;

        Quaternion targetRot = Quaternion.LookRotation(desiredForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 8f);

        // === 10. Движение ===
        float directionMultiplier = (currentDirection == TramDirection.Forward) ? 1f : -1f;
        rb.linearVelocity = transform.forward * currentSpeed * directionMultiplier;

        // === 11. Проверка достижения узла ===
        float distanceToTarget = Vector3.Distance(transform.position, targetNode.transform.position);

        if (distanceToTarget < 5f)
        {
            currentNode = targetNode;
            Debug.Log($"✓ Достигнут: {currentNode.nodeName} ({currentDirection})");

            if (currentDirection == TramDirection.Forward && currentNode.isStopNode)
            {
                isInStopZone = true;
                stopZoneEndDistance = totalDistanceTraveled + currentNode.stopZoneLength;
                currentStopName = currentNode.stopName;
                hasStoppedInZone = false;
            }
        }

        // === 12. Выход из зоны остановки ===
        if (isInStopZone && totalDistanceTraveled >= stopZoneEndDistance)
        {
            if (!hasStoppedInZone)
                evaluator?.addPenalty($"Пропуск остановки: {currentStopName}");
            
            isInStopZone = false;
            currentStopName = "";
            hasStoppedInZone = false;
        }

        // === 13. Фиксация остановки в зоне ===
        if (isInStopZone && currentSpeed <= 0.5f)
        {
            hasStoppedInZone = true;
        }
    }

    // === НОВАЯ МЕТОДИКА: плавная подстройка скорости к целевой ===
    private void AdjustSpeedToTarget()
    {
        // === Экстренное торможение (с инерцией) ===
        if (isEmergencyBraking)
        {
            // Сильное, но плавное торможение
            currentSpeed -= emergencyBrakePower * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0f); // не уходим в минус
            return;
        }

        // === Обычное стремление к целевой скорости ===
        if (currentSpeed < targetSpeed)
        {
            // Разгон: плавно увеличиваем скорость
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, targetSpeed);
        }
        else if (currentSpeed > targetSpeed)
        {
            // Торможение: плавно уменьшаем скорость
            currentSpeed -= brakePower * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, targetSpeed);
        }
        // Если currentSpeed == targetSpeed — держим скорость (инерция уже учтена в brakePower/acceleration)
    }

        private void CheckSpeedLimit()
        {
            if (currentNode == null || evaluator == null) return;
            if (currentNode.hasSpeedLimit && currentSpeed > currentNode.maxSpeed + 0.5f)
            {
                evaluator.addPenalty($"Превышение скорости: {currentSpeed:F1} > {currentNode.maxSpeed:F1}");
            }
        }

        // === Публичные методы для UI ===
        public float GetCurrentSpeed() => currentSpeed;
        public float GetTargetSpeed() => targetSpeed;
        public void SetTargetSpeed(float value) => targetSpeed = Mathf.Clamp(value, 0f, maxSpeed);

        [Header("Анимация рычага")]
        public Transform leverPivot; // Перетащи сюда объект LeverPivot из иерархии
        public float maxLeverAngle = 45f; // Насколько сильно отклоняется рычаг (в градусах)

        void LateUpdate()
        {
            UpdateLeverVisuals();
        }

        void UpdateLeverVisuals()
        {
            if (leverPivot == null) return;

            // 1. Вычисляем нормализованную скорость (от 0 до 1)
            float normalizedSpeed = targetSpeed / maxSpeed;

            // 2. Считаем угол: если скорость 0, угол 0. Если макс, угол maxLeverAngle
            float angle = normalizedSpeed * maxLeverAngle;

            // 3. Вращаем ПУСТОЙ ОБЪЕКТ (Pivot), а не сам рычаг!
            // Предполагаем, что рычаг отклоняется вперёд (по оси X).
            // Если вращается не туда, поменяй X на Z или убери минус.
            leverPivot.localEulerAngles = new Vector3(-angle, 0, 0); 
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

            // 1. Сброс флагов: если зелёный или мы уехали далеко
            if (!light.IsRed || distance > 15f)
            {
                approachingOnRed[light] = false;
                hasCrossedOnRed[light] = false;
            }

            // 2. Фиксируем подъезд на красный
            if (isAhead && distance < 8f && light.IsRed)
            {
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

}