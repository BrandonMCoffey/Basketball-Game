using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private List<AnimatedPlayer> _players;
    [SerializeField] private PlayerControlsUIManager _playerControlsPrefab;
    [SerializeField] private float _uiHeightOffset = 1.0f;

    private void Start()
    {
        foreach (var player in _players)
        {
            var display = Instantiate(_playerControlsPrefab, transform);
            display.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * _uiHeightOffset);
        }
    }
}
