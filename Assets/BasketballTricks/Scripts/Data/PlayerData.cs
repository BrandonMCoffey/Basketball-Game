using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _playerName;
    [SerializeField] private string _cardTitle;
    [SerializeField] private CardRarity _rarity;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private PlayerPosition _naturalPosition;
    [SerializeField] private PlayerPosition _secondaryNaturalPosition;

    [SerializeField] private bool _customPlayerArt;
    [SerializeField, HideIf(nameof(_customPlayerArt))] private RandomPlayerArtData _randomHairData;
    [SerializeField, ShowIf(nameof(_customPlayerArt))] private PlayerArtData _artData;

    [SerializeField] private List<CustomPlayerAction> _actions = new List<CustomPlayerAction>();

    public string PlayerName => _playerName;
    public Sprite PlayerSprite => _playerSprite;
    public bool HasArtData => _customPlayerArt || _randomHairData != null;
    public PlayerArtData ArtData => _customPlayerArt ? _artData : _randomHairData.GetData();

    public ActionData GetAction(int index)
    {
        if (index >= _actions.Count || index < 0) return new ActionData();
        return _actions[index].GetData();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_playerName)) _playerName = name;
        for (int i = 0; i < _actions.Count; i++)
        {
            _actions[i].DataPreview = _actions[i].GetData();
        }
    }

    [Button]
    private void UpdateCustomPlayerArtFromRandom()
    {
        if (_randomHairData != null)
        {
            _artData = _randomHairData.GetData();
        }
    }
}

[System.Serializable]
public class CustomPlayerAction
{
    [SerializeField] private PlayerActionData _action;
    [SerializeField] private int _count = 1;
    [SerializeField] private float _actionCostOverride = -1;
    [SerializeField] private float _actionHypeOverride = -1;
    [SerializeField] private float _actionDurationOverride = -1;
    [SerializeField, Range(0, 1)] private float _actionAccuracyOverride = 0;
    [ReadOnly] public ActionData DataPreview;

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

[System.Serializable]
public struct PlayerArtData
{
    public Color SkinColor;
    public Color HairColor;
    public Texture JerseyTexture;
    public GameObject HairPrefab;
    public GameObject FacialHairPrefab;
    public GameObject AccessoryPrefab;
}

public enum CardRarity
{
    None,
    Rookie,
    Career,
    AllStar,
    Signature
}

public enum PlayerPosition
{
    None,
    PointGuard,
    ShootingGuard,
    ShootingForward,
    PowerForward,
    Center
}