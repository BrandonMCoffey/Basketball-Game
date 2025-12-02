using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UIButtonController : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private List<Sprite> _icons;
    [SerializeField] string _text;
    [SerializeField] bool _startOffScreen = true;
    [SerializeField, ShowIf("_startOffScreen")] float _delayBeforeOnScreen = 0.5f;

    [Header("References")]
    [SerializeField] RectTransform _buttonTextContainer;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] RectTransform _iconContainer;
    [SerializeField] Image _buttonIcon;

    RectTransform _rectTransform;
    Vector2 _onScreenAnchoredPos;
    List<Vector2> _iconPositions = new List<Vector2>();

    private void OnValidate()
    {
        if (_buttonText == null) return;
        if (string.IsNullOrEmpty(_text) || _buttonText != null)
        {
            _buttonText.text = _text;
        }
    }

    void Start() 
    {
        CreateImages();
        Initialize();
        AnimateOnScreen(_delayBeforeOnScreen);
    }

    // Animates button and its elements onto the screen
    void AnimateOnScreen(float delay)
    {
        _rectTransform.DOAnchorPos(_onScreenAnchoredPos, 0.5f).SetDelay(delay).SetEase(Ease.OutBack);

        _buttonTextContainer.DOAnchorPos(new Vector2(_buttonTextContainer.anchoredPosition.x, _onScreenAnchoredPos.y + 20), 0.5f)
            .SetDelay(delay + 0.1f).SetEase(Ease.OutBack);

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            if (_icons == null || _icons.Count == 0) return;
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            iconRect.DOAnchorPos(_iconPositions[i], 0.5f).SetDelay(delay + 0.2f + i * 0.05f).SetEase(Ease.OutBack);
        }
    }

    // Sets initial positions of button and its elements
    void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _onScreenAnchoredPos = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = _startOffScreen ? new Vector2(_onScreenAnchoredPos.x, -Screen.height * 2) : _onScreenAnchoredPos;

        _buttonTextContainer.anchoredPosition = _startOffScreen ? 
            new Vector2(_buttonTextContainer.anchoredPosition.x, -Screen.height) : 
            _buttonTextContainer.anchoredPosition;

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            if (_icons == null || _icons.Count == 0) return;
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            _iconPositions.Add(iconRect.anchoredPosition);
            iconRect.anchoredPosition += _startOffScreen ? new Vector2(Random.Range(-100, 100), -Random.Range(100, 300)) : Vector2.zero;
        }
    }

    // Creates icon images and positions them with slight offsets
    void CreateImages()
    {
        if (_icons == null || _icons.Count == 0)
        {
            _buttonIcon.gameObject.SetActive(false);
            return;
        }
        _buttonIcon.sprite = _icons[0];
        _buttonIcon.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-6f, 6f));

        for (int i = 1; i < _icons.Count; i++)
        {
            Image iconInstance = Instantiate(_buttonIcon, _iconContainer);
            iconInstance.sprite = _icons[i];
            iconInstance.rectTransform.anchoredPosition += new Vector2(i * 5, 0);
            iconInstance.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-6f, 6f));

        }
    }

    public void OnTapBegin() { }
    public void OnTapEnd() { }
}
