using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDestination : MonoBehaviour
{
    [SerializeField] private AnimatedPlayer _playerPrefab;
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private SpriteRenderer _renderer;

    public Color Color => _color;
    public AnimatedPlayer SpawnedPlayer { get; private set; }

    private void OnValidate()
    {
        if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
        if (_renderer != null) _renderer.color = _color;
    }

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        SpawnedPlayer = Instantiate(_playerPrefab, transform.position, transform.rotation, transform);
    }
}
