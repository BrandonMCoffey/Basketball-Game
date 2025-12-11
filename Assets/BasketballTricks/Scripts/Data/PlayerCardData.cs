using CoffeyUtils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCardData", menuName = "BasketballTricks/PlayerCardData")]
public class PlayerCardData : ScriptableObject
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private string _cardTitle;
    [SerializeField] private CardRarity _rarity;

    [SerializeField] public List<CustomPlayerAction> _actions = new List<CustomPlayerAction>();

    public PlayerData PlayerData => _playerData;
    public string CardTitle => _cardTitle;
    public CardRarity Rarity => _rarity;

    public int ActionCount => _actions.Count == 0 ? 3 : _actions.Count;
    public int GetActionCount(int index)
    {
        if (index >= _actions.Count || index < 0)
        {
            return index switch
            {
                0 => 1,
                1 => 1,
                2 => 1,
                _ => 0,
            };
        }
        return _actions[index].Count;
    }
    public int GetActionIndex(PlayerActionData actionData)
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            if (_actions[i].GetActionSO() == actionData)
            {
                return i;
            }
        }
        return -1;
    }
    public ActionData GetAction(int index)
    {
        if (index >= _actions.Count || index < 0)
        {
            return index switch
            {
                0 => new ActionData(ActionType.Trick),
                1 => new ActionData(ActionType.Pass),
                2 => new ActionData(ActionType.Shot),
                _ => new ActionData(ActionType.None)
            };
        }
        return _actions[index].GetData();
    }
    public List<CustomPlayerAction> GetAllActions() => new List<CustomPlayerAction>(_actions);

    private void OnValidate()
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            _actions[i].UpdateDataPreview();
        }
    }

    public void PrepareForGameplay()
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            _actions[i].PrepareForGameplay();
        }
    }
}

[System.Serializable]
public struct CustomPlayerAction
{
    [SerializeField] private PlayerActionData _action;
    [SerializeField] private LevelableFloat _count;
    [SerializeField] private Sprite _imageOverride;
    [SerializeField, ReadOnly] private ActionData _dataPreview;

    public CustomPlayerAction(PlayerActionData action = null)
    {
        _action = action;
        _count = new LevelableFloat(false, 1, 1, 1);
        _imageOverride = null;
        _dataPreview = new ActionData();
    }

    public int Count => Mathf.RoundToInt(_action != null ? _count.GetValue(_action.Data.ActionLevel) : _count.GetValue(1));
    public PlayerActionData GetActionSO() => _action;
    public ActionData GetData()
    {
        ActionData data = _action != null ? _action.Data : new ActionData(ActionType.Trick);
        if (_imageOverride != null) data.Icon = _imageOverride;
        return data;
    }
    public void UpdateDataPreview() => _dataPreview = GetData();
    public void PrepareForGameplay()
    {
        if (_action != null) _action.PrepareForGameplay();
    }
}

public enum CardRarity
{
    None,
    Rookie,
    Career,
    AllStar,
    Signature
}