using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameCard _cardData;
    [SerializeField] private float _animationCrossfadeDuration = 0.25f;
    [SerializeField] private PlayerPosition _position;
    [SerializeField] private Color _positionColor = Color.cyan;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerArt _playerArt;
    [SerializeField] private SpriteRenderer _positionIndicator;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private TMP_Text _actionText;
    [SerializeField] private Transform _basketballSocket;
    [SerializeField] private Vector3 _basketballOffset;

    public GameCard CardData => _cardData;
    public Color PositionColor => _positionColor;
    public Vector3 BasketballPosition => _basketballSocket.TransformPoint(_basketballOffset);
    public PlayerArt PlayerArt => _playerArt;
    public Vector3 HeadPosition => _playerArt.Head.position;

    private float _fadeTimer;

    private void OnValidate()
    {
        if (_positionIndicator == null) _positionIndicator = GetComponentInChildren<SpriteRenderer>();
        if (_positionIndicator != null) _positionIndicator.color = _positionColor;
    }

    private void Awake()
    {
        _actionText.text = "";
    }

    private void Update()
    {
        _fadeTimer += Time.deltaTime;
    }

    public void SetAnimation(PlayerAnimation anim) => SetAnimation(anim, _animationCrossfadeDuration);
    public void SetAnimation(PlayerAnimation anim, float fade)
    {
        //Debug.Log($"Play Anim {anim.ToString()} for {fade} fade. {_fadeTimer} seconds since last fade.");
        _fadeTimer = 0;
        _animator.CrossFadeInFixedTime(anim.ToString(), fade);
    }

    public void UpdateCanPlace(Vector3 pos, bool canPlace)
    {
        if (_positionIndicator != null) _positionIndicator.enabled = canPlace;
        transform.position = pos + Vector3.up * 0.01f;
    }

    public void Place(GameCard data)
    {
        _cardData = data;
        if (data.PlayerData.HasArtData) _playerArt.SetPlayerArt(data.PlayerData.ArtData);
        SetAnimation(PlayerAnimation.Idle);
    }

    public void FaceOtherPlayer(Player otherPlayer, float duration)
    {
        var pos = transform.position;
        var otherPos = otherPlayer.transform.position;
        (pos.y, otherPos.y) = (otherPos.y, pos.y);
        transform.DOLookAt(otherPos, duration);
        otherPlayer.transform.DOLookAt(pos, duration);
    }

    public void FacePosition(Vector3 pos, float duration)
    {
        pos.y = transform.position.y;
        transform.DOLookAt(pos, duration);
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
    // Basic animations (0-9)
    Idle = 1,
    IdleHold = 2,

    Dangle = 5,

    // Passes (10-99)
    Pass = 10,
    Catch = 20,

    // Tricks (100-199)
    BasicDribble = 100,
    SpiderDribble = 101,
    FingerSpin = 120,

    // Shots (200-299)
    Shoot = 200,
}