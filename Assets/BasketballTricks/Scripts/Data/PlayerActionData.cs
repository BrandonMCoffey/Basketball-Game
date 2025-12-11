using CoffeyUtils;
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
        _data.UpdatePreviewText();
    }

    public void PrepareForGameplay()
    {
        _data.NextEffect.Level = _data.ActionLevel;
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
    [SerializeField, TextArea] private string _cardText;
    [SerializeField, TextArea, ReadOnly] private string _previewText;
    [SerializeField, TextArea, ReadOnly] private string _cardSummary;

    [Header("Data")]
    public ActionType Type;
    public CardRarity AssociatedRarity;
    public PlayerPosition AllowedPositions;
    public int ActionLevel;
    [SerializeField] private AppliedEffects _cardData;
    public bool HasEffectIfSequence;
    [ShowIf(nameof(HasEffectIfSequence))] public EffectIfSequence EffectIfSequence;
    [ShowIf(nameof(HasEffectIfSequence)), SerializeField, ReadOnly] private string _effectIfSequencePreviewText;
    public bool HasEffectIfPrevious;
    [ShowIf(nameof(HasEffectIfPrevious))] public EffectIfPrevious EffectIfPrevious;
    [ShowIf(nameof(HasEffectIfPrevious)), SerializeField, ReadOnly] private string _effectIfPreviousPreviewText;
    public bool HasNextEffect;
    [ShowIf(nameof(HasNextEffect))] public EffectNext NextEffect;
    [ShowIf(nameof(HasNextEffect)), SerializeField, ReadOnly] private string _nextEffectPreviewText;

    [Header("Visuals")]
    public Sprite Icon;
    public PlayerAnimation Animation;
    public float Duration;

    public GetEffects Effects => _cardData.GetEffects(ActionLevel);
    public bool UseEffectIfPrevious(ActionType type, PlayerPosition position, out GetEffects effects)
    {
        if (EffectIfPrevious.RequiredType == type && EffectIfPrevious.RequiredPosition.HasFlag(position))
        {
            effects = EffectIfPrevious.Effects.GetEffects(ActionLevel);
            return true;
        }
        effects = new GetEffects();
        return false;
    }

    public ActionData(ActionType type = ActionType.None, CardRarity rarity = CardRarity.Rookie)
    {
        Name = type.ToString();
        Icon = null;
        Type = type;
        AssociatedRarity = rarity;
        AllowedPositions = PlayerPosition.Any;
        ActionLevel = 1;
        float baseHype = type switch
        {
            ActionType.Trick => 5f,
            ActionType.Pass => 2f,
            ActionType.Shot => 10f,
            _ => 2,
        };
        baseHype *= AssociatedRarity switch
        {
            CardRarity.Rookie => 2f,
            CardRarity.Career => 3f,
            CardRarity.AllStar => 4f,
            CardRarity.Signature => 5f,
            _ => 2f,
        };
        _cardData = new AppliedEffects(1, baseHype);
        HasEffectIfSequence = false;
        EffectIfSequence = new EffectIfSequence()
        {
            Requirements = SequenceRequirements.First,
            OfType = ActionType.None,
            Effects = new AppliedEffects(0, 0),
        };
        _effectIfSequencePreviewText = "";
        HasEffectIfPrevious = false;
        EffectIfPrevious = new EffectIfPrevious()
        {
            RequiredType = ActionType.None,
            RequiredPosition = PlayerPosition.Any,
            Effects = new AppliedEffects(0, 0),
        };
        _effectIfPreviousPreviewText = "";
        HasNextEffect = false;
        NextEffect = new EffectNext
        {
            AppliesTo = NextEffectAppliesTo.NextCardPlayed,
            RequiredType = ActionType.Trick,
            RequiredPosition = PlayerPosition.Any,
            Effects = new AppliedEffects(0, 0),
        };
        _nextEffectPreviewText = "";
        _cardText = "";
        _previewText = "";
        _cardSummary = "";
        Animation = type switch
        {
            ActionType.Trick => PlayerAnimation.BasicDribble,
            ActionType.Pass => PlayerAnimation.Pass,
            ActionType.Shot => PlayerAnimation.Shoot,
            _ => PlayerAnimation.BasicDribble,
        };
        Duration = 2;
    }

    public void UpdatePreviewText()
    {
        if (HasNextEffect) _nextEffectPreviewText = NextEffect.GetDisplayText(ActionLevel);
        if (HasEffectIfPrevious) _effectIfPreviousPreviewText = EffectIfPrevious.GetDisplayText(ActionLevel);
        if (HasEffectIfSequence) _effectIfSequencePreviewText = EffectIfSequence.GetDisplayText(ActionLevel);
        _previewText = GetDisplayText();
        _cardSummary = GetSummaryText();
    }

    public string GetDisplayText() => FormatDisplayText(string.IsNullOrWhiteSpace(_cardText) ? "Gain @Hype Hype" : _cardText);
    private string FormatDisplayText(string text)
    {
        var effects = Effects;
        text = text.Replace("@Hype", effects.HypeGain.ToString());
        text = text.Replace("@Energy", effects.EnergyGain.ToString());
        text = text.Replace("@Cost", effects.Cost.ToString());
        text = text.Replace("@Duration", Duration.ToString());
        text = text.Replace("@DrawCards", effects.DrawCards.ToString());
        text = text.Replace("@MultiplyHype", effects.MultiplyHype.ToString());
        text = text.Replace("@EffectIfSequence", EffectIfSequence.GetDisplayText(ActionLevel));
        text = text.Replace("@EffectIfPrevious", EffectIfPrevious.GetDisplayText(ActionLevel));
        text = text.Replace("@NextEffect", NextEffect.GetDisplayText(ActionLevel));
        text = text.Replace("@Effect", _cardData.GetDisplayText(ActionLevel, false));
        return text.TrimEnd('.');
    }

    public string GetSummaryText()
    {
        string text = FormatDisplayText("For @Cost cost, @Effect.");
        text += HasNextEffect ? $" {_nextEffectPreviewText}." : "";
        text += HasEffectIfPrevious ? $" {_effectIfPreviousPreviewText}." : "";
        text += HasEffectIfSequence ? $" {_effectIfSequencePreviewText}." : "";
        return text.TrimEnd('.');
    }
}

public struct GetEffects
{
    public float Cost;
    public float HypeGain;
    public float MultiplyHype;
    public float EnergyGain;
    public float DrawCards;
    public float RetriggerCard;

    public static GetEffects operator +(GetEffects a, GetEffects b)
    {
        return new GetEffects
        {
            Cost = a.Cost + b.Cost,
            HypeGain = a.HypeGain + b.HypeGain,
            MultiplyHype = a.MultiplyHype + b.MultiplyHype,
            EnergyGain = a.EnergyGain + b.EnergyGain,
            DrawCards = a.DrawCards + b.DrawCards,
            RetriggerCard = a.RetriggerCard + b.RetriggerCard,
        };
    }
}

[System.Serializable]
public struct AppliedEffects
{
    [SerializeField] private LevelableFloat _cost;
    [SerializeField] private LevelableFloat _hypeGain;
    [SerializeField] private LevelableFloat _multiplyHype;
    [SerializeField] private LevelableFloat _energyGain;
    [SerializeField] private LevelableFloat _drawCards;
    [SerializeField] private LevelableFloat _retriggerCard;

    public AppliedEffects(int cost, float hype)
    {
        _cost = new LevelableFloat(false, cost, cost, cost);
        _hypeGain = new LevelableFloat(true, hype, hype * 1.5f, hype * 2f);
        _energyGain = new LevelableFloat(false);
        _drawCards = new LevelableFloat(false);
        _multiplyHype = new LevelableFloat(false);
        _retriggerCard = new LevelableFloat(false);
    }

    public GetEffects GetEffects(int level)
    {
        return new GetEffects
        {
            Cost = _cost.GetValue(level),
            HypeGain = _hypeGain.GetValue(level),
            MultiplyHype = _multiplyHype.GetValue(level),
            EnergyGain = _energyGain.GetValue(level),
            DrawCards = _drawCards.GetValue(level),
            RetriggerCard = _retriggerCard.GetValue(level),
        };
    }

    public string GetDisplayText(int level, bool showCost = true)
    {
        string effectText = "";
        float hypeEffect = _hypeGain.GetValue(level);
        if (hypeEffect > 0) effectText += $"gain +{hypeEffect} Hype";
        else if (hypeEffect < 0) effectText += $"lose -{Mathf.Abs(hypeEffect)} Hype";
        float energyEffect = _energyGain.GetValue(level);
        if (energyEffect != 0)
        {
            if (hypeEffect != 0) effectText += " and ";
            if (energyEffect > 0) effectText += $"gain +{energyEffect} Energy";
            else if (energyEffect < 0) effectText += $"lose -{Mathf.Abs(energyEffect)} Energy";
        }
        float costEffect = showCost ? _cost.GetValue(level) : 0;
        if (costEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0) effectText += " and ";
            if (costEffect > 0) effectText += $"increase cost by {costEffect}";
            else if (costEffect < 0) effectText += $"reduce cost by {Mathf.Abs(costEffect)}";
        }
        float drawEffect = _drawCards.GetValue(level);
        if (drawEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0) effectText += " and ";
            if (drawEffect > 0) effectText += $"draw {drawEffect} cards";
            else if (drawEffect < 0) effectText += $"discard {Mathf.Abs(drawEffect)} cards";
        }
        float multiplyHypeEffect = _multiplyHype.GetValue(level);
        if (multiplyHypeEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0) effectText += " and ";
            effectText += $"multiply Hype gain by {multiplyHypeEffect}";
        }
        float retriggerEffect = _retriggerCard.GetValue(level);
        if (retriggerEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0 || multiplyHypeEffect != 0) effectText += " and ";
            effectText += $"trigger card an additional {(retriggerEffect > 1 ? retriggerEffect + " times" : "time")}";
        }
        return effectText;
    }
}

[System.Serializable]
public struct EffectIfPrevious
{
    public ActionType RequiredType;
    public PlayerPosition RequiredPosition;
    public AppliedEffects Effects;

    public EffectIfPrevious(ActionType type)
    {
        RequiredType = type;
        RequiredPosition = PlayerPosition.None;
        Effects = new AppliedEffects(0, 0);
    }

    public string GetDisplayText(int level)
    {
        string effectText = "If last card played was ";
        effectText += RequiredType switch
        {
            ActionType.Trick => "trick",
            ActionType.Pass => "pass",
            ActionType.Shot => "shot",
            _ => "any card",
        };
        effectText += RequiredPosition switch
        {
            PlayerPosition.PG => " from Point Guard, ",
            PlayerPosition.SG => " from Shooting Guard, ",
            PlayerPosition.SF => " from Small Forward, ",
            PlayerPosition.PF => " from Power Forward, ",
            PlayerPosition.C => " from Center, ",
            _ => ", ",
        };
        effectText += Effects.GetDisplayText(level);
        return effectText;
    }
}

[System.Serializable]
public struct EffectIfSequence
{
    public SequenceRequirements Requirements;
    public ActionType OfType;
    public AppliedEffects Effects;

    public string GetDisplayText(int level)
    {
        string cardType = OfType switch
        {
            ActionType.Trick => "trick",
            ActionType.Pass => "pass",
            ActionType.Shot => "shot",
            _ => "card",
        };
        string effectText = Requirements switch
        {
            SequenceRequirements.First => $"If first {cardType} in sequence, ",
            SequenceRequirements.Last => $"If last {cardType} in sequence, ",
            SequenceRequirements.NoTypePlayed => $"If no {(OfType == ActionType.Pass ? "passe" : cardType)}s are played this sequence, ",
            SequenceRequirements.ForEachOfType => $"For each {cardType} in sequence, ",
            _ => "",
        };
        effectText += Effects.GetDisplayText(level);
        return effectText;
    }
}

[System.Serializable]
public struct EffectNext
{
    public NextEffectAppliesTo AppliesTo;
    public ActionType RequiredType;
    public PlayerPosition RequiredPosition;
    public AppliedEffects Effects;
    [HideInInspector] public int Level;

    public GetEffects GetEffects() => Effects.GetEffects(Level);

    public string GetDisplayText() => GetDisplayText(Level);
    public string GetDisplayText(int level)
    {
        string effectText = "On next ";
        effectText += RequiredType switch
        {
            ActionType.Trick => "trick ",
            ActionType.Pass => "pass ",
            ActionType.Shot => "shot ",
            _ => "card ",
        };
        effectText += AppliesTo switch
        {
            NextEffectAppliesTo.NextCardPlayed => "played ",
            NextEffectAppliesTo.NextMatchingCardThisHand => "this sequence ",
            NextEffectAppliesTo.NextMatchingCardThisGame => "this game ",
            NextEffectAppliesTo.NextCardDrawn => "drawn ",
            _ => "",
        };
        effectText += RequiredPosition switch
        {
            PlayerPosition.PG => "by Point Guard ",
            PlayerPosition.SG => "by Shooting Guard ",
            PlayerPosition.SF => "by Small Forward ",
            PlayerPosition.PF => "by Power Forward ",
            PlayerPosition.C => "by Center ",
            _ => "",
        };
        effectText += Effects.GetDisplayText(level);
        return effectText;
    }
}

public enum NextEffectAppliesTo
{
    NextCardPlayed,
    NextMatchingCardThisHand,
    NextMatchingCardThisGame,
    NextCardDrawn,
}

public enum SequenceRequirements
{
    First,
    Last,
    NoTypePlayed,
    ForEachOfType,
}