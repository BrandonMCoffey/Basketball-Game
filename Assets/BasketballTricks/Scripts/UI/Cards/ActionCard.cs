using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private ActionData _actionData;
    [SerializeField] private Image _colorImage;
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private TMP_Text _actionDescription;
    [SerializeField] private TMP_Text _actionType;
    [SerializeField] private TMP_Text _actionCost;
    [SerializeField] private TMP_Text _actionHype;
    [SerializeField] private Image _actionIcon;

    [Header("Drag")]
    [SerializeField] private float _selectScale = 1.1f;
    [SerializeField] private float _dragFollowSpeed = 20f;

    private ActionDeckManager _manager;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDragging;
    private bool _wasDragged;
    private Tween _moveTween;
    private Tween _rotTween;
    private bool _isSelected;

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

    public void Init(ActionData data, Color color, ActionDeckManager manager)
    {
        _actionData = data;
        if (_actionName != null) _actionName.text = data.Name;
        if (_actionDescription != null) _actionDescription.text = data.GetDisplayText();
        if (_actionType != null) _actionType.text = data.Type.ToString();
        if (_actionCost != null) _actionCost.text = data.Cost.GetValue(data.ActionLevel).ToString("0");
        if (_actionHype != null) _actionHype.text = data.HypeGain.GetValue(data.ActionLevel).ToString("0");
        if (_actionIcon != null) _actionIcon.sprite = data.Icon;
        if (_colorImage != null) _colorImage.color = color;
        _manager = manager;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _wasDragged = false;
        _rectTransform.SetAsLastSibling();
        _rectTransform.DOScale(_selectScale, 0.2f);
        _rectTransform.DOLocalRotate(Vector3.zero, 0.2f);

        _moveTween?.Kill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _wasDragged = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        _rectTransform.DOScale(1f, 0.2f);

        if (!_wasDragged)
        {
            _isSelected = !_isSelected;
        }

        _manager.UpdateCardLayout();
    }

    public void UpdateTransform(Vector2 targetPos, Quaternion targetRot, float duration, Ease ease)
    {
        _moveTween?.Kill();
        _rotTween?.Kill();

        if (!_isDragging)
        {
            _moveTween = _rectTransform.DOLocalMove(targetPos, duration).SetEase(ease);
        }

        _rotTween = _rectTransform.DOLocalRotateQuaternion(targetRot, duration);
    }

    public void UpdateRotationOnly(Quaternion targetRot, float duration)
    {
        _rotTween?.Kill();
        _rotTween = _rectTransform.DOLocalRotateQuaternion(targetRot, duration);
    }
}