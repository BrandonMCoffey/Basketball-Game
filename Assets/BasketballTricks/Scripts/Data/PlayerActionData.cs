using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionData", menuName = "BasketballTricks/PlayerActionData", order = 1)]
public class PlayerActionData : ScriptableObject
{
    [SerializeField] private string _name = "Action";
    [SerializeField] private float _duration = 1;
    [SerializeField] private float _points = 5;
    [SerializeField] private float _mult = 0;
    [SerializeField] private Sprite _icon;
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private float _animationDuration;

    public string Name => _name;
    public float Duration => _duration;
    public float Score => _points;
    public float Mult => _mult;
    public Sprite Icon => _icon;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_name)) _name = name;
        if (_duration <= 0f) _duration = 1f;
        _animationDuration = _animation != null ? _animation.length : 0;
    }
}

public enum ActionType
{
    Shot,
    Pass,
    Trick
}

public enum ActionCategory
{

}