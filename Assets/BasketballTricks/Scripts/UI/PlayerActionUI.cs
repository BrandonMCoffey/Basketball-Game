using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField] private SlideInPanel _slideInPanel;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Image _playerImage;
    [SerializeField] private List<PlayerActionVisuals> _actionVisuals;

    private Player _player;
    private PlayerData _playerData;

    public void ShowPlayer(Player player)
    {
        _slideInPanel.SetShown(true);
        _player = player;
        _playerData = player != null ? player.PlayerData : null;
        UpdateData();
    }

    public void Hide()
    {
        _slideInPanel.SetShown(false);
    }

    private void UpdateData()
    {
        if (_playerName != null) _playerName.text = _playerData != null ? _playerData.PlayerName : "Player";
        if (_playerImage != null) _playerImage.sprite = _playerData != null ? _playerData.PlayerSprite : null;
        for (int i = 0; i < _actionVisuals.Count; i++)
        {
            _actionVisuals[i].SetData(_playerData != null ? _playerData.GetAction(i) : new ActionData());
        }
    }
}
