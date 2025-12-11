using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Collections;

public class LongButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    [SerializeField] string _text;
    [SerializeField] float _clickScale = 1.1f;
    [SerializeField] UnityEvent _onButtonPressed;
    [SerializeField] Ease _clickStartEase = Ease.InOutCubic;
    [SerializeField] Ease _clickEndEase = Ease.OutBack;
    [SerializeField] bool _moveTextOnPress = true;
    [SerializeField, ShowIf("_moveTextOnPress")] bool _moveTextUpOnPress = true;
    [SerializeField] bool _invokeOnAnimComplete = false;

    [Header("References")]
    [SerializeField] TextMeshProUGUI _buttonText;

    bool _interactable = true;
    public bool Interactable 
    { 
        get => _interactable; 
        set
        {
            _interactable = value;
            _buttonText.color = _interactable ? Color.white : Color.gray;
        }
    }
    float _moveTextAmount = 100f;
    Vector2 _originalTextPos;
    bool _checkForMousePos = false;
    RectTransform _rectTransform;   

    void OnValidate()
    {
        if (_buttonText == null) return;
        if (string.IsNullOrEmpty(_text) || _buttonText != null)
        {
            _buttonText.text = _text;
        }
    }

    private void Start() 
    {
        _moveTextAmount = _moveTextUpOnPress ? -100f : 100f;
        if (_buttonText != null)
            _originalTextPos = _buttonText.rectTransform.anchoredPosition;
        _rectTransform = GetComponent<RectTransform>();

        // Force no moving text
        _moveTextOnPress = false;
    }

    private void Update()
    {
        if (_checkForMousePos)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, null, out localPoint);
            if (!_rectTransform.rect.Contains(localPoint))
            {
                transform.DOScale(1f, 0.2f).SetEase(_clickEndEase);
                _checkForMousePos = false;
                if (_buttonText != null)
                {
                    _buttonText.rectTransform.DOAnchorPos(_originalTextPos, 0.2f).SetEase(_clickEndEase);
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_interactable) return;
        _rectTransform.DOScale(_clickScale, 0.1f).SetEase(_clickStartEase);
        if (_moveTextOnPress && _buttonText != null)
            _buttonText.rectTransform.DOAnchorPos(_originalTextPos + Vector2.up * _moveTextAmount, 0.1f).SetEase(_clickStartEase);
        _checkForMousePos = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_interactable) return;
        _rectTransform.DOScale(1f, 0.2f).SetEase(_clickEndEase);
        if (_moveTextOnPress && _buttonText != null)
            _buttonText.rectTransform.DOAnchorPos(_originalTextPos, 0.2f).SetEase(_clickEndEase);
        if (_checkForMousePos) StartCoroutine(InvokeActionRoutine(0.2f));
        _checkForMousePos = false;
    }

    IEnumerator InvokeActionRoutine(float delay = 0f)
    {
        if (_invokeOnAnimComplete)
        {
            yield return new WaitForSeconds(delay);
        }
        _onButtonPressed?.Invoke();
    }
}
