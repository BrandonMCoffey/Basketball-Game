using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerActionUI _actionUI;
    [SerializeField] private PlayerControlsUI _playerControlsPrefab;
    [SerializeField] private float _uiHeightOffset = 1.0f;

    private List<PlayerControlsUI> _playerControlsUI;
    private int _selectedIndex = -1;

    private void Start()
    {
        _playerControlsUI = new List<PlayerControlsUI>(_playerManager.Players.Count);
        foreach (var player in _playerManager.Players)
        {
            var display = Instantiate(_playerControlsPrefab, transform);
            _playerControlsUI.Add(display);
        }
        UpdateDisplayTransforms();
        _playerManager.RefreshPlayers += UpdateDisplayTransforms;
    }

    private void UpdateDisplayTransforms()
    {
        for (int i = 0; i < _playerControlsUI.Count; i++)
        {
            _playerControlsUI[i].transform.position = Camera.main.WorldToScreenPoint(_playerManager.GetPlayerPosition(i) + Vector3.up * _uiHeightOffset);
            _playerControlsUI[i].Init(this, i);
        }
    }

    public void ToggleSelectPlayer(int index)
    {
        if (index ==  _selectedIndex)
        {
            _selectedIndex = -1;
            _actionUI.Hide();

            foreach (var display in _playerControlsUI)
            {
                display.SetSelected(false);
            }
        }
        else
        {
            _selectedIndex = index;
            _actionUI.ShowPlayer(_playerManager.Players[index]);

            for (int i = 0; i < _playerControlsUI.Count; i++)
            {
                _playerControlsUI[i].SetSelected(i == index);
            }
        }
    }
}
