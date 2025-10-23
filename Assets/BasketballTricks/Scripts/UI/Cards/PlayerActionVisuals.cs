using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionVisuals : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _duration;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _mult;
    [SerializeField] private Image _icon;

    public void SetData(PlayerActionData data)
    {
        if (_name != null) _name.text = data != null ? data.name : "Action";
        if (_duration != null) _duration.text = data != null ? $"{data.Duration} second{(data.Duration > 1 ? "s" : "")}" : "1 second";
        if (_score != null) _score.text = $"Score: {(data != null ? data.Score : 0)}";
        if (_mult != null) _mult.text = $"Mult: {(data != null ? data.Mult : 0)}";
        if (_icon != null) _icon.sprite = data != null ? data.Icon : null;
    }
}
