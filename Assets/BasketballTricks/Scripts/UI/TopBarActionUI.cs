using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarActionUI : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;

    public void SetAction(Player player, PlayerActionData action)
    {
        if (_background != null) _background.color = player.PositionColor;
        if (_icon != null) _icon.sprite = action.Icon;
        if (_text != null) _text.text = action.Duration.ToString("0.0") + "s";
        var rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(10 + action.Duration * 10, rect.sizeDelta.y);
    }
}
