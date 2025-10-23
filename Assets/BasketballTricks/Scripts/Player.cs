using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private SpriteRenderer _positionIndicator;
    [SerializeField] private Color _positionColor = Color.cyan;

    public PlayerData PlayerData => _playerData;
    public Color PositionColor => _positionColor;

    private void OnValidate()
    {
        if (_positionIndicator == null) _positionIndicator = GetComponentInChildren<SpriteRenderer>();
        if (_positionIndicator != null) _positionIndicator.color = _positionColor;
    }

    public void Place(Vector3 pos, PlayerData data)
    {
        transform.position = pos + Vector3.up * 0.01f;
        _playerData = data;
    }
}
