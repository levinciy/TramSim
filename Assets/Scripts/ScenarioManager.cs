// using UnityEngine;


// public class ScenarioManager : MonoBehaviour
// {
//     [Header("🔗 Ссылки")]
//     public TramOnRails tramController;
//     public ScenarioUI scenarioUI;

//     [Header("📂 Сценарии")]
//     public ScenarioData[] scenarios;

//     private ScenarioData activeScenario;
//     private bool isRunning = false;
//     private float timer = 0f;


//     private CrossingCarController activeCar;
//     private bool carInDangerZone = false;
//     private bool scenarioEvaluated = false;
//     private Vector3 intersectionCenter;

    
//     void Start()
//     {

//         if (scenarios != null)
//         {
//             foreach (var sc in scenarios)
//             {
//                 sc.hasBeenTriggered = false;
//             }
//         }
        

//     }
    
//     void Update()
//     {
//         if (tramController == null || scenarios == null || scenarios.Length == 0) return;

//         if (!isRunning)
//         {
//             CheckActivation();
//         }
//         else
//         {
//             if (activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.TrafficLight)
//             {
//                 // Просто отсчитываем время до переключения
//                 timer -= Time.deltaTime;
//                 if (timer <= 0f)
//                 {
//                     TriggerTrafficLightScenario();
//                 }
//             }
//             else if(activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.CarCrossing){
//                 CheckCarScenario();
//             }
//             else if(activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.PedestrianCrossing){
//                 CheckPedestrianScenario();
//             }

//         }
//     }

//     void CheckActivation()
//     {
//         foreach (var sc in scenarios)
//         {

//             if (sc.hasBeenTriggered) continue;
//             if (string.IsNullOrEmpty(sc.targetNodeName)) continue;

//             GameObject nodeObj = GameObject.Find(sc.targetNodeName);
//             if (nodeObj == null) continue;

//             float dist = Vector3.Distance(tramController.transform.position, nodeObj.transform.position);
//             if (dist <= sc.activationDistance)
//             {
//                 // activeScenario = sc;
//                 // isRunning = true;
//                 // timer = sc.timeLimitSeconds; // Теперь это "время до красного"
//                 // Debug.Log($" Сценарий '{sc.scenarioName}' активирован. Красный через {sc.timeLimitSeconds} сек.");
//                 Activate(sc,nodeObj);
//                 break;
//             }
//         }
//     }

//     void TriggerTrafficLightScenario()
//     {
//         if (activeScenario == null) return;

//         // 🚦 Просто переключаем светофор на красный
//         if (activeScenario.changeTrafficLight && !string.IsNullOrEmpty(activeScenario.targetLightName))
//         {
//             GameObject lightObj = GameObject.Find(activeScenario.targetLightName);
//             TrafficLight light = lightObj?.GetComponent<TrafficLight>();
            
//             if (light != null)
//             {
//                 light.ForceSetLight(activeScenario.setToColor);
//                 if (scenarioUI != null)
//                 {
//                     scenarioUI.ShowScenarioNotification(
//                         activeScenario.scenarioName,
//                         $"Светофор переключён на {activeScenario.setToColor}",
//                         true // isSuccess = true (это не штраф, просто событие)
//                     );
                    
//                     // Скрываем подсказку через 2 секунды
//                     Invoke(nameof(HideHintDelayed), 2f);
//                 }
//                 //Debug.Log($"🚦 СЦЕНАРИЙ: Светофор '{activeScenario.targetLightName}' переключён на {activeScenario.setToColor}");
//             }
//         }

//         if (activeScenario.showHint)
//         {
//             Debug.Log($"💬 Подсказка: {activeScenario.hintText}");
//         }


//         isRunning = false;
//         activeScenario = null;
//     }

//     void Activate(ScenarioData data, GameObject nodeObj)
//     {

//         data.hasBeenTriggered = true;
//         activeScenario = data;
//         isRunning = true;
//         //timer = data.timeLimitSeconds;
//         if (data.type == ScenarioData.ScenarioType.TrafficLight)
//         {
//             timer = data.timeLimitSeconds;
//         }
//         else if (data.type == ScenarioData.ScenarioType.CarCrossing)
//         {
//             timer = data.activationDelay;
            
//             // Находим центр перекрёстка (середина массива точек)
//             if (data.carWaypointNames != null && data.carWaypointNames.Length > 0)
//             {
//                 GameObject centerWp = GameObject.Find(data.carWaypointNames[data.carWaypointNames.Length / 2]);
//                 if (centerWp != null)
//                     intersectionCenter = centerWp.transform.position;
//             }
//         }
//         else if (data.type == ScenarioData.ScenarioType.PedestrianCrossing)
//         {
//             timer = data.activationDelay;
            
//             // Находим центр перекрёстка (середина массива точек)
//             if (data.pedestrianWaypointNames != null && data.pedestrianWaypointNames.Length > 0)
//             {
//                 GameObject centerWp = GameObject.Find(data.pedestrianWaypointNames[data.pedestrianWaypointNames.Length / 2]);
//                 if (centerWp != null)
//                     intersectionCenter = centerWp.transform.position;
//             }
//         }


//         if (data.showHint && scenarioUI != null)
//         {
//             scenarioUI.ShowHint(data.hintText);
//         }

//         Debug.Log($"⏱ Сценарий '{data.scenarioName}' активирован. Красный через {data.timeLimitSeconds} сек.");
//     }

//     void HideHintDelayed()
//     {
//         if (scenarioUI != null)
//             scenarioUI.HideHint();
//     }


//     void CheckCarScenario()
//     {
//         timer -= Time.deltaTime;
        
//         // Задержка истекла — запускаем машину
//         if (timer <= 0f && activeCar == null)
//         {
//             ActivateCar();
//         }
        
//         // Машина активна — проверяем опасную зону
//         if (activeCar != null && !scenarioEvaluated)
//         {
//             activeCar.CheckDangerZone(intersectionCenter, activeScenario.dangerZoneRadius);
            
//             if (carInDangerZone)
//             {
//                 float tramSpeed = tramController.GetCurrentSpeed();
//                 if (tramSpeed > activeScenario.safeSpeedThreshold)
//                 {
//                     EvaluateCarScenario(false, $"Скорость {tramSpeed * 3.6f:F0} км/ч — нужно было притормозить!");
//                 }
//                 else
//                 {
//                     EvaluateCarScenario(true, "Скорость снижена. Безопасный проезд.");
//                 }
//             }
//         }
//     }

//     void ActivateCar()
//     {
//         if (string.IsNullOrEmpty(activeScenario.carObjectName))
//         {
//             Debug.LogError("⚠️ Не указано имя машины в сценарии!");
//             CleanupScenario();
//             return;
//         }

//         // ✅ Ищем машину, которая УЖЕ в сцене (не спавним!)
//         GameObject carObj = GameObject.Find(activeScenario.carObjectName);
//         if (carObj == null)
//         {
//             Debug.LogError($"⚠️ Машина '{activeScenario.carObjectName}' не найдена в сцене!");
//             CleanupScenario();
//             return;
//         }

//         activeCar = carObj.GetComponent<CrossingCarController>();
        
//         if (activeCar == null)
//         {
//             Debug.LogError($"⚠️ У объекта '{activeScenario.carObjectName}' нет компонента CrossingCarController!");
//             CleanupScenario();
//             return;
//         }

//         // ✅ Находим точки пути
//         Transform[] pathTransforms = new Transform[activeScenario.carWaypointNames.Length];
//         bool allFound = true;
//         for (int i = 0; i < activeScenario.carWaypointNames.Length; i++)
//         {
//             GameObject wp = GameObject.Find(activeScenario.carWaypointNames[i]);
//             if (wp != null)
//             {
//                 pathTransforms[i] = wp.transform;
//             }
//             else
//             {
//                 Debug.LogError($"⚠️ Точка пути не найдена: {activeScenario.carWaypointNames[i]}");
//                 allFound = false;
//             }
//         }
        
//         if (!allFound)
//         {
//             Debug.LogError("⚠️ Не все точки пути найдены — машина не поедет!");
//             CleanupScenario();
//             return;
//         }

//         // ✅ Настраиваем и запускаем машину
//         activeCar.Setup(pathTransforms, activeScenario.carSpeed);
        
//         activeCar.OnEnterDangerZone += () => carInDangerZone = true;
//         activeCar.OnExitDangerZone += () => {
//             if (!scenarioEvaluated)
//                 EvaluateCarScenario(true, "Машина проехала, помех не создано.");
//         };
//         activeCar.OnCrossingComplete += () => {
//             if (!scenarioEvaluated)
//                 EvaluateCarScenario(true, "Машина завершила проезд.");
//         };
        
//         activeCar.StartCrossing();
        
//         Debug.Log($"🚗 Машина '{activeScenario.carObjectName}' начала движение");
//     }

//     void EvaluateCarScenario(bool success, string message)
//     {
//         if (scenarioEvaluated) return; // ✅ Защита от двойной оценки
//         scenarioEvaluated = true;
//         string scenarioName = activeScenario != null ? activeScenario.scenarioName : "Unknown";
//         if (scenarioUI != null)
//         {
//             scenarioUI.ShowScenarioNotification(
//                 scenarioName, 
//                 message, 
//                 success
//             );
//             // ✅ Скрываем подсказку после оценки
//             Invoke(nameof(HideHintDelayed), 2f);
//         }
        
//         if (!success && tramController.evaluator != null)
//         {
//             tramController.evaluator.addPenalty($"Сценарий '{activeScenario.scenarioName}': {message}");
//         }
        
//         Debug.Log(success ? $"✅ {message}" : $"❌ {message}");
        
//         // ✅ Завершаем сценарий
//         CleanupScenario();
//     }

//     void CleanupScenario()
//     {
//         isRunning = false;
        
//         if (activeCar != null)
//         {

//         activeCar = null;
//         }
        
//         carInDangerZone = false;
//         scenarioEvaluated = false;
//         activeScenario = null;
        
//         Debug.Log("🧹 Сценарий очищен");

//         if (activePedestrian != null)
//         {
//             activePedestrian = null;
//         }
//         pedestrianInCrosswalk = false;
//     }


    
//     // ✅ Публичный метод для сброса всех сценариев (при перезапуске уровня)
//     public void ResetAllScenarios()
//     {
//         foreach (var sc in scenarios)
//         {
//             sc.ResetScenario();
//         }
//         CleanupScenario();
//     }


//     private PedestrianController activePedestrian;
//     private bool pedestrianInCrosswalk = false;
//     void ActivatePedestrian()
//     {
//         if (string.IsNullOrEmpty(activeScenario.pedestrianObjectName))
//         {
//             Debug.LogError("⚠️ Не указан имя пешехода!");
//             CleanupScenario();
//             return;
//         }

//         GameObject pedObj = GameObject.Find(activeScenario.pedestrianObjectName);
//         if (pedObj == null)
//         {
//             Debug.LogError($"⚠️ Пешеход '{activeScenario.pedestrianObjectName}' не найден!");
//             CleanupScenario();
//             return;
//         }

//         activePedestrian = pedObj.GetComponent<PedestrianController>();
        
//         if (activePedestrian == null)
//         {
//             Debug.LogError($"⚠️ У объекта нет компонента PedestrianController!");
//             CleanupScenario();
//             return;
//         }

//         // Находим точки пути
//         Transform[] pathTransforms = new Transform[activeScenario.pedestrianWaypointNames.Length];
//         bool allFound = true;
//         for (int i = 0; i < activeScenario.pedestrianWaypointNames.Length; i++)
//         {
//             GameObject wp = GameObject.Find(activeScenario.pedestrianWaypointNames[i]);
//             if (wp != null)
//                 pathTransforms[i] = wp.transform;
//             else
//             {
//                 Debug.LogError($"⚠️ Точка пути не найдена: {activeScenario.pedestrianWaypointNames[i]}");
//                 allFound = false;
//             }
//         }
        
//         if (!allFound)
//         {
//             Debug.LogError("⚠️ Не все точки пути найдены!");
//             CleanupScenario();
//             return;
//         }

//         activePedestrian.Setup(pathTransforms, activeScenario.pedestrianSpeed);
        
//         activePedestrian.OnEnterCrosswalk += () => pedestrianInCrosswalk = true;
//         activePedestrian.OnExitCrosswalk += () => {
//             if (!scenarioEvaluated)
//                 EvaluatePedestrianScenario(true, "Пешеход завершил переход.");
//         };

//         activePedestrian.OnHit += () => {
//             if (!scenarioEvaluated)
//             {
//                 EvaluatePedestrianScenario(false, "Пешеход сбит! Остановитесь перед переходом.");
//             }
//         };
        
//         activePedestrian.StartCrossing();
        
//         Debug.Log($"🚶 Пешеход '{activeScenario.pedestrianObjectName}' начал движение");
//     }

//     void CheckPedestrianScenario()
//     {
//         timer -= Time.deltaTime;
        
//         if (timer <= 0f && activePedestrian == null)
//         {
//             ActivatePedestrian();
//         }
        
//         if (activePedestrian != null && !scenarioEvaluated)
//         {
//             activePedestrian.CheckDangerZone(intersectionCenter, activeScenario.dangerZoneRadius);
            
//             if (pedestrianInCrosswalk)
//             {
//                 float tramSpeed = tramController.GetCurrentSpeed();
//                 if (tramSpeed > activeScenario.safeSpeedThreshold)
//                 {
//                     EvaluatePedestrianScenario(false, $"Скорость {tramSpeed * 3.6f:F0} км/ч — нужно было притормозить перед пешеходом!");
//                 }
//                 else
//                 {
//                     EvaluatePedestrianScenario(true, "Скорость снижена. Пешеход безопасно перешёл.");
//                 }
//             }
//         }
//     }

//     void EvaluatePedestrianScenario(bool success, string message)
//     {
//         if (scenarioEvaluated) return;
//         scenarioEvaluated = true;
        
//         string scenarioName = activeScenario != null ? activeScenario.scenarioName : "Unknown";
        
//         if (scenarioUI != null)
//         {
//             scenarioUI.ShowScenarioNotification(scenarioName, message, success);
//             Invoke(nameof(HideHintDelayed), 2f);
//         }
        
//         if (!success && tramController.evaluator != null)
//         {
//             tramController.evaluator.addPenalty($"Сценарий '{scenarioName}': {message}");
//         }
        
//         Debug.Log(success ? $"✅ {message}" : $"❌ {message}");
//         CleanupScenario();
//     }
// }

using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    [Header("🔗 Ссылки")]
    public TramOnRails tramController;
    public ScenarioUI scenarioUI;

    [Header("📂 Сценарии")]
    public ScenarioData[] scenarios;

    private ScenarioData activeScenario;
    private bool isRunning = false;
    private float timer = 0f;

    private CrossingCarController activeCar;
    private bool carInDangerZone = false;
    private bool scenarioEvaluated = false;
    private Vector3 intersectionCenter;

    private PedestrianController activePedestrian;
    private bool pedestrianInCrosswalk = false;
    
    void Start()
    {
        if (scenarios != null)
        {
            foreach (var sc in scenarios)
            {
                sc.hasBeenTriggered = false;
            }
        }
    }
    
    void Update()
    {
        if (tramController == null || scenarios == null || scenarios.Length == 0) return;

        if (!isRunning)
        {
            CheckActivation();
        }
        else
        {
            if (activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.TrafficLight)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    TriggerTrafficLightScenario();
                }
            }
            else if(activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.CarCrossing){
                CheckCarScenario();
            }
            else if(activeScenario != null && activeScenario.type == ScenarioData.ScenarioType.PedestrianCrossing){
                CheckPedestrianScenario();
            }
        }
    }

    void CheckActivation()
    {
        foreach (var sc in scenarios)
        {
            if (sc.hasBeenTriggered) continue;
            if (string.IsNullOrEmpty(sc.targetNodeName)) continue;

            GameObject nodeObj = GameObject.Find(sc.targetNodeName);
            if (nodeObj == null) continue;

            float dist = Vector3.Distance(tramController.transform.position, nodeObj.transform.position);
            if (dist <= sc.activationDistance)
            {
                Activate(sc, nodeObj);
                break;
            }
        }
    }

    void TriggerTrafficLightScenario()
    {
        if (activeScenario == null) return;

        if (activeScenario.changeTrafficLight && !string.IsNullOrEmpty(activeScenario.targetLightName))
        {
            GameObject lightObj = GameObject.Find(activeScenario.targetLightName);
            TrafficLight light = lightObj?.GetComponent<TrafficLight>();
            
            if (light != null)
            {
                light.ForceSetLight(activeScenario.setToColor);
            }
        }

        // ✅ СКРЫВАЕМ ПОДСКАЗКУ
        if (scenarioUI != null)
        {
            scenarioUI.HideHint();
        }

        CleanupScenario();
    }

    void Activate(ScenarioData data, GameObject nodeObj)
    {
        data.hasBeenTriggered = true;
        activeScenario = data;
        isRunning = true;
        scenarioEvaluated = false;

        if (data.type == ScenarioData.ScenarioType.TrafficLight)
        {
            timer = data.timeLimitSeconds;
        }
        else if (data.type == ScenarioData.ScenarioType.CarCrossing)
        {
            timer = data.activationDelay;
            
            if (data.carWaypointNames != null && data.carWaypointNames.Length > 0)
            {
                GameObject centerWp = GameObject.Find(data.carWaypointNames[data.carWaypointNames.Length / 2]);
                if (centerWp != null)
                    intersectionCenter = centerWp.transform.position;
            }
        }
        else if (data.type == ScenarioData.ScenarioType.PedestrianCrossing)
        {
            timer = data.activationDelay;
            
            if (data.pedestrianWaypointNames != null && data.pedestrianWaypointNames.Length > 0)
            {
                GameObject centerWp = GameObject.Find(data.pedestrianWaypointNames[data.pedestrianWaypointNames.Length / 2]);
                if (centerWp != null)
                    intersectionCenter = centerWp.transform.position;
            }
        }

        if (data.showHint && scenarioUI != null)
        {
            scenarioUI.ShowHint(data.hintText);
        }

        Debug.Log($"⏱ Сценарий '{data.scenarioName}' активирован");
    }

    void CheckCarScenario()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f && activeCar == null)
        {
            ActivateCar();
        }
        
        if (activeCar != null && !scenarioEvaluated)
        {
            activeCar.CheckDangerZone(intersectionCenter, activeScenario.dangerZoneRadius);
            
            if (carInDangerZone)
            {
                float tramSpeed = tramController.GetCurrentSpeed();
                
                // ✅ ПРОВЕРКА: реальная опасность (близость к машине)
                float distanceToCar = Vector3.Distance(
                    tramController.transform.position, 
                    activeCar.transform.position
                );
                
                // Штраф только если скорость высокая И машина близко (< 15м)
                if (tramSpeed > activeScenario.safeSpeedThreshold && distanceToCar < 15f)
                {
                    EvaluateCarScenario(false, $"Скорость {tramSpeed * 3.6f:F0} км/ч — опасное сближение!");
                }
                else if (!carInDangerZone)
                {
                    // Если вышли из опасной зоны и скорость была нормальной
                    EvaluateCarScenario(true, "Безопасный проезд.");
                }
            }
        }
    }

    void ActivateCar()
    {
        if (string.IsNullOrEmpty(activeScenario.carObjectName))
        {
            Debug.LogError("⚠️ Не указано имя машины в сценарии!");
            CleanupScenario();
            return;
        }

        GameObject carObj = GameObject.Find(activeScenario.carObjectName);
        if (carObj == null)
        {
            Debug.LogError($"⚠️ Машина '{activeScenario.carObjectName}' не найдена в сцене!");
            CleanupScenario();
            return;
        }

        activeCar = carObj.GetComponent<CrossingCarController>();
        
        if (activeCar == null)
        {
            Debug.LogError($"⚠️ У объекта '{activeScenario.carObjectName}' нет компонента CrossingCarController!");
            CleanupScenario();
            return;
        }

        Transform[] pathTransforms = new Transform[activeScenario.carWaypointNames.Length];
        bool allFound = true;
        for (int i = 0; i < activeScenario.carWaypointNames.Length; i++)
        {
            GameObject wp = GameObject.Find(activeScenario.carWaypointNames[i]);
            if (wp != null)
            {
                pathTransforms[i] = wp.transform;
            }
            else
            {
                Debug.LogError($"⚠️ Точка пути не найдена: {activeScenario.carWaypointNames[i]}");
                allFound = false;
            }
        }
        
        if (!allFound)
        {
            Debug.LogError("⚠️ Не все точки пути найдены — машина не поедет!");
            CleanupScenario();
            return;
        }

        activeCar.Setup(pathTransforms, activeScenario.carSpeed);
        
        activeCar.OnEnterDangerZone += () => carInDangerZone = true;
        activeCar.OnExitDangerZone += () => {
            if (!scenarioEvaluated)
                EvaluateCarScenario(true, "Машина проехала, помех не создано.");
        };
        activeCar.OnCrossingComplete += () => {
            if (!scenarioEvaluated)
                EvaluateCarScenario(true, "Машина завершила проезд.");
        };
        
        activeCar.StartCrossing();
        
        Debug.Log($"🚗 Машина начала движение");
    }

    void EvaluateCarScenario(bool success, string message)
    {
        if (scenarioEvaluated) return;
        scenarioEvaluated = true;
        
        // ✅ СОХРАНЯЕМ ДАННЫЕ ПЕРЕД ОЧИСТКОЙ
        string scenarioName = activeScenario != null ? activeScenario.scenarioName : "Unknown";
        
        if (!success && tramController.evaluator != null)
        {
            tramController.evaluator.addPenalty($"Сценарий '{scenarioName}': {message}");
        }
        
        Debug.Log(success ? $"✅ {message}" : $"❌ {message}");
        
        if (scenarioUI != null)
            scenarioUI.HideHint();
            
        CleanupScenario();
    }

    void CleanupScenario()
    {
        isRunning = false;
        
        if (activeCar != null)
        {
            activeCar = null;
        }
        carInDangerZone = false;
        
        if (activePedestrian != null)
        {
            activePedestrian = null;
        }
        pedestrianInCrosswalk = false;
        
        scenarioEvaluated = false;
        activeScenario = null;
    }

    public void ResetAllScenarios()
    {
        foreach (var sc in scenarios)
        {
            sc.ResetScenario();
        }
        CleanupScenario();
    }

    void ActivatePedestrian()
    {
        if (string.IsNullOrEmpty(activeScenario.pedestrianObjectName))
        {
            Debug.LogError("⚠️ Не указан имя пешехода!");
            CleanupScenario();
            return;
        }

        GameObject pedObj = GameObject.Find(activeScenario.pedestrianObjectName);
        if (pedObj == null)
        {
            Debug.LogError($"⚠️ Пешеход '{activeScenario.pedestrianObjectName}' не найден!");
            CleanupScenario();
            return;
        }

        activePedestrian = pedObj.GetComponent<PedestrianController>();
        
        if (activePedestrian == null)
        {
            Debug.LogError($"⚠️ У объекта нет компонента PedestrianController!");
            CleanupScenario();
            return;
        }

        Transform[] pathTransforms = new Transform[activeScenario.pedestrianWaypointNames.Length];
        bool allFound = true;
        for (int i = 0; i < activeScenario.pedestrianWaypointNames.Length; i++)
        {
            GameObject wp = GameObject.Find(activeScenario.pedestrianWaypointNames[i]);
            if (wp != null)
                pathTransforms[i] = wp.transform;
            else
            {
                Debug.LogError($"⚠️ Точка пути не найдена: {activeScenario.pedestrianWaypointNames[i]}");
                allFound = false;
            }
        }
        
        if (!allFound)
        {
            Debug.LogError("⚠️ Не все точки пути найдены!");
            CleanupScenario();
            return;
        }

        activePedestrian.Setup(pathTransforms, activeScenario.pedestrianSpeed);
        
        activePedestrian.OnEnterCrosswalk += () => pedestrianInCrosswalk = true;
        activePedestrian.OnExitCrosswalk += () => {
            if (!scenarioEvaluated)
                EvaluatePedestrianScenario(true, "Пешеход завершил переход.");
        };

        activePedestrian.OnHit += () => {
            if (!scenarioEvaluated)
            {
                EvaluatePedestrianScenario(false, "Пешеход сбит!");
            }
        };
        
        activePedestrian.StartCrossing();
        
        Debug.Log($"🚶 Пешеход начал движение");
    }

    void CheckPedestrianScenario()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f && activePedestrian == null)
        {
            ActivatePedestrian();
        }
        
        if (activePedestrian != null && !scenarioEvaluated)
        {
            activePedestrian.CheckDangerZone(intersectionCenter, activeScenario.dangerZoneRadius);
            
            if (pedestrianInCrosswalk)
            {
                float tramSpeed = tramController.GetCurrentSpeed();
                if (tramSpeed > activeScenario.safeSpeedThreshold)
                {
                    EvaluatePedestrianScenario(false, $"Скорость {tramSpeed * 3.6f:F0} км/ч — нужно было притормозить!");
                }
                else
                {
                    EvaluatePedestrianScenario(true, "Пешеход безопасно перешёл.");
                }
            }
        }
    }

    void EvaluatePedestrianScenario(bool success, string message)
    {
        if (scenarioEvaluated) return;
        scenarioEvaluated = true;
        
        // ✅ СОХРАНЯЕМ ДАННЫЕ ПЕРЕД ОЧИСТКОЙ
        string scenarioName = activeScenario != null ? activeScenario.scenarioName : "Unknown";
        
        if (!success && tramController.evaluator != null)
        {
            tramController.evaluator.addPenalty($"Сценарий '{scenarioName}': {message}");
        }
        
        Debug.Log(success ? $"✅ {message}" : $"❌ {message}");
        
        if (scenarioUI != null)
            scenarioUI.HideHint();
            
        CleanupScenario();
    }
}