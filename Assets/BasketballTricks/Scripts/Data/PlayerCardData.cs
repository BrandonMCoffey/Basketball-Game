using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCardData", menuName = "BasketballTricks/PlayerCardData")]
public class PlayerCardData : ScriptableObject
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private string _cardTitle;
    [SerializeField] private CardRarity _rarity;

    [SerializeField] private List<CustomPlayerAction> _actions = new List<CustomPlayerAction>();

    public PlayerData PlayerData => _playerData;

    public int ActionCount => _actions.Count;
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
    [SerializeField] private int _count = 1;
    [SerializeField] private int _unlockLevel = 0;
    [SerializeField] private float _actionCostOverride = -1;
    [SerializeField] private float _actionHypeOverride = -1;
    [SerializeField] private float _actionDurationOverride = -1;
    [SerializeField, Range(0, 1)] private float _actionAccuracyOverride = 0;
    [ReadOnly] public ActionData DataPreview;

    public int Count => _count;
    public ActionData GetData()
    {
        ActionData data = _action != null ? _action.Data : new ActionData(ActionType.Trick);
        if (_actionCostOverride >= 0) data.Cost = _actionCostOverride;
        if (_actionHypeOverride >= 0) data.Hype = _actionHypeOverride;
        if (_actionDurationOverride >= 0) data.Duration = _actionDurationOverride;
        if (_actionAccuracyOverride > 0) data.Accuracy = _actionAccuracyOverride;
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