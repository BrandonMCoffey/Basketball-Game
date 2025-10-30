using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuReference
{
    public RectTransform Panel;
    public Vector2 OnScreenPosition;
    public Vector2 OffScreenPosition;
    public float SlideInDuration;
    public Ease SlideInEase;
    public UnityEvent<bool> OnPanelToggled;

    public void ShowPanel()
    {
        var onPanelToggled = OnPanelToggled;
        Panel.DOAnchorPos(OnScreenPosition, SlideInDuration)
            .SetEase(SlideInEase)
            .OnComplete(() => onPanelToggled?.Invoke(true));
    }

    public void HidePanel()
    {
        var onPanelToggled = OnPanelToggled;
        Panel.DOAnchorPos(OffScreenPosition, SlideInDuration)
            .SetEase(SlideInEase)
            .OnComplete(() => onPanelToggled?.Invoke(false));
    }
}