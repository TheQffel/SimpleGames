using UnityEngine;
using UnityEngine.Events;

public class AdjustScrollViewToScreen : MonoBehaviour
{
    public RectTransform ReferenceResolution;
    public RectTransform ScreenSize;
    Vector2 CurrentScale = new Vector2();

    void Update()
    {
        if(ReferenceResolution.sizeDelta != CurrentScale)
        {
            CurrentScale = ReferenceResolution.sizeDelta;
            ScreenSize.sizeDelta = CurrentScale;
        }
    }
}
