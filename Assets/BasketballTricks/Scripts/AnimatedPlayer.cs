using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPlayer : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    public PlayerData PlayerData => _playerData;
}
