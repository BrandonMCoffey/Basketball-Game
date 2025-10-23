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

    private void OnValidate()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, 0);
    }

    public void TogglePanel()
    {
        _shown = !_shown;
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, _showTime).SetEase(Ease.OutQuart);
    }
}
