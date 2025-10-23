using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerControlsUIManager _playerControlsPrefab;
    [SerializeField] private float _uiHeightOffset = 1.0f;

    private List<PlayerControlsUIManager> _playerControlsUI;

    private void Start()
    {
        _playerControlsUI = new List<PlayerControlsUIManager>(_playerManager.Players.Count);
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
            var player = _playerManager.GetPlayer(i);
            _playerControlsUI[i].transform.position = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * _uiHeightOffset);
            _playerControlsUI[i].Init(player);
        }
    }
}
