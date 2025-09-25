using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private bool _playerEnabled = true;
    [SerializeField] private string _playerName;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private TeamData _team;
    [SerializeField] private int _shootingStat;
    [SerializeField] private int _shootingPractice;
    [SerializeField] private int _dribblingStat;
    [SerializeField] private int _dribblingPractice;
    [SerializeField] private int _teamPlayStat;

    public bool PlayerEnabled { get => _playerEnabled; set => _playerEnabled = value; }
    public string PlayerName => _playerName;
    public Sprite PlayerSprite => _playerSprite;
    public Sprite TeamLogo => _team != null ? _team.TeamLogo : null;
    public int ShootingStat => Mathf.RoundToInt(_shootingStat * (1f + _shootingPractice * 0.01f));
    public int DribblingStat => Mathf.RoundToInt(_dribblingStat * (1f + _dribblingPractice * 0.01f));
    public int TeamPlayStat => _teamPlayStat;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_playerName)) _playerName = name;
        if (_shootingStat == 0 && _dribblingStat == 0 && _teamPlayStat == 0) RandomizeStats();
    }

    public void RandomizeStats()
    {
        _shootingStat = Random.Range(1, 99);
        _dribblingStat = Random.Range(1, 99);
        _teamPlayStat = Random.Range(1, 99);
    }

    public void IncrementDribblingPractice()
    {
        if (_dribblingPractice < 100) _dribblingPractice++;
    }

    public void IncrementShootingPractice()
    {
        if (_shootingPractice < 10) _shootingPractice++;
    }
}
