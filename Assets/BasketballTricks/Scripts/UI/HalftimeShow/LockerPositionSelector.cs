using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class LockerPositionSelector : MonoBehaviour, IDropHandler
{
    public static LockerPositionSelector CurrentDropDestination;

    [SerializeField] private PlayerPosition _position;
    [SerializeField] private TMP_Text _positionText;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private Image _playerImage;
    [SerializeField] private GameObject _playerSelect;
    [SerializeField] private GameObject _playerInfo;
    [SerializeField] private GameObject _naturalPosition;
    [SerializeField] private GameObject _actionPanel;
    [SerializeField] private Image _bg;
    [SerializeField] private ImageDataMatcher _matcher;
    [SerializeField] private List<TMP_Text> _actionList;
    [SerializeField] private RectTransform _addButtonImage;

    [Header("Colors")]
    [SerializeField] private List<Image> _coloredImages;
    [SerializeField] private List<Image> _darkColoredImages;
    [SerializeField, Range(0, 1)] private float _saturationAdjust = 0.8f;
    [SerializeField, Range(0, 1)] private float _valueAdjust = 0.8f;
    [SerializeField, Range(0, 1)] private float _darkenAdjust = 0.4f;
    [SerializeField, Range(0, 1)] private float _alpha = 0.8f;

    private GameCard _activeGameCard;
    private LockerRoomController _controller;
    private int _controllerIndex;
    private Vector3 _originalPosition;

    public GameCard Card => _activeGameCard;
    public PlayerPosition Position => _position;
    public Vector3 OriginalPosition => _originalPosition;
    Coroutine _addPlayerAnimRoutine;
    RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;

    private void OnValidate()
    {
        if (_positionText != null) _positionText.text = PlayerData.PositionToString(_position);
    }

    private void Awake() 
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(int index, Color color, LockerRoomController controller)
    {
        _originalPosition = RectTransform.anchoredPosition;
        _controllerIndex = index;
        _controller = controller;
        if (_matcher != null)
        {
            if (_bg != null) _bg.sprite = _matcher.GetPositionBackground(_position);
        }
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s *= _saturationAdjust;
        v *= _valueAdjust;
        var c = Color.HSVToRGB(h, s, v);
        c.a = _alpha;
        foreach (var image in _coloredImages)
        {
            image.color = c;
        }
        c = Color.HSVToRGB(h, s, v * _darkenAdjust);
        c.a = _alpha;
        foreach (var image in _darkColoredImages)
        {
            image.color = c;
        }
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_activeGameCard == null || _activeGameCard == null)
        {
            _playerInfo.SetActive(false);
            _playerSelect.SetActive(true);

            if (_addPlayerAnimRoutine != null) StopCoroutine(_addPlayerAnimRoutine);
            _addPlayerAnimRoutine = StartCoroutine(AddPlayerAnimationRoutine());
            
            return;
        }
        _playerSelect.SetActive(false);
        if (_addPlayerAnimRoutine != null)
        {
            StopCoroutine(_addPlayerAnimRoutine);
            _addPlayerAnimRoutine = null;
        }
        _addButtonImage.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
        if (_playerNameText != null) _playerNameText.text = _activeGameCard != null ? _activeGameCard.PlayerData.PlayerName : "Player Name";
        if (_playerImage != null)
        {
            _playerImage.sprite = _activeGameCard != null ? _activeGameCard.PlayerData.PlayerSprite : null;
            var aspect = _playerImage.GetComponent < AspectRatioFitter>();
            if (aspect != null)
            {
                _playerImage.SetNativeSize();
                aspect.aspectRatio = _playerImage.sprite != null ? (float)_playerImage.sprite.rect.width / _playerImage.sprite.rect.height : 1f;
            }
        }
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

    IEnumerator AddPlayerAnimationRoutine()
    {
        while (true)
        {
            _addButtonImage.DOScale(1.2f, 0.5f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.5f);
            _addButtonImage.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
