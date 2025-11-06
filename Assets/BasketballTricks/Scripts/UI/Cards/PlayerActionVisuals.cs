using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionVisuals : MonoBehaviour
{
    [SerializeField] private TMP_Text _type;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _duration;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _mult;
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _defaultIconTrick;
    [SerializeField] private Sprite _defaultIconPass;
    [SerializeField] private Sprite _defaultIconShot;

    public void SetData(ActionData data)
    {
        if (_type != null) _type.text = data.Type.ToString();
        if (_name != null) _name.text = data.Name;
        if (_duration != null) _duration.text = $"{data.Duration} second{(data.Duration > 1 ? "s" : "")}";
        if (_score != null) _score.text = $"Score: {data.Points}";
        if (_mult != null) _mult.text = $"Mult: {data.Mult}";
        if (_icon != null) _icon.sprite = GetSprite(data);
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
