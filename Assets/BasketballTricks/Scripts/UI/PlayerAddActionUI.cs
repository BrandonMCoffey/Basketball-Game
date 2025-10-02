using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAddActionUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    private PlayerControlsUIManager _manager;
    private PlayerActionData _action;
    private int _index;

    public void Init(PlayerControlsUIManager manager, PlayerActionData action, int index)
    {
        _manager = manager;
        _action = action;
        _index = index;

        _icon.sprite = action.Icon;
    }

    public void PressButton()
    {
        _manager.AddAction(_action, _index);
    }
}
