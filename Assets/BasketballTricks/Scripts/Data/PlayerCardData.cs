using CoffeyUtils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
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

    public int ActionCount => _actions.Count();
    public int GetActionCount(int index)
    {
        if (index >= _actions.Count || index < 0) return 0;
        return _actions[index].Count;
    }
    public ActionData GetAction(int index)
    {
        if (index >= _actions.Count || index < 0) return new ActionData(index == 2 ? ActionType.Shot : (index == 1 ? ActionType.Pass : ActionType.Trick));
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
}

[System.Serializable]
public class CustomPlayerAction
{
    [SerializeField] private PlayerActionData _action;
    [SerializeField] private LevelableFloat _count = new LevelableFloat(false, 1, 1, 1);
    [SerializeField] private Sprite _imageOverride;
    [SerializeField, ReadOnly] private ActionData _dataPreview;

    public int Count => Mathf.RoundToInt(_action != null ? _count.GetValue(_action.Data.ActionLevel) : _count.GetValue(1));
    public PlayerActionData GetActionSO() => _action;
    public ActionData GetData()
    {
        ActionData data = _action != null ? _action.Data : new ActionData(ActionType.Trick);
        if (_imageOverride != null) data.Icon = _imageOverride;
        return data;
    }
    public void UpdateDataPreview() => _dataPreview = GetData();
}

public enum CardRarity
{
    None,
    Rookie,
    Career,
    AllStar,
    Signature
}