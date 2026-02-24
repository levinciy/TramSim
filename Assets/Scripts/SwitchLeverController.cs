using UnityEngine;

public class SwitchLeverController : ButtonLeverBase
{
    [Header("Позиции — относительные смещения от начального положения")]
    public Vector3 neutralOffset = Vector3.zero;
    public Vector3 leftOffset = new Vector3(0, -45, 0);
    public Vector3 rightOffset = new Vector3(0, +45, 0);

    [Header("Связь с логикой")]
    public TramOnRails tramController;

    protected override void UpdateTargetRotation()
    {
        if (tramController == null) return;

        Vector3 offset = tramController.switchMode switch
        {
            SwitchMode.Neutral => neutralOffset,
            SwitchMode.Left => leftOffset,
            SwitchMode.Right => rightOffset,
            _ => neutralOffset
        };

        targetRotation = startRotation * Quaternion.Euler(offset);
    }
}