using System.Collections;
using System.Collections.Generic;
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


    [Header("References")]
    [SerializeField] TextMeshProUGUI _buttonText;

    void OnValidate()
    {
        if (_buttonText == null) return;
        if (string.IsNullOrEmpty(_text) || _buttonText != null)
        {
            _buttonText.text = _text;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Add functionality for when the button is pressed down
        transform.DOScale(1.25f, 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Add functionality for when the button is released
        transform.DOScale(1f, 0.2f);
        _onButtonPressed?.Invoke();
    }
}
