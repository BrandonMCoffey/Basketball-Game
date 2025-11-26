using UnityEngine;
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

    public string PlayerName => _playerName;
    public Sprite PlayerSprite => _playerSprite;
    public bool HasArtData => _customPlayerArt || _randomHairData != null;
    public PlayerArtData ArtData => _customPlayerArt ? _artData : _randomHairData.GetData();
    public bool IsNaturalPosition(PlayerPosition position) => _naturalPosition.HasFlag(position);

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
            PlayerPosition.PointGuard => "PG",
            PlayerPosition.ShootingGuard => "SG",
            PlayerPosition.SmallForward => "SF",
            PlayerPosition.PowerForward => "PF",
            PlayerPosition.Center => "C",
            _ => "NA"
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
    Any = PointGuard | ShootingGuard | SmallForward | PowerForward | Center,
    PointGuard = 1 << 0,
    ShootingGuard = 1 << 1,
    SmallForward = 1 << 2,
    PowerForward = 1 << 3,
    Center = 1 << 4,
}