using UnityEngine;

public class RouteNode : MonoBehaviour
{
    public string nodeName = "Node";

    // Следующие узлы
    public RouteNode defaultNext;
    public RouteNode leftNext;
    public RouteNode rightNext;
    public RouteNode previous;

    // Визуальный индикатор (опционально)
    // public Renderer indicator;
    // public Color neutralColor = Color.gray;
    // public Color leftColor = Color.blue;
    // public Color rightColor = Color.red;

    // Получить следующий узел по режиму
    public RouteNode GetNextNode(SwitchMode mode)
    {
        return mode switch
        {
            SwitchMode.Left => leftNext,
            SwitchMode.Right => rightNext,
            _ => defaultNext
        };
    }

    public RouteNode GetPrevious(){
        return this.previous;
    }

    // Обновить цвет индикатора
    // public void UpdateIndicator(SwitchMode mode)
    // {
    //     if (indicator != null)
    //     {
    //         indicator.material.color = mode switch
    //         {
    //             SwitchMode.Left => leftColor,
    //             SwitchMode.Right => rightColor,
    //             _ => neutralColor
    //         };
    //     }
    // }
}