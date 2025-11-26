using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerActionUI _actionUI;
    [SerializeField] private PlayerControlsUI _playerControlsPrefab;
    [SerializeField] private float _multY = 0.9f;

    private PlayerManager _playerManager;
    private List<PlayerControlsUI> _playerControlsUI;
    private int _selectedIndex = -1;

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        _playerControlsUI = new List<PlayerControlsUI>(_playerManager.Players.Count);
        foreach (var player in _playerManager.Players)
        {
            var display = Instantiate(_playerControlsPrefab, transform);
            _playerControlsUI.Add(display);
        }
        UpdateDisplayTransforms();
        PlayerManager.RefreshPlayers += UpdateDisplayTransforms;
    }

    private void UpdateDisplayTransforms()
    {
        for (int i = 0; i < _playerControlsUI.Count; i++)
        {
            var pos = _playerManager.PlayerPosToMouse(i);
            pos.y *= _multY;
            _playerControlsUI[i].transform.position = pos;
            _playerControlsUI[i].Init(this, i, _playerManager.GetPlayer(i));
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
