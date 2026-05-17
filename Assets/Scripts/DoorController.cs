using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Настройки")]
    public bool isOpen = false;
    public float animationSpeed = 4f;
    public Vector3 openOffset = new Vector3(0, 0, -1.2f); // Направление сдвига при открытии

    private Vector3 closedPos;
    private Vector3 openPos;
    private Vector3 targetPos;
    private bool isAnimating = false;

    [Header("Логика пассажиров")]
    public float boardingRadius = 15f; // Радиус поиска остановки

    void Awake()
    {
        closedPos = transform.localPosition;
        openPos = closedPos + openOffset;
        targetPos = closedPos;
    }

    void Update()
    {
        if (isAnimating)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * animationSpeed);

            if (Vector3.Distance(transform.localPosition, targetPos) < 0.01f)
            {
                transform.localPosition = targetPos;
                isAnimating = false;
            }
        }
    }

    public void SetState(bool open)
    {
        if (isOpen == open) return; // Не перезапускаем анимацию, если состояние не меняется
        isOpen = open;
        targetPos = isOpen ? openPos : closedPos;
        isAnimating = true;

        if (isOpen)
        {
            CheckForStopsAndBoard();
        }
    }

    void CheckForStopsAndBoard()
    {
        // Ищем все коллайдеры в радиусе открытия двери
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, boardingRadius);

        foreach (var hit in hitColliders)
        {
            // Ищем скрипт остановки на этом объекте (или на родителе)
            BusStopController stop = hit.GetComponentInParent<BusStopController>();
            
            if (stop != null)
            {
                // Нашли остановку рядом! Загружаем пассажиров.
                stop.BoardPassengers();
            }
        }
    }

    public void Open() => SetState(true);
    public void Close() => SetState(false);
}