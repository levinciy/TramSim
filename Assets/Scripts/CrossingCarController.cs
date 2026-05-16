using UnityEngine;
using System;

public class CrossingCarController : MonoBehaviour
{
    [Header("Настройки")]
    public Transform[] waypoints;
    public float speed = 8f;
    
    public event Action OnEnterDangerZone;
    public event Action OnExitDangerZone;
    public event Action OnCrossingComplete; // ✅ Новое событие

    private int currentWaypoint = 0;
    private bool isMoving = false;
    private bool hasEnteredZone = false;
    private bool hasFinished = false; // ✅ Защита от повторного завершения

    public void Setup(Transform[] pathWaypoints, float moveSpeed)
    {
        waypoints = pathWaypoints;
        speed = moveSpeed;
        hasFinished = false;
    }

    public void StartCrossing()
    {
        if (hasFinished) return; // ✅ Защита
        
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("⚠️ CrossingCarController: нет точек пути!");
            return;
        }

        currentWaypoint = 0;
        isMoving = true;
        hasEnteredZone = false;
        transform.position = waypoints[0].position;
        transform.gameObject.SetActive(true);
        
        Debug.Log($"🚗 Машина начала движение: {waypoints.Length} точек, скорость {speed} м/с");
    }

    void Update()
    {
        if (!isMoving || hasFinished || waypoints == null || waypoints.Length < 2) return;

        transform.position = Vector3.MoveTowards(
            transform.position, 
            waypoints[currentWaypoint].position, 
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                FinishCrossing();
            }
        }
    }

    public void CheckDangerZone(Vector3 center, float radius)
    {
        if (!isMoving || hasFinished) return;
        
        float dist = Vector3.Distance(transform.position, center);
        
        if (dist <= radius && !hasEnteredZone)
        {
            hasEnteredZone = true;
            OnEnterDangerZone?.Invoke();
            Debug.Log("🚨 Машина в опасной зоне!");
        }
        else if (dist > radius && hasEnteredZone)
        {
            hasEnteredZone = false;
            OnExitDangerZone?.Invoke();
        }
    }

    void FinishCrossing()
    {
        if (hasFinished) return; // ✅ Защита от двойного вызова
        
        isMoving = false;
        hasFinished = true;
        
        Debug.Log("✅ Машина завершила проезд перекрёстка");
        
        OnCrossingComplete?.Invoke();
        
        // ✅ Не деактивируем сразу — дадим время на оценку
        Invoke(nameof(DeactivateCar), 3f);
    }

    void DeactivateCar()
    {
        transform.gameObject.SetActive(false);
        Debug.Log("🚗 Машина деактивирована");
    }
    
    // ✅ Публичный метод для принудительной остановки (если нужно)
    public void ForceStop()
    {
        isMoving = false;
        hasFinished = true;
        DeactivateCar();
    }
}