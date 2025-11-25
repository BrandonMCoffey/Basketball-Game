using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionCard : MonoBehaviour
{
    [SerializeField] private ActionData _actionData;
    [SerializeField] private Image _colorImage;
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private TMP_Text _actionDescription;
    [SerializeField] private TMP_Text _actionType;
    [SerializeField] private TMP_Text _actionCost;
    [SerializeField] private TMP_Text _actionHype;
    [SerializeField] private Image _actionIcon;

    public void Init(ActionData data, Color color)
    {
        _actionData = data;
        if (_actionName != null) _actionName.text = data.Name;
        if (_actionDescription != null) _actionDescription.text = data.GetDisplayText();
        if (_actionType != null) _actionType.text = data.Type.ToString();
        if (_actionCost != null) _actionCost.text = data.Cost.GetValue(data.ActionLevel).ToString("0");
        if (_actionHype != null) _actionHype.text = data.HypeGain.GetValue(data.ActionLevel).ToString("0");
        if (_actionIcon != null) _actionIcon.sprite = data.Icon;
        if (_colorImage != null) _colorImage.color = color;
    }
}
