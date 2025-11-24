using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionData", menuName = "BasketballTricks/PlayerActionData", order = 1)]
public class PlayerActionData : ScriptableObject
{
    [SerializeField] private ActionData _data = new ActionData(ActionType.Trick);

    public ActionData Data => _data;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_data.Name)) _data.Name = name;
        if (_data.Duration <= 0f) _data.Duration = 1f;
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
    public CardRarity AssociatedRarity;
    public string Name;
    public float Cost;
    public float Hype;
    public float Duration;
    [Range(0, 1)] public float Accuracy;
    public Sprite Icon;
    public PlayerAnimation Animation;
    public string CardText;

    public ActionData(ActionType type = ActionType.None)
    {
        Type = type;
        AssociatedRarity = CardRarity.None;
        Name = type.ToString();
        Cost = 1;
        CardText = "";
        Hype = type switch
        {
            ActionType.Trick => 20,
            ActionType.Pass => 5,
            ActionType.Shot => 5,
            _ => 0,
        };
        Duration = type switch
        {
            ActionType.Trick => 3,
            ActionType.Pass => 2,
            ActionType.Shot => 4,
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
        Animation = type switch
        {
            ActionType.Trick => PlayerAnimation.BasicDribble,
            ActionType.Pass => PlayerAnimation.Pass,
            ActionType.Shot => PlayerAnimation.Shoot,
            _ => PlayerAnimation.BasicDribble,
        };
    }
}