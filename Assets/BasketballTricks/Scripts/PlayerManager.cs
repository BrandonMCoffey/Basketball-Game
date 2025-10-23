using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private List<Player> _players = new List<Player>();
    [SerializeField] private LayerMask _floorMask = 1;

    private void OnValidate()
    {
        if (_players.Count == 0)
        {
            _players = GetComponentsInChildren<Player>().ToList();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public bool AttemptPlacePlayer(PlayerData data, Vector2 mousePos)
    {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hitInfo, 100f, _floorMask))
        {
            foreach (var player in _players)
            {
                if (player.PlayerData == null)
                {
                    player.Place(hitInfo.point, data);
                    return true;
                }
            }
        }
        return false;
    }
}
