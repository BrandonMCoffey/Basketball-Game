using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockerPositionSelector : MonoBehaviour, IDropHandler
{
    public static LockerPositionSelector CurrentDropDestination;

    [SerializeField] private PlayerPosition _position;
    [SerializeField] private TMP_Text _positionText;
    [SerializeField] private TMP_Text _positionText2;
    [SerializeField] private CanvasGroup _selectPlayerPanel;
    [SerializeField] private CanvasGroup _playerInfoPanel;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Transform _selectPlayer;
    [SerializeField] private GameObject _playerInfo;
    [SerializeField] private GameObject _naturalPosition;
    [SerializeField] private GameObject _actionPanel;
    [SerializeField] private List<TMP_Text> _actionList;

    [Header("Colors")]
    [SerializeField] private List<Image> _coloredImages;
    [SerializeField, Range(0, 1)] private float _saturationAdjust = 0.8f;
    [SerializeField, Range(0, 1)] private float _valueAdjust = 0.8f;

    private GameCard _activeGameCard;
    private LockerRoomController _controller;
    private int _controllerIndex;
    private Vector3 _originalPosition;

    public GameCard Card => _activeGameCard;
    public Vector3 OriginalPosition => _originalPosition + (_activeGameCard != null ? Vector3.up * 200f : Vector3.zero);

    private void OnValidate()
    {
        if (_positionText != null) _positionText.text = PlayerData.PositionToString(_position);
        if (_positionText2 != null) _positionText2.text = PlayerData.PositionToString(_position);
    }

    public void Init(int index, Color color, LockerRoomController controller)
    {
        _originalPosition = transform.position;
        _controllerIndex = index;
        _controller = controller;
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s *= _saturationAdjust;
        v *= _valueAdjust;
        foreach (var image in _coloredImages)
        {
            image.color = Color.HSVToRGB(h, s, v);
        }
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_activeGameCard == null || _activeGameCard == null)
        {
            _playerInfo.SetActive(false);
            return;
        }
        if (_playerNameText != null) _playerNameText.text = _activeGameCard != null ? _activeGameCard.PlayerData.PlayerName : "Player Name";
        if (_playerImage != null) _playerImage.sprite = _activeGameCard != null ? _activeGameCard.PlayerData.PlayerSprite : null;
        if (_naturalPosition != null) _naturalPosition.SetActive(_activeGameCard != null && _activeGameCard.PlayerData.IsNaturalPosition(_position));
        int i = 0;
        int count = Mathf.Min(_actionList.Count, _activeGameCard != null ? _activeGameCard.ActionCount : 0);
        for (; i < count; i++)
        {
            _actionList[i].text = $"{_activeGameCard.GetActionCount(i)}x {_activeGameCard.GetAction(i).Name}";
            _actionList[i].gameObject.SetActive(true);
        }
        for (; i < _actionList.Count; i++)
        {
            _actionList[i].gameObject.SetActive(false);
        }
        _playerInfo.SetActive(true);
    }

    public void OnSelect()
    {
        _controller.SelectPosition(_controllerIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {
        CurrentDropDestination = this;
    }

    public void AddCard(GameCard card)
    {
        _activeGameCard = card;
        GameManager.Instance.SetPositionCard(_position, card);
        UpdateVisuals();
    }
}
