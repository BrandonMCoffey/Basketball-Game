using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private SpriteRenderer _positionIndicator;
    [SerializeField] private Color _positionColor = Color.cyan;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private TMP_Text _actionText;

    public PlayerData PlayerData => _playerData;
    public Color PositionColor => _positionColor;

    private void OnValidate()
    {
        if (_positionIndicator == null) _positionIndicator = GetComponentInChildren<SpriteRenderer>();
        if (_positionIndicator != null) _positionIndicator.color = _positionColor;
    }

    private void Awake()
    {
        _actionText.text = "";
    }

    public void Place(Vector3 pos, PlayerData data)
    {
        transform.position = pos + Vector3.up * 0.01f;
        _playerData = data;
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
