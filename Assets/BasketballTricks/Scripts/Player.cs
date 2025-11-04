using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerArt _playerArt;
    [SerializeField] private SpriteRenderer _positionIndicator;
    [SerializeField] private Color _positionColor = Color.cyan;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private TMP_Text _actionText;
    [SerializeField] private Transform _basketballSocket;
    [SerializeField] private Vector3 _basketballOffset;

    public PlayerData PlayerData => _playerData;
    public Color PositionColor => _positionColor;
    public Vector3 BasketballPosition => _basketballSocket.TransformPoint(_basketballOffset);
    public PlayerArt PlayerArt => _playerArt;

    private void OnValidate()
    {
        if (_positionIndicator == null) _positionIndicator = GetComponentInChildren<SpriteRenderer>();
        if (_positionIndicator != null) _positionIndicator.color = _positionColor;
    }

    private void Awake()
    {
        _actionText.text = "";
    }

    public void SetAnimation(string stateName, float fade = 0f)
    {
        _animator.CrossFade(stateName, 0f);
    }

    public void UpdateCanPlace(Vector3 pos, bool canPlace)
    {
        if (_positionIndicator != null) _positionIndicator.enabled = canPlace;
        transform.position = pos + Vector3.up * 0.01f;
    }

    public void Place(PlayerData data)
    {
        _playerData = data;
        SetAnimation("Idle");
    }

    public void FaceOtherPlayer(Player otherPlayer, float passDuration)
    {
        var pos = transform.position;
        var otherPos = otherPlayer.transform.position;
        (pos.y, otherPos.y) = (otherPos.y, pos.y);
        transform.DOLookAt(otherPos, passDuration * 0.5f);
        otherPlayer.transform.DOLookAt(pos, passDuration * 0.5f);
    }

    public void FacePosition(Vector3 pos, float passDuration)
    {
        pos.y = transform.position.y;
        transform.DOLookAt(pos, passDuration * 0.5f);
    }

    public void SetActionText(string text, float duration)
    {
        _actionText.text = text;
        StartCoroutine(ActionTextRoutine(duration));
    }

    private IEnumerator ActionTextRoutine(float duration)
    {
        yield return new WaitForSeconds(duration - 0.1f);
        _actionText.text = "";
    }

    public void EmitParticles()
    {
        _particles.Emit(30);
    }
}

[System.Serializable]
public enum PlayerAnimation
{
    Idle,
    Placing,
    Basketball
}