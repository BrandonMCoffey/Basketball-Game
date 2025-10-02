using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private PlayerData _data;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _shootingStat;
    [SerializeField] private TMP_Text _dribblingStat;
    [SerializeField] private TMP_Text _teamPlayStat;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _teamImage;

    protected int _index;

    public void SetIndex(int index)
    {
        _index = index;
    }

    private void OnValidate()
    {
        if (_data != null) UpdateVisuals();
    }

    public void SetData(PlayerData data)
    {
        _data = data;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        _playerName.text = _data.PlayerName;
        _shootingStat.text = $"{_data.ShootingStat}";
        _dribblingStat.text = $"{_data.DribblingStat}";
        _teamPlayStat.text = $"{_data.TeamPlayStat}";
        if (_data.PlayerSprite != null) _playerImage.sprite = _data.PlayerSprite;
        if (_data.TeamLogo != null) _teamImage.sprite = _data.TeamLogo;
    }
}
