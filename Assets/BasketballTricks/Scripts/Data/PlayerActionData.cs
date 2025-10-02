using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionData", menuName = "BasketballTricks/PlayerActionData", order = 1)]
public class PlayerActionData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _duration;
    [SerializeField] private Sprite _icon;

    public string Name => _name;
    public float Duration => _duration;
    public Sprite Icon => _icon;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_name)) _name = name;
        if (_duration <= 0f) _duration = 1f;
    }
}
