using UnityEngine;

public class RouteNode : MonoBehaviour
{
    public string nodeName = "Node";

    // Следующие узлы
    public RouteNode defaultNext;
    public RouteNode leftNext;
    public RouteNode rightNext;
    public RouteNode previous;

    public bool isSwitchPoint = false;
    [Header("Ограничения")]
    public bool hasSpeedLimit = false;
    [Tooltip("Максимальная разрешённая скорость в м/с")]
    public float maxSpeed=15f;

    // public bool isStopZone = false; 
    // public float stopZoneRadius = 8f;

    // public enum NodeType
    // {
    //     Regular,
    //     StopZoneStart,
    //     StopZoneEnd
    // }

    // public NodeType nodeType = NodeType.Regular;

    public bool isStopNode = false;      // ← это начало зоны
    public float stopZoneLength = 8f;    // длина зоны вперёд по маршруту
    public string stopName = "Остановка";

    public RouteNode GetNextNode(SwitchMode mode)
    {

        if (!isSwitchPoint)
            return defaultNext;


        return mode switch
        {
            SwitchMode.Left => leftNext,
            SwitchMode.Right => rightNext,
            _ => defaultNext
        };
    }

    public RouteNode GetPrevious(){
        return previous;
    }


}