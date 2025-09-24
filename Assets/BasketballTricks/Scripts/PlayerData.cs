using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "BasketballTricks/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _playerName;
    [SerializeField] private int _stat1;
    [SerializeField] private int _stat2;
    [SerializeField] private int _stat3;

    public string PlayerName => _playerName;
    public int Stat1 => _stat1;
    public int Stat2 => _stat2;
    public int Stat3 => _stat3;

    public void RandomizeStats()
    {
        _stat1 = Random.Range(1, 99);
        _stat2 = Random.Range(1, 99);
        _stat3 = Random.Range(1, 99);
    }
}
