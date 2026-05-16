using UnityEngine;
using System;

public class PedestrianController : MonoBehaviour
{
    [Header("Настройки")]
    public Transform[] waypoints;
    public float walkSpeed = 1.5f;
    
    public event Action OnEnterCrosswalk;
    public event Action OnExitCrosswalk;

    private int currentWaypoint = 0;
    private bool isMoving = false;
    private bool hasEnteredZone = false;

    public void Setup(Transform[] pathWaypoints, float speed)
    {
        waypoints = pathWaypoints;
        walkSpeed = speed;
    }

    public void StartCrossing()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("⚠️ PedestrianController: нет точек пути!");
            return;
        }

        currentWaypoint = 0;
        isMoving = true;
        hasEnteredZone = false;
        transform.position = waypoints[0].position;
        transform.gameObject.SetActive(true);
        
        Debug.Log($" Пешеход начал движение: {waypoints.Length} точек");
    }

    void Update()
    {
        if (!isMoving || waypoints == null || waypoints.Length < 2) return;

        // Движение к следующей точке
        transform.position = Vector3.MoveTowards(
            transform.position, 
            waypoints[currentWaypoint].position, 
            walkSpeed * Time.deltaTime
        );

        // Поворот в направлении движения
        if (currentWaypoint < waypoints.Length - 1)
        {
            Vector3 direction = waypoints[currentWaypoint + 1].position - transform.position;
            if (direction.magnitude > 0.1f)
            {
                transform.forward = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * 5f);
            }
        }

        // Достигли точки
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
        if (!isMoving) return;
        
        float dist = Vector3.Distance(transform.position, center);
        
        if (dist <= radius && !hasEnteredZone)
        {
            hasEnteredZone = true;
            OnEnterCrosswalk?.Invoke();
            Debug.Log(" Пешеход на переходе!");
        }
        else if (dist > radius && hasEnteredZone)
        {
            hasEnteredZone = false;
            OnExitCrosswalk?.Invoke();
        }
    }

    void FinishCrossing()
    {
        isMoving = false;
        Debug.Log(" Пешеход завершил переход");
        
        OnExitCrosswalk?.Invoke();
        Invoke(nameof(DeactivatePedestrian), 2f);
    }

    void DeactivatePedestrian()
    {
        transform.gameObject.SetActive(false);
    }


    public event Action OnHit; // Событие "пешеход сбит"

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что столкнулись с трамваем (по тегу или компоненту)
        if ((other.CompareTag("Tram") || other.GetComponent<TramOnRails>() != null) && isMoving)
        {
            OnHit?.Invoke();
            Debug.Log("💥 Пешеход сбит!");

            isMoving = false;
            Invoke(nameof(DeactivatePedestrian), 0.5f);
        }
    }
}