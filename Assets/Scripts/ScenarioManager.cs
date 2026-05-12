using UnityEngine;

// public class ScenarioManager : MonoBehaviour
// {
//     [Header("🔗 Ссылки")]
//     public TramOnRails tramController;
//     public DriverEvaluator evaluator;
    
//     // Опционально: если есть UIManager для подсказок, назначь его. Если нет — оставь пустым.
//     public Component uiHintTarget; 

//     [Header("📂 Сценарии")]
//     public ScenarioData[] scenarios;

//     private ScenarioData activeScenario;
//     private bool isRunning = false;
//     private float timer = 0f;

//     void Update()
//     {
//         if (tramController == null || scenarios == null || scenarios.Length == 0) return;

//         if (!isRunning)
//         {
//             CheckActivation();
//         }
//         else
//         {
//             RunTimerAndCheck();
//         }
//     }

//     // 1️⃣ Поиск точки активации
//     void CheckActivation()
//     {
//         foreach (var sc in scenarios)
//         {
//             //if (sc.triggerNode == null) continue;
//             if (string.IsNullOrEmpty(sc.targetNodeName)) continue;
//             GameObject NodeObj = GameObject.Find(sc.targetNodeName);
//             RouteNode node = NodeObj?.GetComponent<RouteNode>();
//             float dist = Vector3.Distance(tramController.transform.position, node.transform.position);
//             // float dist = Vector3.Distance(tramController.transform.position, sc.triggerNode.transform.position);
//             if (dist <= sc.activationDistance)
//             {
//                 Activate(sc);
//                 break; // Запускаем только первый подходящий
//             }
//         }
//     }

//     // 2️⃣ Запуск сценария
//     void Activate(ScenarioData data)
//     {
//         activeScenario = data;
//         isRunning = true;
//         timer = data.timeLimitSeconds;

//         // Применяем изменения к светофору
//         // if (data.changeTrafficLight && data.targetLight != null)
//         // {
//         //     data.targetLight.ForceSetLight(data.setToColor);
//         // }

//         if (data.changeTrafficLight && !string.IsNullOrEmpty(data.targetLightName))
//         {
//             GameObject lightObj = GameObject.Find(data.targetLightName);
//             TrafficLight light = lightObj?.GetComponent<TrafficLight>();
//             //if (light != null)
//                 light.ForceSetLight(data.setToColor);
//         }

//         // Показываем подсказку (если есть UI)
//         if (data.showHint && uiHintTarget != null)
//         {
//             // Пример вызова: uiHintTarget.SendMessage("ShowHint", data.hintText);
//             // Или просто Debug, если UI ещё нет
//             Debug.Log($"💬 Подсказка: {data.hintText}");
//         }

//         Debug.Log($"🎬 АКТИВИРОВАН: {data.scenarioName}");
//     }

//     // 3️⃣ Логика активного сценария
//     void RunTimerAndCheck()
//     {
//         timer -= Time.deltaTime;

//         // ✅ Проверка успеха
//         if (tramController.GetCurrentSpeed() <= activeScenario.successSpeedThreshold)
//         {
//             //GetComponent<Light>().ForceSetLight(TrafficLight.LightColors.Green);
//             Finish(true, "Остановка выполнена вовремя");
//             return;
//         }

//         // ❌ Проверка провала
//         if (timer <= 0f)
//         {
//             Finish(false, "Время вышло. Трамвай проехал на красный.");
//         }
//     }

//     // 4️⃣ Завершение
//     void Finish(bool success, string message)
//     {
//         isRunning = false;
        
//         if (success)
//             Debug.Log($"✅ УСПЕХ: {activeScenario.scenarioName} — {message}");
//         else
//         {
//             Debug.Log($"❌ ПРОВАЛ: {activeScenario.scenarioName} — {message}");
//             if (evaluator != null)
//                 evaluator.addPenalty($"Сценарий '{activeScenario.scenarioName}': {message}");
//         }

//         activeScenario = null;
//     }

//     // 🛠 Для отладки: запуск сценария по индексу без езды
//     public void DebugStartScenario(int index)
//     {
//         if (index >= 0 && index < scenarios.Length)
//         {
//             isRunning = false;
//             Activate(scenarios[index]);
//         }
//     }
// }

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

    void Update()
    {
        if (tramController == null || scenarios == null || scenarios.Length == 0) return;

        if (!isRunning)
        {
            CheckActivation();
        }
        else
        {
            // Просто отсчитываем время до переключения
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                TriggerScenario();
            }
        }
    }

    void CheckActivation()
    {
        foreach (var sc in scenarios)
        {
            if (string.IsNullOrEmpty(sc.targetNodeName)) continue;

            GameObject nodeObj = GameObject.Find(sc.targetNodeName);
            if (nodeObj == null) continue;

            float dist = Vector3.Distance(tramController.transform.position, nodeObj.transform.position);
            if (dist <= sc.activationDistance)
            {
                // activeScenario = sc;
                // isRunning = true;
                // timer = sc.timeLimitSeconds; // Теперь это "время до красного"
                // Debug.Log($" Сценарий '{sc.scenarioName}' активирован. Красный через {sc.timeLimitSeconds} сек.");
                Activate(sc,nodeObj);
                break;
            }
        }
    }

    void TriggerScenario()
    {
        if (activeScenario == null) return;

        // 🚦 Просто переключаем светофор на красный
        if (activeScenario.changeTrafficLight && !string.IsNullOrEmpty(activeScenario.targetLightName))
        {
            GameObject lightObj = GameObject.Find(activeScenario.targetLightName);
            TrafficLight light = lightObj?.GetComponent<TrafficLight>();
            
            if (light != null)
            {
                light.ForceSetLight(activeScenario.setToColor);
                if (scenarioUI != null)
                {
                    scenarioUI.ShowScenarioNotification(
                        activeScenario.scenarioName,
                        $"Светофор переключён на {activeScenario.setToColor}",
                        true // isSuccess = true (это не штраф, просто событие)
                    );
                    
                    // Скрываем подсказку через 2 секунды
                    Invoke(nameof(HideHintDelayed), 2f);
                }
                //Debug.Log($"🚦 СЦЕНАРИЙ: Светофор '{activeScenario.targetLightName}' переключён на {activeScenario.setToColor}");
            }
        }

        if (activeScenario.showHint)
        {
            Debug.Log($"💬 Подсказка: {activeScenario.hintText}");
        }


        isRunning = false;
        activeScenario = null;
    }

    void Activate(ScenarioData data, GameObject nodeObj)
    {
        activeScenario = data;
        isRunning = true;
        timer = data.timeLimitSeconds;


        if (data.showHint && scenarioUI != null)
        {
            scenarioUI.ShowHint(data.hintText);
        }

        Debug.Log($"⏱ Сценарий '{data.scenarioName}' активирован. Красный через {data.timeLimitSeconds} сек.");
    }

    void HideHintDelayed()
    {
        if (scenarioUI != null)
            scenarioUI.HideHint();
    }
}