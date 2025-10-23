using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _playerName;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private TeamData _team;
    [SerializeField] private List<PlayerActionData> _availableActions;

    public string PlayerName => _playerName;
    public Sprite PlayerSprite => _playerSprite;
    public Sprite TeamLogo => _team != null ? _team.TeamLogo : null;
    public List<PlayerActionData> AllAvailableActions => _availableActions;
    public PlayerActionData GetAction(int index) => index < _availableActions.Count ? _availableActions[index] : null;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_playerName)) _playerName = name;
    }
}
