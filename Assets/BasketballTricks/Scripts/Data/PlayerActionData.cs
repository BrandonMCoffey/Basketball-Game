using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionData", menuName = "BasketballTricks/PlayerActionData", order = 1)]
public class PlayerActionData : ScriptableObject
{
    [SerializeField] private ActionData _data = new ActionData(ActionType.Trick);
    [SerializeField] private float _animationDuration = 0;

    public ActionData Data => _data;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_data.Name)) _data.Name = name;
        if (_data.Duration <= 0f) _data.Duration = 1f;
        _animationDuration = _data.Animation != null ? _data.Animation.length : 0;
    }

    [Button]
    private void ResetValues()
    {
        var sprite = _data.Icon;
        var clip = _data.Animation;
        _data = new ActionData(_data.Type);
        _data.Name = name;
        _data.Icon = sprite;
        _data.Animation = clip;
    }
}

public enum ActionType
{
    None,
    Trick,
    Pass,
    Shot
}

[System.Serializable]
public struct ActionData
{
    public ActionType Type;
    public string Name;
    public float Duration;
    public float Points;
    public float Mult;
    [Range(0, 1)] public float Accuracy;
    public Sprite Icon;
    public AnimationClip Animation;

    public ActionData(ActionType type = ActionType.None)
    {
        Type = type;
        Name = type.ToString();
        Duration = type switch
        {
            ActionType.Trick => 1,
            ActionType.Pass => 2,
            ActionType.Shot => 4,
            _ => 0,
        };
        Points = type switch
        {
            ActionType.Trick => 20,
            ActionType.Pass => 5,
            ActionType.Shot => 5,
            _ => 0,
        };
        Mult = type switch
        {
            ActionType.Trick => 2,
            ActionType.Pass => 0,
            ActionType.Shot => 10,
            _ => 0,
        };
        Accuracy = type switch
        {
            ActionType.Trick => 0.8f,
            ActionType.Pass => 0.95f,
            ActionType.Shot => 0.9f,
            _ => 0,
        };
        Icon = null;
        Animation = null;
    }
}