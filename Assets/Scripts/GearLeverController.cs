using UnityEngine;

public class GearLeverController : ButtonLeverBase
{
    [Header("Позиции — относительные смещения")]
    public Vector3 forwardOffset = Vector3.zero;      // например, 0,0,0
    public Vector3 reverseOffset = new Vector3(0, 180, 0); // поворот на 180° (если ручка переворачивается)

    [Header("Связь с логикой")]
    public TramOnRails tramController;

    protected override void UpdateTargetRotation()
    {
        if (tramController == null) return;

        Vector3 offset = tramController.currentDirection switch
        {
            TramDirection.Forward => forwardOffset,
            TramDirection.Reverse => reverseOffset,
            _ => forwardOffset
        };

        targetRotation = startRotation * Quaternion.Euler(offset);
    }
}