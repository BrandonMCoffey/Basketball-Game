using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ImageDataMatcher : SerializedScriptableObject
{
    [SerializeField] private Sprite _defaultBackground;
    [SerializeField] private Dictionary<PlayerPosition, Sprite> _positionBackgrounds = new Dictionary<PlayerPosition, Sprite>();
    [SerializeField] private Sprite _defaultRarityGlow;
    [SerializeField] private Dictionary<CardRarity, Sprite> _rarityGlows = new Dictionary<CardRarity, Sprite>();

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

    [Button]
    private void CreateDictionaries()
    {
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
    }
}