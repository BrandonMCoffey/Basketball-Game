using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Collections;
using CoffeyUtils.Sound;
using System;

public class LongButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    [SerializeField, TextArea] string _text;
    [SerializeField] float _clickScale = 1.1f;
    [SerializeField] float _delayBeforeInvoke = 0f;
    [SerializeField] UnityEvent _onButtonPressed;
    public Action OnButtonPressed;
    [SerializeField] Ease _clickStartEase = Ease.InOutCubic;
    [SerializeField] Ease _clickEndEase = Ease.OutBack;
    [SerializeField] bool _moveTextOnPress = true;
    [SerializeField, ShowIf("_moveTextOnPress")] bool _moveTextUpOnPress = true;
    [SerializeField] bool _invokeOnAnimComplete = false;
    [SerializeField] SFXEventsEnum _clickStartSfx = SFXEventsEnum.ButtonClickStart;
    [SerializeField] SFXEventsEnum _clickReleaseSfx = SFXEventsEnum.ButtonClickRelease;

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

    public void SetText(string newText)
    {
        _text = newText;
        if (_buttonText != null)
        {
            _buttonText.text = _text;
        }
    }

    private void Start() 
    {
        _moveTextAmount = _moveTextUpOnPress ? -100f : 100f;
        
        if (_buttonText != null)
        {
            _originalTextPos = _buttonText.rectTransform.anchoredPosition;
        }
        
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
                    SoundManager.PlaySfx(_clickReleaseSfx);
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_interactable) return;
        _rectTransform.DOScale(_clickScale, 0.1f).SetEase(_clickStartEase);
        
        if (_moveTextOnPress && _buttonText != null)
        {
            _buttonText.rectTransform.DOAnchorPos(_originalTextPos + Vector2.up * _moveTextAmount, 0.1f).SetEase(_clickStartEase);
        }
        
        SoundManager.PlaySfx(_clickStartSfx);
        _checkForMousePos = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_interactable) return;
        _rectTransform.DOScale(1f, 0.2f).SetEase(_clickEndEase);
        
        if (_moveTextOnPress && _buttonText != null)
        {
            _buttonText.rectTransform.DOAnchorPos(_originalTextPos, 0.2f).SetEase(_clickEndEase);
        }
        
        if (_checkForMousePos) StartCoroutine(InvokeActionRoutine(0.2f));
        SoundManager.PlaySfx(_clickReleaseSfx);
        _checkForMousePos = false;
    }

    IEnumerator InvokeActionRoutine(float delay = 0f)
    {
        if (_invokeOnAnimComplete) 
        {
            yield return new WaitForSeconds(Mathf.Max(delay, _delayBeforeInvoke));
        }
        else
        {
            yield return new WaitForSeconds(_delayBeforeInvoke);
        }

        _onButtonPressed?.Invoke();
        OnButtonPressed?.Invoke();
    }

    public void Jump()
    {
        if (!_interactable) return;
        transform.DOScale(_clickScale, 0.1f).SetEase(_clickStartEase).OnComplete(() => 
        {
            transform.DOScale(1f, 0.2f).SetEase(_clickEndEase).SetDelay(0.15f);
        });
    }
}
