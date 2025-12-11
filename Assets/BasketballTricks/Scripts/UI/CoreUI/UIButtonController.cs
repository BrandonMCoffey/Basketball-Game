using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using CoffeyUtils.Sound;

public class UIButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Button Settings")]
    [SerializeField] private List<Sprite> _icons;
    [SerializeField] string _text;
    [SerializeField] bool _startOffScreen = true;
    [SerializeField] Ease _easeType = Ease.OutBack;
    [SerializeField, ShowIf("_startOffScreen")] float _delayBeforeOnScreen = 0.5f;

    [Header("References")]
    [SerializeField] RectTransform _buttonTextContainer;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Image _buttonIcon;

    [Header("Functionality")]
    [SerializeField] UnityEvent _onButtonReleased;

    RectTransform _rectTransform;
    Vector2 _onScreenAnchoredPos;
    Vector2 _onScreenButtonTextContainerPos;
    bool _checkForMousePos = false;
    bool _positionsInitialized = false;
    bool _imagesCreated = false;

    private void OnValidate()
    {
        if (_buttonText == null) return;
        if (string.IsNullOrEmpty(_text) || _buttonText != null)
        {
            _buttonText.text = _text;
        }
    }

    private void Start()
    {
        AnimateOnScreen();
    }

    public void AnimateOnScreen() 
    {
        Initialize();
        CreateImages();
        AnimOn();
    }

    // Animates button and its elements onto the screen
    private void AnimOn()
    {
        _rectTransform.DOAnchorPos(_onScreenAnchoredPos, 0.25f).SetDelay(_delayBeforeOnScreen).SetEase(_easeType);

        _buttonTextContainer.DOAnchorPos(_onScreenButtonTextContainerPos, 0.25f).SetDelay(_delayBeforeOnScreen).SetEase(_easeType);
        _buttonIcon.rectTransform.DOAnchorPos(Vector2.zero, 0.25f).SetDelay(_delayBeforeOnScreen).SetEase(_easeType);
    }

    public void AnimateOffScreen()
    {
        _rectTransform.DOAnchorPos(new Vector2(_onScreenAnchoredPos.x, -Screen.height * 2), 0.25f).SetEase(_easeType).SetDelay(_delayBeforeOnScreen);

        _buttonTextContainer.DOAnchorPos(new Vector2(_onScreenButtonTextContainerPos.x, -Screen.height * 2), 0.25f).SetDelay(_delayBeforeOnScreen).SetEase(_easeType);
        _buttonIcon.rectTransform.DOAnchorPos(new Vector2(0, -Screen.height * 3), 0.25f).SetDelay(_delayBeforeOnScreen + 0.2f).SetEase(_easeType);
    }

    // Sets initial positions of button and its elements
    private void Initialize()
    {
        if (_positionsInitialized) return;
        _positionsInitialized = true;
        if (_icons == null || _icons.Count == 0)
        {
            _buttonIcon.rectTransform.localPosition = Vector2.zero;
        }

        _rectTransform = GetComponent<RectTransform>();
        _onScreenAnchoredPos = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = _startOffScreen ? new Vector2(_onScreenAnchoredPos.x, -Screen.height * 2) : _onScreenAnchoredPos;

        _onScreenButtonTextContainerPos = _buttonTextContainer.anchoredPosition;

        _buttonTextContainer.anchoredPosition = _startOffScreen ? 
            new Vector2(_onScreenButtonTextContainerPos.x, -Screen.height * 2) : 
            _onScreenButtonTextContainerPos;

    }

    // Creates icon images and positions them with slight offsets
    private void CreateImages()
    {
        if (_imagesCreated) return;
        if (_icons == null || _icons.Count == 0) return;
        _buttonIcon.sprite = _icons[0];
        _imagesCreated = true;
    }

    private void Update()
    {
        if (_checkForMousePos)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, null, out localMousePos);
            if (!_rectTransform.rect.Contains(localMousePos))
            {
                TapEnd();
                _checkForMousePos = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.DOScale(0.8f, 0.1f).SetEase(_easeType);
        _buttonIcon.rectTransform.DOAnchorPos(new Vector2(0, -10f), 0.1f).SetEase(_easeType);
        _checkForMousePos = true;
        SoundManager.PlaySfx(SFXEventsEnum.ButtonClickStart);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_checkForMousePos) _onButtonReleased?.Invoke();
        TapEnd();
    }

    private void TapEnd()
    {
        _rectTransform.DOScale(0.7f, 0.1f).SetEase(_easeType);
        _buttonIcon.rectTransform.DOAnchorPos(Vector2.zero, 0.1f).SetEase(_easeType);
        _checkForMousePos = false;
        SoundManager.PlaySfx(SFXEventsEnum.ButtonClickRelease);
    }
}
