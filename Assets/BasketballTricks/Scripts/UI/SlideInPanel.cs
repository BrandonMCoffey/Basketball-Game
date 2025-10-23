using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private bool _shown = true;
    [SerializeField] private Vector2 _shownPosition;
    [SerializeField] private Vector2 _hiddenPosition;
    [SerializeField] private float _showTime = 0.5f;
    [SerializeField] private List<SlideInPanel> _hideOtherPanelsWhenOpened = new List<SlideInPanel>();

    private void OnValidate()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, 0);
    }

    public void TogglePanel() => SetShown(!_shown);
    public void SetShown(bool shown)
    {
        if (_shown == shown) return;
        _shown = shown;
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, _showTime).SetEase(Ease.OutQuart);

        if (shown)
        {
            foreach (var panel in _hideOtherPanelsWhenOpened)
            {
                if (panel != null) panel.SetShown(false);
            }
        }
    }
}
