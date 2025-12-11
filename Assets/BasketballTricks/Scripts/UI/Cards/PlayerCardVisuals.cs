using Sirenix.OdinInspector;
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
    [SerializeField] private Image _playerImage2;
    [SerializeField] private Image _bgImage;
    [SerializeField] private Sprite _defaultPlayerImage;
    [SerializeField] private Sprite _defaultTeamImage;
    [SerializeField] private List<PlayerActionVisuals> _actionVisuals;
    [SerializeField] private TMP_Text _cardBackPositions;
    [SerializeField] private List<TMP_Text> _cardBackImportantText;
    [SerializeField] private Transform _cardBackDataTree;
    [SerializeField] private ImageDataMatcher _matcher;

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

    public void SetData(GameCard data, PlayerPosition position = PlayerPosition.None, bool positionBonus = false)
    {
        _card = data;
        if (_bgImage != null && _matcher != null)
        {
            _bgImage.sprite = _matcher.GetPositionBackground(position);
            _bgImage.color = positionBonus ? Color.white : Color.gray;
        }
        UpdateVisuals();
        UpdateCardBackDataVisuals();
    }

    private void UpdateVisuals()
    {
        if (_playerName != null) _playerName.text = _card != null ? _card.PlayerName : "Player";
        if (_playerName2 != null) _playerName2.text = _card != null ? _card.PlayerName : "Player";
        if (_playerImage != null) _playerImage.sprite = _card != null ? _card.PlayerSprite : _defaultPlayerImage;
        if (_playerImage2 != null) _playerImage2.sprite = _card != null ? _card.PlayerSprite : _defaultPlayerImage;

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

    [Button]
    public void UpdateCardBackDataVisuals()
    {
        _cardBackPositions.text = _card.PlayerData.NaturalPosition.ToString();

        var lines = _card.PlayerData.CardBackImportantData;
        int count = Mathf.Min(_cardBackImportantText.Count, lines.Count);
        for (int i = 0; i < count; i++)
        {
            _cardBackImportantText[i].text = lines[i];
        }

        var data = _card.PlayerData.CardBackData;
        for (int i = 0; i < _cardBackDataTree.childCount; i++)
        {
            if (data.Count <= i) break;
            CardBackColumnData dataColumn = data[i];
            var column = _cardBackDataTree.GetChild(i);
            for (int j = 0; j < column.childCount; j++)
            {
                var row = column.GetChild(j);
                var text = row.GetComponent<TMP_Text>();
                if (text == null) continue;
                switch (j)
                {
                    case 0:
                        text.text = dataColumn.Title;
                        text.color = Color.white;
                        break;
                    case 1:
                        text.text = string.IsNullOrEmpty(dataColumn.Row1) ? " " : dataColumn.Row1;
                        text.color = Color.black;
                        break;
                    case 2:
                        text.text = string.IsNullOrEmpty(dataColumn.Row2) ? " " : dataColumn.Row2;
                        text.color = Color.black;
                        break;
                }
            }
        }
    }
}
