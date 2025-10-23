using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    [SerializeField] string _text;
    public string Text => _text;

    [Header("References")]
    [SerializeField] TextMeshProUGUI _buttonText;
    public TextMeshProUGUI ButtonText => _buttonText;
    [SerializeField] Image _buttonIcon;
    public Image ButtonIcon => _buttonIcon;

    private void OnValidate()
    {
        if (_buttonText == null) return;
        if (string.IsNullOrEmpty(_text) || ButtonText != null)
        {
            ButtonText.text = _text;
        }

        if (_buttonIcon == null) return;
        if (_icon == null || ButtonIcon != null)
        {
            ButtonIcon.sprite = _icon;
        }
    }

    public void OnTapBegin() { }
    public void OnTapEnd() { }
}
