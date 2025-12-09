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
    [SerializeField] private TMP_Text _actionCost;
    [SerializeField] private TMP_Text _actionHype;
    [SerializeField] private Image _actionIcon;
    [SerializeField] private Sprite _defaultTrickIcon;
    [SerializeField] private Sprite _defaultPassIcon;
    [SerializeField] private Sprite _defaultShotIcon;
    [SerializeField] private GameObject _showWhenNotSelected;

    [Header("Drag")]
    [SerializeField] private float _dragScale = 1.25f;
    [SerializeField] private float _selectedScale = 1.2f;
    [SerializeField] private float _playScale = 1.25f;
    [SerializeField] private float _dragFollowSpeed = 20f;

    public GameAction Action => _action;

    private ActionDeckManager _manager;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDragging;
    private bool _wasDragged;
    private bool _canPlay;
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
            _manager.OnCardDragReorder(this);
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
        _canPlay = false;
        _played = false;
        _playerImage.sprite = gameAction.Player.CardData.PlayerData.PlayerSprite;
        _rectTransform.DOScale(1f, 0.2f);
        var aspect = _playerImage.GetComponent<AspectRatioFitter>();
        if (aspect != null)
        {
            _playerImage.SetNativeSize();
            aspect.aspectRatio = _playerImage.sprite != null ? (float)_playerImage.sprite.rect.width / _playerImage.sprite.rect.height : 1f;
        }
        _showWhenNotSelected.SetActive(true);
    }

    public void RefreshVisuals(float adjustCost = 0, float adjustHype = 0)
    {
        var data = _action.Player.CardData.GetAction(_action.ActionIndex);
        var effects = data.Effects;
        if (_playerNumber != null) _playerNumber.text = _action.Player.CardData.PlayerData.PlayerNumber;
        if (_actionName != null) _actionName.text = data.Name;
        if (_actionDescription != null) _actionDescription.text = data.GetDisplayText();
        if (_actionType != null) _actionType.text = data.Type.ToString();
        if (_actionCost != null)
        {
            _actionCost.text = (effects.Cost + adjustCost).ToString("0");
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
            else _actionIcon.sprite = data.Type switch
            {
                ActionType.Trick => _defaultTrickIcon,
                ActionType.Pass => _defaultPassIcon,
                ActionType.Shot => _defaultShotIcon,
                _ => null
            };
        }
        if (_colorImage != null) _colorImage.color = _action.Player.PositionColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_played) return;
        _wasDragged = false;
        _rectTransform.SetAsLastSibling();
        _rectTransform.DOScale(_dragScale, 0.2f);
        _rectTransform.DOLocalRotate(Vector3.zero, 0.2f);

        _moveTween?.Kill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_played) return;
        _isDragging = true;
        _wasDragged = true;

        bool canPlay = transform.position.y > _manager.CardPlayPoint.position.y;
        if (canPlay != _canPlay)
        {
            _rectTransform.DOScale(canPlay ? _playScale : _dragScale, 0.1f);
        }
        _canPlay = canPlay;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_played) return;
        _isDragging = false;

        if (_canPlay)
        {
            _rectTransform.DOScale(_isSelected ? _selectedScale : 0f, 0.2f);
            _rectTransform.DOAnchorPos(_rectTransform.anchoredPosition + Vector2.down * 500, 2f);
            _manager.PlayCard(this);
            _played = true;
        }
        else
        {
            if (!_wasDragged)
            {
                _isSelected = !_isSelected;
                _showWhenNotSelected.SetActive(!_isSelected);
                _manager.OnUpdateSelected();
                if (!_isSelected) RefreshVisuals();
            }
            _rectTransform.DOScale(_isSelected ? _selectedScale : 1f, 0.2f);
        }

        _manager.UpdateCardLayout();
    }

    public void UpdateTransform(Vector2 targetPos, Quaternion targetRot, float duration, Ease ease)
    {
        if (_played) return;
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
        if (_played) return;
        _rotTween?.Kill();
        _rotTween = _rectTransform.DOLocalRotateQuaternion(targetRot, duration);
    }
}