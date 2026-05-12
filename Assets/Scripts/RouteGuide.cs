using UnityEngine;

public class RouteGuide : MonoBehaviour
{
    [Header("Настройки")]
    public TramOnRails tramController; // Ссылка на скрипт трамвая
    public GameObject routeMarker;     // Твой 3D-маркер
    public float markerHeight = 2.5f;  // Высота над рельсами, чтобы было видно
    public bool lookAtCamera = true;   // Поворачивать маркер к камере?

    private Transform currentTarget;
    private Camera playerCamera;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Если маркер не назначен, создаем временный куб
        if (routeMarker == null)
        {
            Debug.LogWarning("⚠️ Маркер не назначен! Создаю временный.");
            routeMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            routeMarker.GetComponent<Renderer>().material.color = Color.red;
        }

        routeMarker.SetActive(false);
    }

    void Update()
    {
        if (tramController == null) return;

        // 1. Получаем целевой узел из контроллера трамвая
        // (Нам нужно, чтобы TramOnRails мог отдать нам цель)
        RouteNode targetNode = tramController.GetGuidanceTarget();

        if (targetNode != null && tramController.IsCorrectRoute(targetNode))
        {
            if (!routeMarker.activeSelf)
                routeMarker.SetActive(true);

            // 2. Двигаем маркер к узлу
            Vector3 targetPos = targetNode.transform.position;
            targetPos.y += markerHeight; // Поднимаем вверх
            routeMarker.transform.position = targetPos;

            // 3. Вращение маркера (Billboard effect - всегда лицом к камере)
            if (lookAtCamera)
            {
                routeMarker.transform.LookAt(playerCamera.transform);
            }
            else
            {
                // Иначе просто смотрим вдоль пути (опционально)
                routeMarker.transform.rotation = targetNode.transform.rotation;
            }
        }
        else
        {
            // Если целей нет (конец маршрута) — скрываем маркер
            routeMarker.SetActive(false);
        }
    }
}