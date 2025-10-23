using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField] private SlideInPanel _playerActionPanel;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Image _playerImage;
    [SerializeField] private List<PlayerActionVisuals> _actionVisuals;

    private Player _player;
    private PlayerData _playerData;

    public void ShowPlayer(Player player)
    {
        _playerActionPanel.SetShown(true);
        _player = player;
        _playerData = player != null ? player.PlayerData : null;
        UpdateData();
    }

    public void Hide()
    {
        _playerActionPanel.SetShown(false);
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

    public void SelectAction(int index)
    {
        PlayerManager.Instance.AddAction(_player, index);
    }
}
