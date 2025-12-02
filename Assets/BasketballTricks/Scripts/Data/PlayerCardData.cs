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

    [SerializeField] private List<CustomPlayerAction> _actions = new List<CustomPlayerAction>();

    public PlayerData PlayerData => _playerData;

    public int ActionCount => _actions.Count();
    public int GetActionCount(int index)
    {
        if (index >= _actions.Count || index < 0) return 0;
        return _actions[index].Count;
    }
    public ActionData GetAction(int index)
    {
        if (index >= _actions.Count || index < 0) return new ActionData();
        return _actions[index].GetData();
    }

    private void OnValidate()
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            _actions[i].DataPreview = _actions[i].GetData();
        }
    }
}

[System.Serializable]
public class CustomPlayerAction
{
    [SerializeField] private PlayerActionData _action;
    [SerializeField] private LevelableFloat _count = new LevelableFloat(false, 1, 1, 1);
    [ReadOnly] public ActionData DataPreview;

    public int Count => Mathf.RoundToInt(_action != null ? _count.GetValue(_action.Data.ActionLevel) : _count.GetValue(1));
    public ActionData GetData()
    {
        ActionData data = _action != null ? _action.Data : new ActionData(ActionType.Trick);
        return data;
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