using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardVisuals : MonoBehaviour
{
    [SerializeField] protected GameCard _card;

    [Header("UI References")]
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _playerName2;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Sprite _defaultPlayerImage;
    [SerializeField] private Sprite _defaultTeamImage;
    [SerializeField] private List<PlayerActionVisuals> _actionVisuals;

    [Header("Flip References")]
    [SerializeField] private Transform _cardVisuals;
    [SerializeField] private CanvasGroup _frontGroup;
    [SerializeField] private CanvasGroup _backGroup;

    public GameCard Card => _card;

    protected bool _isFlipped;
    protected bool _flipping;

    protected virtual void Awake()
    {
        _frontGroup.interactable = false;
        _frontGroup.blocksRaycasts = false;
        _backGroup.interactable = false;
        _backGroup.blocksRaycasts = false;
    }

    protected virtual void OnValidate()
    {
        if (_card != null) UpdateVisuals();
    }

    public void SetData(GameCard data)
    {
        _card = data;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_playerName != null) _playerName.text = _card != null ? _card.PlayerData.PlayerName : "Player";
        if (_playerName2 != null) _playerName2.text = _card != null ? _card.PlayerData.PlayerName : "Player";
        if (_playerImage != null) _playerImage.sprite = _card != null ? _card.PlayerData.PlayerSprite : _defaultPlayerImage;

        for (int i = 0; i < _actionVisuals.Count; i++)
        {
            _actionVisuals[i].SetData(_card != null ? _card.GetAction(i) : new ActionData());
        }
    }

    public void FlipCard()
    {
        StartCoroutine(FlipCardRoutine());
    }

    private IEnumerator FlipCardRoutine()
    {
        if (_flipping) yield return null;
        _flipping = true;
        RefreshInteractables();
        _isFlipped = !_isFlipped;
        bool halfway = false;
        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            float delta = Mathf.Lerp(MathFunctions.EaseInOutQuart(t), t, 0.5f);
            float angle = _isFlipped ? Mathf.Lerp(0, 180, delta) : Mathf.Lerp(180, 360, delta);
            if (!halfway && angle >= (_isFlipped ? 90 : 270))
            {
                halfway = true;
                _frontGroup.alpha = _isFlipped ? 0 : 1;
                _backGroup.alpha = _isFlipped ? 1 : 0;
            }
            _cardVisuals.localRotation = Quaternion.Euler(0, angle, 0);
            _cardVisuals.localScale = Vector3.one + new Vector3(1f, 0.5f, 1f) * Mathf.Sin(t * Mathf.PI) * 0.1f;
            yield return null;
        }
        _flipping = false;
        RefreshInteractables();
    }

    protected virtual void RefreshInteractables() { }
}
