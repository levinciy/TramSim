using UnityEngine;

// Позволяет создавать ассеты через меню Unity
[CreateAssetMenu(fileName = "TrafficLightScenario", menuName = "TramSim/Scenario/TrafficLight")]
public class ScenarioData : ScriptableObject
{
    public enum ScenarioType { TrafficLight, CarCrossing, PedestrianCrossing }
    public ScenarioType type = ScenarioType.TrafficLight;
    [Header(" Общие")]
    public string scenarioName = "Светофор: Красный";
    //public RouteNode triggerNode;          // Узел маршрута, где активируется сценарий
    public string targetNodeName = "";
    public float activationDistance = 15f; // Дистанция до узла для запуска (метры)

    [Header("Светофор")]
    public bool changeTrafficLight = true;
    //public TrafficLight targetLight;       // Ссылка на объект светофора
    public string targetLightName = "";
    public TrafficLight.LightColors setToColor = TrafficLight.LightColors.Red;

    [Header(" Условия выполнения")]
    public float timeLimitSeconds = 10f;   // Сколько секунд даётся на реакцию
    public float successSpeedThreshold = 0.5f; // Скорость ниже которой считается "остановкой"

    [Header(" Машина")]
    // public GameObject carPrefab;
    // public Transform carSpawnPoint;
    // public Transform[] carWaypoints;
    public string carObjectName = "";
    public string carSpawnPointName = "";       
    public string[] carWaypointNames;  
    public float carSpeed = 8f;
    public float activationDelay = 2f;
    public float safeSpeedThreshold = 5f;
    public float dangerZoneRadius = 8f;

    [Header(" Пешеход")]
    public string pedestrianObjectName = "";
    public string[] pedestrianWaypointNames;
    public float pedestrianSpeed = 1.5f;
    public float pedestrianActivationDelay = 2f;

    [Header(" Подсказка")]
    public bool showHint = true;
    [TextArea(3, 5)] public string hintText = "Впереди красный сигнал. Остановитесь перед стоп-линией!";

    public bool hasBeenTriggered = false;

    public void ResetScenario()
    {
        hasBeenTriggered = false;
    }
}