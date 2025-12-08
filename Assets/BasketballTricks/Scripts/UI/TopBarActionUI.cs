using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarActionUI : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Sprite _defaultIconTrick;
    [SerializeField] private Sprite _defaultIconPass;
    [SerializeField] private Sprite _defaultIconShot;

    private int _index;

    public void SetAction(Player player, ActionData data, int index)
    {
        if (_background != null) _background.color = player.PositionColor;
        if (_icon != null) _icon.sprite = GetSprite(data);
        if (_text != null) _text.text = data.Duration.ToString("0.0") + "s";
        var rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(10 + data.Duration * 10, rect.sizeDelta.y);
        _index = index;
    }

    public void RemoveAction()
    {
        //PlayerManager.Instance.RemoveAction(_index);
    }

    private Sprite GetSprite(ActionData data)
    {
        if (data.Icon != null) return data.Icon;
        return data.Type switch
        {
            ActionType.Trick => _defaultIconTrick,
            ActionType.Pass => _defaultIconPass,
            ActionType.Shot => _defaultIconShot,
            _ => null,
        };
    }
}
