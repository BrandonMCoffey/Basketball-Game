using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _playerName;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private TeamData _team;

    [Header("Action 1")] // Trick
    [SerializeField] private PlayerActionData _action1;
    [SerializeField] private float _action1DurationOverride = -1;
    [SerializeField] private float _action1PointsOverride = -1;
    [SerializeField] private float _action1MultOverride = -1;
    [SerializeField, Range(0, 1)] private float _action1AccuracyOverride = 0;
    [SerializeField, ReadOnly] private ActionData _action1Data;

    [Header("Action 2")] // Pass
    [SerializeField] private PlayerActionData _action2;
    [SerializeField] private float _action2DurationOverride = -1;
    [SerializeField] private float _action2PointsOverride = -1;
    [SerializeField] private float _action2MultOverride = -1;
    [SerializeField, Range(0, 1)] private float _action2AccuracyOverride = 0;
    [SerializeField, ReadOnly] private ActionData _action2Data;

    [Header("Action 3")] // Shot
    [SerializeField] private PlayerActionData _action3;
    [SerializeField] private float _action3DurationOverride = -1;
    [SerializeField] private float _action3PointsOverride = -1;
    [SerializeField] private float _action3MultOverride = -1;
    [SerializeField, Range(0, 1)] private float _action3AccuracyOverride = 0;
    [SerializeField, ReadOnly] private ActionData _action3Data;

    public string PlayerName => _playerName;
    public Sprite PlayerSprite => _playerSprite;
    public Sprite TeamLogo => _team != null ? _team.TeamLogo : null;

    public ActionData GetAction(int index)
    {
        ActionData data;
        switch (index)
        {
            case 0:
                data = _action1 != null ? _action1.Data : new ActionData(ActionType.Trick);
                if (_action1DurationOverride >= 0) data.Duration = _action1DurationOverride;
                if (_action1PointsOverride >= 0) data.Points = _action1PointsOverride;
                if (_action1MultOverride >= 0) data.Mult = _action1MultOverride;
                if (_action1AccuracyOverride > 0) data.Accuracy = _action1AccuracyOverride;
                return data;
            case 1:
                data = _action2 != null ? _action2.Data : new ActionData(ActionType.Pass);
                if (_action2DurationOverride >= 0) data.Duration = _action2DurationOverride;
                if (_action2PointsOverride >= 0) data.Points = _action2PointsOverride;
                if (_action2MultOverride >= 0) data.Mult = _action2MultOverride;
                if (_action2AccuracyOverride > 0) data.Accuracy = _action2AccuracyOverride;
                return data;
            case 2:
                data = _action3 != null ? _action3.Data : new ActionData(ActionType.Shot);
                if (_action3DurationOverride >= 0) data.Duration = _action3DurationOverride;
                if (_action3PointsOverride >= 0) data.Points = _action3PointsOverride;
                if (_action3MultOverride >= 0) data.Mult = _action3MultOverride;
                if (_action3AccuracyOverride > 0) data.Accuracy = _action3AccuracyOverride;
                return data;
            default:
                return new ActionData();
        }
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_playerName)) _playerName = name;
        _action1Data = _action1 != null ? _action1.Data : new ActionData(ActionType.Trick);
        _action2Data = _action2 != null ? _action2.Data : new ActionData(ActionType.Pass);
        _action3Data = _action3 != null ? _action3.Data : new ActionData(ActionType.Shot);
    }
}
