using UnityEngine;

public class BusStopController : MonoBehaviour
{
    [Header("Настройки")]
    public string stopName = "Остановка 1";

    [Header("Пассажиры")]
    // Кто стоит на улице (исчезнет)
    public GameObject[] passengersAtStop;
    
    // Кто сидит в трамвае (появится)
    // Ссылка на пустой объект внутри трамвая, где сидят пассажиры этой остановки
    public GameObject tramPassengerGroup; 

    private bool isBoarded = false;

    public void BoardPassengers()
    {
        if (isBoarded) return; // Чтобы не спавнить дважды
        isBoarded = true;

        Debug.Log($" {stopName}: Пассажиры садятся в трамвай!");

        // 1. Скрываем тех, кто на улице
        foreach (var p in passengersAtStop)
        {
            if (p != null) p.SetActive(false);
        }

        // 2. Показываем тех, кто в салоне (если ссылка есть)
        if (tramPassengerGroup != null)
        {
            tramPassengerGroup.SetActive(true);
        }
    }

    // Метод для сброса (при перезапуске уровня)
    public void ResetStop()
    {
        isBoarded = false;
        foreach (var p in passengersAtStop)
            if (p != null) p.SetActive(true);
            
        if (tramPassengerGroup != null)
            tramPassengerGroup.SetActive(false);
    }
}