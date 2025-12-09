using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _playerName;
    [SerializeField] private string _playerNumber;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private PlayerPosition _naturalPosition;

    [SerializeField] private bool _customPlayerArt;
    [SerializeField, HideIf(nameof(_customPlayerArt))] private RandomPlayerArtData _randomHairData;
    [SerializeField, ShowIf(nameof(_customPlayerArt))] private PlayerArtData _artData;

    [Space(25)]
    [SerializeField] private List<string> _cardBackImportantData;
    [SerializeField] private List<CardBackColumnData> _cardBackData;

    public string PlayerName => _playerName;
    public string PlayerNumber => _playerNumber;
    public Sprite PlayerSprite => _playerSprite;
    public bool HasArtData => _customPlayerArt || _randomHairData != null;
    public PlayerArtData ArtData => _customPlayerArt ? _artData : _randomHairData.GetData();
    public PlayerPosition NaturalPosition => _naturalPosition;
    public bool IsNaturalPosition(PlayerPosition position) => _naturalPosition.HasFlag(position);
    public List<string> CardBackImportantData => _cardBackImportantData;
    public List<CardBackColumnData> CardBackData => _cardBackData;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_playerName))
        {
            var split = name.Split('_');
            if (split.Length >= 2)
            {
                _playerNumber = split[0];
                _playerName = split[1];
            }
            else
            {
                _playerName = name;
            }
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

    public static string PositionToString(PlayerPosition position)
    {
        return position switch
        {
            PlayerPosition.PG => "PG",
            PlayerPosition.SG => "SG",
            PlayerPosition.SF => "SF",
            PlayerPosition.PF => "PF",
            PlayerPosition.C => "C",
            _ => "NA"
        };
    }

    [Button]
    private void SetDefaultCardBackData()
    {
        _cardBackImportantData = new List<string>
        {
            "BORN: ",
            "HEIGHT: ",
            "WEIGHT: ",
            "JERSEY #: " + _playerNumber,
            "DRAFTED: ",
        };
        _cardBackData = new List<CardBackColumnData>
        {
            new CardBackColumnData("YEAR", "25-26", "TOTAL"),
            new CardBackColumnData("TEAM"),
            new CardBackColumnData("G"),
            new CardBackColumnData("FG%"),
            new CardBackColumnData("FT%"),
            new CardBackColumnData("3PM"),
            new CardBackColumnData("RPG"),
            new CardBackColumnData("APG"),
            new CardBackColumnData("STL"),
            new CardBackColumnData("BLK"),
            new CardBackColumnData("PTS"),
            new CardBackColumnData("PPG"),
        };
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

[System.Flags]
public enum PlayerPosition
{
    None = 0,
    Any = PG | SG | SF | PF | C,
    PG = 1 << 0,
    SG = 1 << 1,
    SF = 1 << 2,
    PF = 1 << 3,
    C = 1 << 4,
}

[System.Serializable]
public struct CardBackColumnData
{
    public string Title;
    public string Row1;
    public string Row2;

    public CardBackColumnData(string title, string row1 = "", string row2 = "")
    {
        Title = title;
        Row1 = row1;
        Row2 = row2;
    }
}