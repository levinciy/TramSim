using UnityEngine;

public class SwitchPoint : MonoBehaviour
{
    public Transform defaultBranch; 
    public Transform leftBranch;    
    public Transform rightBranch;   

    // Опционально: визуальный индикатор
    // public Renderer indicator;
    // public Color neutralColor = Color.gray;
    // public Color leftColor = Color.blue;
    // public Color rightColor = Color.red;

    // void OnValidate()
    // {
    //     if (indicator == null)
    //         indicator = GetComponent<Renderer>();
    // }

    // public void SetIndicator(SwitchMode mode)
    // {
    //     if (indicator == null) return;
    //     switch (mode)
    //     {
    //         case SwitchMode.Left: indicator.material.color = leftColor; break;
    //         case SwitchMode.Right: indicator.material.color = rightColor; break;
    //         default: indicator.material.color = neutralColor; break;
    //     }
    // }
}