using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SlideInPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private bool _shown = true;
    [SerializeField] private Vector2 _shownPosition;
    [SerializeField] private Vector2 _hiddenPosition;
    [SerializeField] private float _showTime = 0.5f;
    [SerializeField] private List<SlideInPanel> _hideOtherPanelsWhenOpened = new List<SlideInPanel>();
    [SerializeField] private List<SlideInPanel> _showOtherPanelsWhenHidden = new List<SlideInPanel>();
    [SerializeField] private TextMeshProUGUI _slideToggleText;

    [SerializeField] private UnityEvent<bool> _onVisibilityChanged;

    private void OnValidate()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, 0);
    }

    public void TogglePanel() => SetShown(!_shown);
    public void SetShown(bool shown) => SetShown(shown, true);
    public void SetShown(bool shown, bool updateLinked)
    {
        if (_shown == shown) return;
        _shown = shown;
        _rectTransform.DOAnchorPos(_shown ? _shownPosition : _hiddenPosition, _showTime).SetEase(Ease.OutExpo);

        if (shown && updateLinked)
        {
            foreach (var panel in _hideOtherPanelsWhenOpened)
            {
                if (panel != null) panel.SetShown(false, false);
            }
        }
        else if (updateLinked)
        {
            foreach (var panel in _showOtherPanelsWhenHidden)
            {
                if (panel != null) panel.SetShown(true, false);
            }
        }

        if (_slideToggleText != null)
        {
            _slideToggleText.text = _shown ? "<-" : "->";
        }

        _onVisibilityChanged?.Invoke(_shown);

    }
}
