using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GameAction _action;
    [SerializeField] private Image _colorImage;
    [SerializeField] private Image _playerImage;
    [SerializeField] private TMP_Text _playerNumber;
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private TMP_Text _actionDescription;
    [SerializeField] private TMP_Text _actionType;
    [SerializeField] private Image _actionCostIcon;
    [SerializeField] private TMP_Text _actionCost;
    [SerializeField] private TMP_Text _actionHype;
    [SerializeField] private Image _actionIcon;
    [SerializeField] private Image _actionRarityMedal;
    [SerializeField] private GameObject _showWhenNotSelected;
    [SerializeField] private GameObject _showWhenDiscarding;
    [SerializeField] private Image _bg;
    [SerializeField] private ImageDataMatcher _matcher;
    [SerializeField] private RawImage _haloImage;

    [Header("Drag")]
    [SerializeField] private float _dragScale = 1.25f;
    [SerializeField] private float _selectedScale = 1.2f;
    [SerializeField] private float _playScale = 1.25f;
    [SerializeField] private float _playedDragScale = 0.4f;
    [SerializeField] private float _dragFollowSpeed = 20f;

    public GameAction Action => _action;

    private ActionDeckManager _manager;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDragging;
    private bool _wasDragged;
    private bool _locked;
    private bool _dragToPlay;
    private bool _played;
    private Tween _moveTween;
    private Tween _rotTween;
    private bool _isSelected;

    public RectTransform RectTransform => _rectTransform;
    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _haloImage.gameObject.SetActive(false);
    }
    private void Start()
    {
        if (_manager == null) _manager = FindObjectOfType<ActionDeckManager>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                Input.mousePosition,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
            _rectTransform.localPosition = Vector3.Lerp(
                _rectTransform.localPosition,
                localPoint,
                Time.deltaTime * _dragFollowSpeed
            );
            if (_played) _manager.OnCardDragReorderPlayed(this);
            else _manager.OnCardDragReorder(this);
        }
    }

    public void Init(GameAction gameAction, ActionDeckManager manager)
    {
        _action = gameAction;
        RefreshVisuals();
        _manager = manager;
        _isDragging = false;
        _wasDragged = false;
        _isSelected = false;
        _locked = false;
        _dragToPlay = false;
        _played = false;
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.DOScale(1f, 0.2f);
        if (_playerImage != null)
        {
            _playerImage.sprite = gameAction.Player.CardData.PlayerSprite;
            var aspect = _playerImage.GetComponent<AspectRatioFitter>();
            if (aspect != null)
            {
                _playerImage.SetNativeSize();
                aspect.aspectRatio = _playerImage.sprite != null ? (float)_playerImage.sprite.rect.width / _playerImage.sprite.rect.height : 1f;
            }
        }
        if (_matcher != null)
        {
            if (_bg != null) _bg.sprite = _matcher.GetPositionBackground(gameAction.Player.Position);
        }
        _haloImage.gameObject.SetActive(false);
    _showWhenNotSelected.SetActive(false);
        _showWhenDiscarding.SetActive(false);
    }

    public void RefreshVisuals(float adjustCost = 0, float adjustHype = 0)
    {
        var data = _action.Player.CardData.GetAction(_action.ActionIndex);
        var effects = data.Effects;
        if (_playerNumber != null) _playerNumber.text = _action.Player.CardData.PlayerNumber;
        if (_actionName != null) _actionName.text = data.Name;
        if (_actionDescription != null) _actionDescription.text = data.GetDisplayText();
        if (_actionType != null) _actionType.text = data.Type.ToString();
        int cost = Mathf.RoundToInt(effects.Cost + adjustCost);
        if (_actionCostIcon != null)
        {
            _actionCostIcon.sprite = _matcher.GetCostIcon(cost);
        }
        if (_actionCost != null)
        {
            _actionCost.text = _matcher.UseCostText(cost) ? cost.ToString() : "";
            _actionCost.color = adjustCost == 0 ? Color.white : (adjustCost < 0 ? Color.green : Color.red);
        }
        if (_actionHype != null)
        {
            _actionHype.text = (effects.HypeGain + adjustHype).ToString("0");
            _actionHype.color = adjustHype == 0 ? Color.white : (adjustHype > 0 ? Color.green : Color.red);
        }
        if (_actionIcon != null)
        {
            if (data.Icon != null) _actionIcon.sprite = data.Icon;
            else _actionIcon.sprite = _matcher.GetActionType(data.Type);
        }
        if (_colorImage != null) _colorImage.color = _matcher.GetPositionColor(_action.Player.Position);
        if (_actionRarityMedal != null) _actionRarityMedal.sprite = _matcher.GetActionCardMedal(data.AssociatedRarity);
    }

    public void SetLocked(bool locked)
    {
        _locked = locked;
        _showWhenNotSelected.SetActive(locked);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _wasDragged = false;
        _rectTransform.SetAsLastSibling();
        _rectTransform.DOScale(_played ? _playedDragScale : _dragScale, 0.2f);
        _rectTransform.DOLocalRotate(Vector3.zero, 0.2f);

        _moveTween?.Kill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _wasDragged = true;

        if (!_locked)
        {
            bool dragToPlay = transform.position.y > (_played ? _manager.CardRemovedPlayedPoint : _manager.CardPlayPoint).position.y;
            if (dragToPlay != _dragToPlay)
            {
                _rectTransform.DOScale(dragToPlay ? (_played ? _playedDragScale : _playScale) : _dragScale, 0.1f);
            }
            _dragToPlay = dragToPlay;
            if (!_played) _showWhenDiscarding.SetActive(RectTransformUtility.RectangleContainsScreenPoint(_manager.DiscardBox, eventData.position));
        }
        else
        {
            _dragToPlay = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        if (!_played && _dragToPlay)
        {
            _manager.PlayCard(this);
            _played = true;
            return;
        }
        else if (_played && !_dragToPlay)
        {
            _manager.RemoveCardFromPlay(this);
            _played = false;
        }
        else if (!_played)
        {
            //if (Vector2.Distance(_manager.DiscardBox.anchoredPosition, _rectTransform.anchoredPosition) < 50)
            if (RectTransformUtility.RectangleContainsScreenPoint(_manager.DiscardBox, eventData.position))
            {
                _manager.DiscardCard(this);
                _showWhenDiscarding.SetActive(true);
                return;
            }
            if (!_wasDragged)
            {
                /*
                _isSelected = !_isSelected;
                _showWhenNotSelected.SetActive(!_isSelected);
                _manager.OnUpdateSelected();
                if (!_isSelected) RefreshVisuals();
                */
            }
            _rectTransform.DOScale(_isSelected ? _selectedScale : 1f, 0.2f);
        }
        if (_wasDragged)
        {
            if (_played) _manager.OnCardDragReorderPlayed(this, true);
            else _manager.OnCardDragReorder(this, true);
        }

        _manager.UpdateCardLayout();
    }

    public void UpdateTransform(Vector2 targetPos, Quaternion targetRot, float duration, Ease ease)
    {
        _moveTween?.Kill();
        _rotTween?.Kill();

        if (!_isDragging)
        {
            _moveTween = _rectTransform.DOAnchorPos(targetPos, duration).SetEase(ease);
        }

        _rotTween = _rectTransform.DOLocalRotateQuaternion(targetRot, duration);
        _rectTransform.DOScale(_isSelected ? _selectedScale : 1f, 0.2f);
    }

    public void UpdateRotationOnly(Quaternion targetRot, float duration)
    {
        _rotTween?.Kill();
        _rotTween = _rectTransform.DOLocalRotateQuaternion(targetRot, duration);
    }
}