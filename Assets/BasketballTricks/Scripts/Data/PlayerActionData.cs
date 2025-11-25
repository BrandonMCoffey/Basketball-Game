using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using CoffeyUtils;

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
        if (_data.HasEffectIfPrevious) _data.EffectIfPreviousPreviewText = _data.EffectIfPrevious.GetDisplayText(_data.ActionLevel);
        if (_data.HasEffectIfSequence) _data.EffectIfSequencePreviewText = _data.EffectIfSequence.GetDisplayText(_data.ActionLevel);
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
    public LevelableFloat Cost;
    public int ActionLevel;
    [Header("Effects")]
    public LevelableFloat HypeGain;
    public LevelableFloat EnergyGain;
    public LevelableFloat DrawCards;
    public LevelableFloat MultiplyHype;
    public bool HasEffectIfSequence;
    [ShowIf(nameof(HasEffectIfSequence))] public EffectIfSequence EffectIfSequence;
    [ShowIf(nameof(HasEffectIfSequence)), ReadOnly] public string EffectIfSequencePreviewText;
    public bool HasEffectIfPrevious;
    [ShowIf(nameof(HasEffectIfPrevious))] public EffectIfPrevious EffectIfPrevious;
    [ShowIf(nameof(HasEffectIfPrevious)), ReadOnly] public string EffectIfPreviousPreviewText;
    public bool HasNextEffect;
    [ShowIf(nameof(HasNextEffect))] public EffectNext NextEffect;
    [ShowIf(nameof(HasNextEffect)), ReadOnly] public string NextEffectPreviewText;
    [Header("Visuals")]
    public Sprite Icon;
    public PlayerAnimation Animation;
    public float Duration;

    public ActionData(ActionType type = ActionType.None)
    {
        Name = type.ToString();
        Icon = null;
        Type = type;
        AssociatedRarity = CardRarity.None;
        AllowedPositions = PlayerPosition.Any;
        Cost = new LevelableFloat(false, 1);
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
        HypeGain = new LevelableFloat(true, baseHype, baseHype * 1.5f, baseHype * 2f);
        EnergyGain = new LevelableFloat(false);
        DrawCards = new LevelableFloat(false);
        MultiplyHype = new LevelableFloat(false);
        HasEffectIfSequence = false;
        EffectIfSequence = new EffectIfSequence()
        {
            Requirements = SequenceRequirements.First,
            OfType = ActionType.None,
            HypeGain = new LevelableFloat(false),
            EnergyGain = new LevelableFloat(false),
            AdjustCost = new LevelableFloat(false),
            DrawCards = new LevelableFloat(false),
            MultiplyHype = new LevelableFloat(false),
        };
        EffectIfSequencePreviewText = "";
        HasEffectIfPrevious = false;
        EffectIfPrevious = new EffectIfPrevious()
        {
            RequiredType = ActionType.None,
            RequiredPosition = PlayerPosition.Any,
            HypeGain = new LevelableFloat(false),
            EnergyGain = new LevelableFloat(false),
            AdjustCost = new LevelableFloat(false),
            DrawCards = new LevelableFloat(false),
            MultiplyHype = new LevelableFloat(false),
        };
        EffectIfPreviousPreviewText = "";
        HasNextEffect = false;
        NextEffect = new EffectNext
        {
            AppliesTo = NextEffectAppliesTo.NextCardPlayed,
            RequiredType = ActionType.Trick,
            RequiredPosition = PlayerPosition.Any,
            AddHypeToCard = new LevelableFloat(false),
            AddEnergyGainToCard = new LevelableFloat(false),
            AdjustCardCost = new LevelableFloat(false),
            DrawCards = new LevelableFloat(false),
            MultiplyHype = new LevelableFloat(false),
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
        // Add more if you want
        string text = CardText;
        text = text.Replace("@Hype", HypeGain.GetValue(ActionLevel).ToString("F1"));
        text = text.Replace("@Energy", EnergyGain.GetValue(ActionLevel).ToString("F1"));
        text = text.Replace("@Cost", Cost.GetValue(ActionLevel).ToString("F1"));
        text = text.Replace("@Duration", Duration.ToString("F1"));
        text = text.Replace("@DrawCards", DrawCards.GetValue(ActionLevel).ToString("F0"));
        text = text.Replace("@MultiplyHype", MultiplyHype.GetValue(ActionLevel).ToString("F1"));
        text = text.Replace("@EffectIfSequence", EffectIfSequence.GetDisplayText(ActionLevel));
        text = text.Replace("@EffectIfPrevious", EffectIfPrevious.GetDisplayText(ActionLevel));
        text = text.Replace("@NextEffect", NextEffect.GetDisplayText(ActionLevel));
        return text;
    }
}

[System.Serializable]
public struct EffectIfPrevious
{
    public ActionType RequiredType;
    public PlayerPosition RequiredPosition;
    public LevelableFloat HypeGain;
    public LevelableFloat EnergyGain;
    public LevelableFloat AdjustCost;
    public LevelableFloat DrawCards;
    public LevelableFloat MultiplyHype;

    public string GetDisplayText(int level)
    {
        string effectText = "If last card played was ";
        effectText += RequiredType switch
        {
            ActionType.Trick => "trick, ",
            ActionType.Pass => "pass, ",
            ActionType.Shot => "shot, ",
            _ => "card, ",
        };
        effectText += RequiredPosition switch
        {
            PlayerPosition.PointGuard => "from Point Guard ",
            PlayerPosition.ShootingGuard => "from Shooting Guard ",
            PlayerPosition.SmallForward => "from Small Forward ",
            PlayerPosition.PowerForward => "from Power Forward ",
            PlayerPosition.Center => "from Center ",
            _ => "",
        };
        float hypeEffect = HypeGain.GetValue(level);
        if (hypeEffect > 0) effectText += $"gain +{hypeEffect} Hype";
        else if (hypeEffect < 0) effectText += $"lose -{Mathf.Abs(hypeEffect)} Hype";
        float energyEffect = EnergyGain.GetValue(level);
        if (energyEffect != 0)
        {
            if (hypeEffect != 0) effectText += " and ";
            if (energyEffect > 0) effectText += $"gain +{energyEffect} Energy";
            else if (energyEffect < 0) effectText += $"lose -{Mathf.Abs(energyEffect)} Energy";
        }
        float costEffect = AdjustCost.GetValue(level);
        if (costEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0) effectText += " and ";
            if (costEffect > 0) effectText += $"increase cost by {costEffect}";
            else if (costEffect < 0) effectText += $"reduce cost by {Mathf.Abs(costEffect)}";
        }
        float drawEffect = DrawCards.GetValue(level);
        if (drawEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0) effectText += " and ";
            if (drawEffect > 0) effectText += $"draw {drawEffect} cards";
            else if (drawEffect < 0) effectText += $"discard {Mathf.Abs(drawEffect)} cards";
        }
        float multiplyHypeEffect = MultiplyHype.GetValue(level);
        if (multiplyHypeEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0) effectText += " and ";
            effectText += $"multiply Hype gain by {multiplyHypeEffect}";
        }
        return effectText;
    }
}

[System.Serializable]
public struct EffectIfSequence
{
    public SequenceRequirements Requirements;
    public ActionType OfType;
    public LevelableFloat HypeGain;
    public LevelableFloat EnergyGain;
    public LevelableFloat AdjustCost;
    public LevelableFloat DrawCards;
    public LevelableFloat MultiplyHype;

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
            SequenceRequirements.ForEachOfType => $"For every {cardType} in sequence, ",
            _ => "",
        };
        float hypeEffect = HypeGain.GetValue(level);
        if (hypeEffect > 0) effectText += $"gain +{hypeEffect} Hype";
        else if (hypeEffect < 0) effectText += $"lose -{Mathf.Abs(hypeEffect)} Hype";
        float energyEffect = EnergyGain.GetValue(level);
        if (energyEffect != 0)
        {
            if (hypeEffect != 0) effectText += " and ";
            if (energyEffect > 0) effectText += $"gain +{energyEffect} Energy";
            else if (energyEffect < 0) effectText += $"lose -{Mathf.Abs(energyEffect)} Energy";
        }
        float costEffect = AdjustCost.GetValue(level);
        if (costEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0) effectText += " and ";
            if (costEffect > 0) effectText += $"increase cost by {costEffect}";
            else if (costEffect < 0) effectText += $"reduce cost by {Mathf.Abs(costEffect)}";
        }
        float drawEffect = DrawCards.GetValue(level);
        if (drawEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0) effectText += " and ";
            if (drawEffect > 0) effectText += $"draw {drawEffect} cards";
            else if (drawEffect < 0) effectText += $"discard {Mathf.Abs(drawEffect)} cards";
        }
        float multiplyHypeEffect = MultiplyHype.GetValue(level);
        if (multiplyHypeEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0) effectText += " and ";
            effectText += $"multiply Hype gain by {multiplyHypeEffect}";
        }
        return effectText;
    }
}

[System.Serializable]
public struct EffectNext
{
    public NextEffectAppliesTo AppliesTo;
    public ActionType RequiredType;
    public PlayerPosition RequiredPosition;
    public LevelableFloat AddHypeToCard;
    public LevelableFloat AddEnergyGainToCard;
    public LevelableFloat AdjustCardCost;
    public LevelableFloat DrawCards;
    public LevelableFloat MultiplyHype;
    public LevelableFloat Retriggers;

    public string GetDisplayText(int level)
    {
        string effectText = "Next ";
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
            PlayerPosition.PointGuard => "by Point Guard ",
            PlayerPosition.ShootingGuard => "by Shooting Guard ",
            PlayerPosition.SmallForward => "by Small Forward ",
            PlayerPosition.PowerForward => "by Power Forward ",
            PlayerPosition.Center => "by Center ",
            _ => "",
        };
        float hypeEffect = AddHypeToCard.GetValue(level);
        if (hypeEffect > 0) effectText += $"gains +{hypeEffect} Hype";
        else if (hypeEffect < 0) effectText += $"loses -{Mathf.Abs(hypeEffect)} Hype";
        float energyEffect = AddEnergyGainToCard.GetValue(level);
        if (energyEffect != 0)
        {
            if (hypeEffect != 0) effectText += " and ";
            if (energyEffect > 0) effectText += $"gains +{energyEffect} Energy";
            else if (energyEffect < 0) effectText += $"loses -{Mathf.Abs(energyEffect)} Energy";
        }
        float costEffect = AdjustCardCost.GetValue(level);
        if (costEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0) effectText += " and ";
            if (costEffect > 0) effectText += $"costs {costEffect} more Energy";
            else if (costEffect < 0) effectText += $"costs {Mathf.Abs(costEffect)} less Energy";
        }
        float drawEffect = DrawCards.GetValue(level);
        if (drawEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0) effectText += " and ";
            if (drawEffect > 0) effectText += $"draw {drawEffect} cards";
            else if (drawEffect < 0) effectText += $"discard {Mathf.Abs(drawEffect)} cards";
        }
        float multiplyHypeEffect = MultiplyHype.GetValue(level);
        if (multiplyHypeEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0) effectText += " and ";
            effectText += $"multiplies Hype gain by {multiplyHypeEffect}";
        }
        float retriggerEffect = Retriggers.GetValue(level);
        if (retriggerEffect != 0)
        {
            if (hypeEffect != 0 || energyEffect != 0 || costEffect != 0 || drawEffect != 0 || multiplyHypeEffect != 0) effectText += " and ";
            effectText += $"triggers an additional {(retriggerEffect > 1 ? retriggerEffect + " times" : "time")}";
        }
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