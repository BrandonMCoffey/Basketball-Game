using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
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

    [Header("Functionality")]
    [SerializeField] UnityEvent _onButtonReleased;

    RectTransform _rectTransform;
    Vector2 _onScreenAnchoredPos;
    List<Vector2> _iconPositions = new List<Vector2>();
    Vector2 _onScreenButtonTextContainerPos;
    bool _checkForMousePos = false;

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
        AnimateOnScreen();
    }

    // Animates button and its elements onto the screen
    public void AnimateOnScreen()
    {
        _rectTransform.DOAnchorPos(_onScreenAnchoredPos, 0.5f).SetDelay(_delayBeforeOnScreen).SetEase(Ease.OutBack);

        _buttonTextContainer.DOAnchorPos(_onScreenButtonTextContainerPos, 0.5f).SetDelay(_delayBeforeOnScreen + 0.2f).SetEase(Ease.OutBack);
        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            //if (_icons == null || _icons.Count == 0) return;
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            iconRect.DOAnchorPos(_iconPositions[i], 0.5f).SetDelay(_delayBeforeOnScreen + 0.2f + i * 0.05f).SetEase(Ease.OutBack);
        }
    }

    public void AnimateOffScreen()
    {
        _rectTransform.DOAnchorPos(new Vector2(_onScreenAnchoredPos.x, -Screen.height * 2), 0.5f).SetEase(Ease.InBack).SetDelay(_delayBeforeOnScreen);

        _buttonTextContainer.DOAnchorPos(new Vector2(_onScreenButtonTextContainerPos.x, -Screen.height * 2), 0.5f).SetDelay(_delayBeforeOnScreen + 0.2f).SetEase(Ease.InBack);

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            //if (_icons == null || _icons.Count == 0) return;
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            iconRect.DOAnchorPos(new Vector2(_iconPositions[i].x, -Screen.height * 2), 0.5f).SetDelay(_delayBeforeOnScreen + 0.2f + i * 0.05f).SetEase(Ease.InBack);
        }
    }

    // Sets initial positions of button and its elements
    void Initialize()
    {
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

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            //if (_icons == null || _icons.Count == 0) return;
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            _iconPositions.Add(iconRect.anchoredPosition);
            iconRect.anchoredPosition += _startOffScreen ? new Vector2(Random.Range(-100, 100), -Random.Range(100, 300)) : Vector2.zero;
        }

    }

    // Creates icon images and positions them with slight offsets
    void CreateImages()
    {
        // if (_icons == null || _icons.Count == 0)
        // {
        //     _buttonIcon.gameObject.SetActive(false);
        //     return;
        // }
        // _buttonIcon.sprite = _icons[0];
        //_buttonIcon.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-6f, 6f));

        for (int i = 1; i < _icons.Count; i++)
        {
            Image iconInstance = Instantiate(_buttonIcon, _iconContainer);
            iconInstance.sprite = _icons[i];
            iconInstance.rectTransform.anchoredPosition += new Vector2(i * 5, 0);
            iconInstance.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-6f, 6f));

        }
    }

    void Update()
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
        _rectTransform.DOScale(0.8f, 0.1f).SetEase(Ease.OutCirc);
        _buttonTextContainer.DOAnchorPosY(_buttonTextContainer.anchoredPosition.y - 180, 0.1f).SetEase(Ease.OutCirc);

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            iconRect.DOAnchorPosY(_onScreenAnchoredPos.y + 200 + Random.Range(0, 100), 0.2f).SetEase(Ease.OutCirc).SetDelay(0.075f + i * 0.03f);
        }
        
        _checkForMousePos = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TapEnd();
        _onButtonReleased?.Invoke();
    }

    void TapEnd()
    {
        _rectTransform.DOScale(0.7f, 0.1f).SetEase(Ease.OutCirc);
        _buttonTextContainer.DOAnchorPos(_onScreenButtonTextContainerPos, 0.1f).SetEase(Ease.OutCirc);

        for (int i = 0; i < _iconContainer.childCount; i++)
        {
            RectTransform iconRect = _iconContainer.GetChild(i).GetComponent<RectTransform>();
            iconRect.DOAnchorPos(_iconPositions[i], 0.2f).SetEase(Ease.OutCirc).SetDelay(0.075f + i * 0.03f);
        }

        _checkForMousePos = false;
    }
}
