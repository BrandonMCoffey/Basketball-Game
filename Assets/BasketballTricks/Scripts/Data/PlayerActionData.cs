using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerActionData", menuName = "BasketballTricks/PlayerActionData", order = 1)]
public class PlayerActionData : ScriptableObject
{
    [SerializeField] private ActionData _data = new ActionData(ActionType.Trick);

    public ActionData Data => _data;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_data.Name)) _data.Name = name;
        if (_data.Duration <= 0f) _data.Duration = 1f;
        if (_data.HasNextEffect) _data.NextEffectPreviewText = _data.NextEffect.GetDisplayText(_data.ActionLevel);
        _data.PreviewText = _data.GetDisplayText();
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
    public string Name;
    [TextArea] public string CardText;
    [TextArea, ReadOnly] public string PreviewText;
    public ActionType Type;
    public CardRarity AssociatedRarity;
    public PlayerPosition AllowedPositions;
    public float Cost;
    public int ActionLevel;
    [Header("Effects")]
    public List<float> HypeGainPerLevel;
    public bool HasGainEnergy;
    [ShowIf(nameof(HasGainEnergy))] public List<float> EnergyGainPerLevel;
    public bool HasNextEffect;
    [ShowIf(nameof(HasNextEffect))] public EffectNext NextEffect;
    [ShowIf(nameof(HasNextEffect)), ReadOnly] public string NextEffectPreviewText;
    [Header("Visuals")]
    public Sprite Icon;
    public PlayerAnimation Animation;
    public float Duration;

    public float HypeGain => HypeGainPerLevel.Count == 0 ? 10 : HypeGainPerLevel[Mathf.Clamp(ActionLevel - 1, 0, HypeGainPerLevel.Count - 1)];
    public float EnergyGain => EnergyGainPerLevel.Count == 0 ? 0 : EnergyGainPerLevel[Mathf.Clamp(ActionLevel - 1, 0, EnergyGainPerLevel.Count - 1)];

    public ActionData(ActionType type = ActionType.None)
    {
        Name = type.ToString();
        Icon = null;
        Type = type;
        AssociatedRarity = CardRarity.None;
        AllowedPositions = PlayerPosition.All;
        Cost = 1;
        ActionLevel = 1;
        float baseHype = type switch
        {
            ActionType.Trick => 2.5f,
            ActionType.Pass => 1f,
            ActionType.Shot => 5f,
            _ => 2,
        };
        baseHype *= AssociatedRarity switch
        {
            CardRarity.Rookie => 2f,
            CardRarity.Career => 4f,
            CardRarity.AllStar => 8f,
            CardRarity.Signature => 10f,
            _ => 2f,
        };
        HypeGainPerLevel = new List<float> { baseHype, baseHype * 1.5f, baseHype * 2f };
        HasGainEnergy = false;
        EnergyGainPerLevel = new List<float> { 0, 0, 0};
        HasNextEffect = false;
        NextEffect = new EffectNext
        {
            AppliesTo = NextEffectAppliesTo.NextCardPlayed,
            RequiredType = ActionType.Trick,
            EnergyEffect = 0,
            HypeEffectPerLevel = new List<float> { 5, 10, 15 },
        };
        NextEffectPreviewText = "";
        CardText = "";
        PreviewText = CardText;
        Animation = type switch
        {
            ActionType.Trick => PlayerAnimation.BasicDribble,
            ActionType.Pass => PlayerAnimation.Pass,
            ActionType.Shot => PlayerAnimation.Shoot,
            _ => PlayerAnimation.BasicDribble,
        };
        Duration = 2;
    }

    public string GetDisplayText()
    {
        string text = CardText;
        text = text.Replace("@Hype", HypeGain.ToString("F1"));
        text = text.Replace("@Energy", EnergyGain.ToString("F1"));
        text = text.Replace("@Cost", Cost.ToString("F1"));
        text = text.Replace("@Duration", Duration.ToString("F1"));
        text = text.Replace("@NextEffect", NextEffect.GetDisplayText(ActionLevel));
        return text;
    }
}

[System.Serializable]
public struct EffectNext
{
    public NextEffectAppliesTo AppliesTo;
    public ActionType RequiredType;
    public List<float> HypeEffectPerLevel;
    public float EnergyEffect;

    public float GetHypeEffect(int level) => HypeEffectPerLevel.Count == 0 ? 10 : HypeEffectPerLevel[Mathf.Clamp(level - 1, 0, HypeEffectPerLevel.Count - 1)];

    public string GetDisplayText(int level)
    {
        string effectText = $"Next {RequiredType} ";
        effectText += AppliesTo switch
        {
            NextEffectAppliesTo.NextCardPlayed => "played ",
            NextEffectAppliesTo.NextMatchingCardThisHand => "this hand ",
            NextEffectAppliesTo.NextMatchingCardThisGame => "this game ",
            _ => "",
        };
        float hypeEffect = GetHypeEffect(level);
        bool hypeEffectExists = hypeEffect != 0;
        if (hypeEffect > 0) effectText += $"gains +{hypeEffect} Hype";
        else if (hypeEffect < 0) effectText += $"loses -{Mathf.Abs(hypeEffect)} Hype";
        bool energyEffectExists = EnergyEffect != 0;
        if (energyEffectExists)
        {
            if (hypeEffectExists) effectText += " and ";
            if (EnergyEffect > 0) effectText += $"gains +{EnergyEffect} Energy.";
            else if (EnergyEffect < 0) effectText += $"loses -{Mathf.Abs(EnergyEffect)} Energy.";
        }
        return effectText;
    }
}

public enum NextEffectAppliesTo
{
    NextCardPlayed,
    NextMatchingCardThisHand,
    NextMatchingCardThisGame,
}