using UnityEngine;

// Позволяет создавать ассеты через меню Unity
[CreateAssetMenu(fileName = "TrafficLightScenario", menuName = "TramSim/Scenario/TrafficLight")]
public class ScenarioData : ScriptableObject
{
    [Header("📋 Общие")]
    public string scenarioName = "Светофор: Красный";
    //public RouteNode triggerNode;          // Узел маршрута, где активируется сценарий
    public string targetNodeName = "";
    public float activationDistance = 15f; // Дистанция до узла для запуска (метры)

    [Header("🚦 Светофор")]
    public bool changeTrafficLight = true;
    //public TrafficLight targetLight;       // Ссылка на объект светофора
    public string targetLightName = "";
    public TrafficLight.LightColors setToColor = TrafficLight.LightColors.Red;

    [Header("⏱ Условия выполнения")]
    public float timeLimitSeconds = 10f;   // Сколько секунд даётся на реакцию
    public float successSpeedThreshold = 0.5f; // Скорость ниже которой считается "остановкой"

    [Header("💬 Подсказка")]
    public bool showHint = true;
    [TextArea(3, 5)] public string hintText = "Впереди красный сигнал. Остановитесь перед стоп-линией!";
}