using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ImageDataMatcher : SerializedScriptableObject
{
    [SerializeField] private Dictionary<PlayerPosition, Color> _positionColors = new Dictionary<PlayerPosition, Color>();
    [SerializeField] private Color _noPenaltyHalo = Color.blue;
    [SerializeField] private Color _slowButConditionMetHalo = Color.Lerp(Color.black, Color.magenta, 0.4f);
    [SerializeField] private Color _conditionMetHalo = Color.magenta;
    [Space(20)]
    [SerializeField] private Sprite _defaultBackground;
    [SerializeField] private Dictionary<PlayerPosition, Sprite> _positionBackgrounds = new Dictionary<PlayerPosition, Sprite>();
    [SerializeField] private Sprite _defaultRarityGlow;
    [SerializeField] private Dictionary<CardRarity, Sprite> _rarityGlows = new Dictionary<CardRarity, Sprite>();
    [SerializeField] private Dictionary<ActionType, Sprite> _actionTypes = new Dictionary<ActionType, Sprite>();
    [SerializeField] private Sprite _defaultCostIcon;
    [SerializeField] private List<Sprite> _costIconsAtIndexCost = new List<Sprite>();

    public Color GetPositionColor(PlayerPosition position)
    {
        return _positionColors.ContainsKey(position) ? _positionColors[position] : Color.white;
    }
    public Sprite GetPositionBackground(PlayerPosition position)
    {
        if (!_positionBackgrounds.ContainsKey(position)) return _defaultBackground;
        return _positionBackgrounds[position] != null ? _positionBackgrounds[position] : _defaultBackground;
    }
    public Sprite GetRarityGlow(CardRarity rarity)
    {
        if (!_rarityGlows.ContainsKey(rarity)) return _defaultRarityGlow;
        return _rarityGlows[rarity] != null ? _rarityGlows[rarity] : _defaultRarityGlow;
    }
    public Sprite GetActionType(ActionType actionType) => _actionTypes.ContainsKey(actionType) ? _actionTypes[actionType] : null;
    public Sprite GetCostIcon(int cost) => !UseCostText(cost) ? _costIconsAtIndexCost[Mathf.Max(0, cost)] : _defaultCostIcon;
    public bool UseCostText(int cost) => cost >= _costIconsAtIndexCost.Count;

    [Button]
    private void CreateDictionaries()
    {
        _positionColors ??= new Dictionary<PlayerPosition, Color>();
        if (!_positionColors.ContainsKey(PlayerPosition.PG)) _positionColors.Add(PlayerPosition.PG, Color.green);
        if (!_positionColors.ContainsKey(PlayerPosition.PF)) _positionColors.Add(PlayerPosition.PF, Color.cyan);
        if (!_positionColors.ContainsKey(PlayerPosition.C)) _positionColors.Add(PlayerPosition.C, Color.yellow);
        if (!_positionColors.ContainsKey(PlayerPosition.SG)) _positionColors.Add(PlayerPosition.SG, Color.Lerp(Color.red, Color.yellow, 0.5f));
        if (!_positionColors.ContainsKey(PlayerPosition.SF)) _positionColors.Add(PlayerPosition.SF, Color.red);

        _positionBackgrounds ??= new Dictionary<PlayerPosition, Sprite>();
        if (!_positionBackgrounds.ContainsKey(PlayerPosition.PG)) _positionBackgrounds.Add(PlayerPosition.PG, null);
        if (!_positionBackgrounds.ContainsKey(PlayerPosition.PF)) _positionBackgrounds.Add(PlayerPosition.PF, null);
        if (!_positionBackgrounds.ContainsKey(PlayerPosition.C)) _positionBackgrounds.Add(PlayerPosition.C, null);
        if (!_positionBackgrounds.ContainsKey(PlayerPosition.SG)) _positionBackgrounds.Add(PlayerPosition.SG, null);
        if (!_positionBackgrounds.ContainsKey(PlayerPosition.SF)) _positionBackgrounds.Add(PlayerPosition.SF, null);

        _rarityGlows ??= new Dictionary<CardRarity, Sprite>();
        if (!_rarityGlows.ContainsKey(CardRarity.Rookie)) _rarityGlows.Add(CardRarity.Rookie, null);
        if (!_rarityGlows.ContainsKey(CardRarity.Career)) _rarityGlows.Add(CardRarity.Career, null);
        if (!_rarityGlows.ContainsKey(CardRarity.AllStar)) _rarityGlows.Add(CardRarity.AllStar, null);
        if (!_rarityGlows.ContainsKey(CardRarity.Signature)) _rarityGlows.Add(CardRarity.Signature, null);

        _actionTypes ??= new Dictionary<ActionType, Sprite>();
        if (!_actionTypes.ContainsKey(ActionType.Trick)) _actionTypes.Add(ActionType.Trick, null);
        if (!_actionTypes.ContainsKey(ActionType.Pass)) _actionTypes.Add(ActionType.Pass, null);
        if (!_actionTypes.ContainsKey(ActionType.Shot)) _actionTypes.Add(ActionType.Shot, null);
    }
}