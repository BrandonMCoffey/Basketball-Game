using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class LongButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Settings")]
    [SerializeField] string _text;
    [SerializeField] UnityEvent _onButtonPressed;
    [SerializeField] bool _moveTextUpOnPress = true;


    [Header("References")]
    [SerializeField] TextMeshProUGUI _buttonText;

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
        _originalTextPos = _buttonText.rectTransform.anchoredPosition;
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_checkForMousePos)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_buttonText.rectTransform.parent as RectTransform, Input.mousePosition, null, out localPoint);
            if (!_rectTransform.rect.Contains(localPoint))
            {
                transform.DOScale(1f, 0.2f);
                _checkForMousePos = false;
                _buttonText.rectTransform.DOAnchorPos(_originalTextPos, 0.2f);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(1.25f, 0.2f);
        _buttonText.rectTransform.DOAnchorPosY(_buttonText.rectTransform.anchoredPosition.y - _moveTextAmount, 0.2f);
        _checkForMousePos = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.2f);
        _checkForMousePos = false;
        _buttonText.rectTransform.DOAnchorPos(_originalTextPos, 0.2f);
        _onButtonPressed?.Invoke();

    }
}
