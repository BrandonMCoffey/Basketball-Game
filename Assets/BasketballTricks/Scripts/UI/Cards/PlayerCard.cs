using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerCard : PlayerCardVisuals, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Interactibility")]
    [SerializeField] private Image _raycastImage;
    [SerializeField] private bool _flipOnClick;

    [Header("Drag")]
    [SerializeField] private bool _canDrag;
    [SerializeField] private float _popScale = 1.1f;
    [SerializeField] private float _dragMoveSpeed = 10f;
    [SerializeField] private float _dragRotateSpeed = 5f;
    [SerializeField] private float _maxDragRotation = 20f;
    [SerializeField] private float _dragRotateSmoothing = 5f;
    [SerializeField] private float _returnSpeed = 4f;

    [Header("Hold to Focus")]
    [SerializeField] private float _holdDuration = 0.5f;
    [SerializeField] private float _holdScale = 3f;
    [SerializeField] private float _holdAnimationDuration = 0.5f;
    [SerializeField] private CanvasGroup _focusBackground;
    [SerializeField] private CanvasGroup _focusViewGroup;
    [SerializeField] private PlayerActionVisuals _actionDetails;
    [SerializeField] private RectTransform _flipTransform;

    private RectTransform _rectTransform;
    private Vector3 _initialScale;
    private Quaternion _initialRotation;
    private Vector3 _initialPosition;
    private bool _isDragging;
    private Vector3 _dragVelocity;
    private Vector3 _currentDragDelta;
    private Vector3 _currentDragPosition;
    private Coroutine _holdCoroutine;
    private bool _interactable;
    private bool _focusView;
    private int _actionDetailIndex;
    private RectTransform _actionDetailTransform;
    private Transform _parent;
    private Transform _holdParent;

    private bool CanDrag => _canDrag && !_focusView && !_flipping;

    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
        _actionDetailTransform = _actionDetails.gameObject.GetComponent<RectTransform>();
    }

    protected override void OnValidate()
    {
        if (_raycastImage == null) _raycastImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            _rectTransform.position = Vector3.Slerp(_rectTransform.position, _currentDragPosition, Time.deltaTime * _dragMoveSpeed);
            _dragVelocity = Vector3.Slerp(_dragVelocity, _currentDragDelta, Time.deltaTime * _dragRotateSmoothing);
            float tilt = Mathf.Clamp(_dragVelocity.x * -_dragRotateSpeed, -_maxDragRotation, _maxDragRotation);
            _rectTransform.localRotation = _initialRotation * Quaternion.Euler(0, 0, tilt);
            _currentDragDelta = Vector3.zero;
        }
    }

    public void Init(Transform parent, Transform holdParent)
    {
        _initialPosition = _rectTransform.position;
        _initialRotation = _rectTransform.localRotation;
        _initialScale = _rectTransform.localScale;
        _parent = parent;
        _holdParent = holdParent;
    }

    public void RefreshTransform()
    {
        _rectTransform.position = _initialPosition;
        _rectTransform.localRotation = _initialRotation;
        _rectTransform.localScale = _initialScale;
    }

    public void SetInteractable(bool interactable)
    {
        _interactable = interactable;
        RefreshInteractables();
    }

    protected override void RefreshInteractables()
    {
        _raycastImage.raycastTarget = !_focusView && _interactable;
        _focusViewGroup.alpha = _focusView ? 1 : 0;
        _focusViewGroup.blocksRaycasts = _focusView && _interactable && !_flipping;
        _focusViewGroup.interactable = _focusView && _interactable && !_flipping;
        if (_flipping && _actionDetailIndex >= 0) FocusShowAction(-1);
    }

    public void FocusShowAction(int index)
    {
        if (_actionDetailIndex == -1 && index == -1) return;
        if (index == -1 || _actionDetailIndex == index)
        {
            _actionDetailIndex = -1;
            _actionDetailTransform.DOLocalMoveX(0, _holdAnimationDuration).SetEase(Ease.OutQuart);
            _actionDetailTransform.DOScaleX(0, _holdAnimationDuration).SetEase(Ease.OutQuart);
            return;
        }
        if (_actionDetailIndex < 0)
        {
            _actionDetailTransform.DOLocalMoveX(150, _holdAnimationDuration).SetEase(Ease.OutQuart);
            _actionDetailTransform.DOScaleX(1, _holdAnimationDuration * 0.5f).SetEase(Ease.OutQuart);
        }
        _actionDetailIndex = index;
        _actionDetails.SetData(_data.GetAction(index));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == 0) OnClickCard();
    }

    protected virtual void OnClickCard()
    {
        if (_flipOnClick && !_focusView) FlipCard();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_focusView) return;
        _holdCoroutine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
    }

    private IEnumerator HoldRoutine()
    {
        yield return new WaitForSeconds(_holdDuration);
        ShowHold(true);
        _holdCoroutine = null;
    }

    public void ShowHold(bool show) => StartCoroutine(ShowHoldRoutine(show));
    private IEnumerator ShowHoldRoutine(bool show)
    {
        if (_isFlipped) FlipCard();
        if (_flipping) yield return null;
        _flipping = true;
        _focusView = true;
        RefreshInteractables();

        if (_holdParent != null) transform.SetParent(_holdParent, true);
        transform.SetAsLastSibling();
        var rectTransform = _focusBackground.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.position = new Vector3(-150, 0, 0);
        rectTransform.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
        _focusBackground.blocksRaycasts = true;
        _focusBackground.interactable = false;

        if (show)
        {
            _flipTransform.DOAnchorPosY(30, 0);
            _actionDetailTransform.DOLocalMoveX(0, 0);
            _actionDetailTransform.DOScaleX(0, 0);
        }
        else
        {
            _flipTransform.DOAnchorPosY(30, _holdAnimationDuration).SetEase(Ease.OutQuart);
            _actionDetailTransform.DOLocalMoveX(0, _holdAnimationDuration).SetEase(Ease.OutQuart);
            _actionDetailTransform.DOScaleX(0, _holdAnimationDuration).SetEase(Ease.OutQuart);
            yield return new WaitForSeconds(_holdAnimationDuration);
        }

        _focusBackground.DOFade(show ? 1 : 0, _holdAnimationDuration).SetEase(Ease.OutQuart);
        _rectTransform.DOMove(show ? transform.root.position : _initialPosition, _holdAnimationDuration).SetEase(Ease.OutQuart);
        _rectTransform.DOScale(show ? _initialScale * _holdScale : _initialScale, _holdAnimationDuration).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(_holdAnimationDuration);

        _focusBackground.blocksRaycasts = show;
        _focusBackground.interactable = show;

        if (show)
        {
            _flipTransform.DOAnchorPosY(-30, _holdAnimationDuration).SetEase(Ease.OutQuart);
        }
        else
        {
            if (_parent != null) transform.SetParent(_parent, true);
        }
        _focusView = show;
        _flipping = false;
        RefreshInteractables();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {        
        if (!CanDrag || _isDragging) return;
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
        _isDragging = true;
        _raycastImage.raycastTarget = false;
        _dragVelocity = Vector3.zero;
        _currentDragDelta = Vector3.zero;

        _rectTransform.localScale = _initialScale * _popScale;

        bool canPlace = PlayerManager.Instance.NewPlayerToPlace(_data);
        if (_parent != null && _holdParent != null) transform.SetParent(canPlace ? _parent : _holdParent, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag || !_isDragging) return;
        _currentDragPosition = (Vector3)eventData.position;
        _currentDragDelta = eventData.delta;

        PlayerManager.Instance.UpdatePlacingPlayer(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag || !_isDragging) return;

        bool success = PlayerManager.Instance.AttemptPlacePlayer(_data, eventData.position);
        if (success)
        {
            // TODO: Disable card permanently
            SetInteractable(false);
            _data = null;
            transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutQuart);
            return;
        }

        StartCoroutine(ReturnToPosition());
    }

    private IEnumerator ReturnToPosition()
    {
        _isDragging = false;
        _raycastImage.raycastTarget = false;
        Vector3 currentPosition = _rectTransform.position;
        Quaternion currentRotation = _rectTransform.localRotation;

        for (float t = 0; t < 1f; t += Time.deltaTime * _returnSpeed)
        {
            float delta = GameManager.EaseInOutQuart(t);
            _rectTransform.position = Vector3.Lerp(currentPosition, _initialPosition, delta);
            _rectTransform.localRotation = Quaternion.Lerp(currentRotation, _initialRotation, delta);
            _rectTransform.localScale = Vector3.Lerp(_initialScale * _popScale, _initialScale, delta);
            yield return null;
        }

        _rectTransform.position = _initialPosition;
        _rectTransform.localRotation = _initialRotation;
        _rectTransform.localScale = _initialScale;
        if (_parent != null) transform.SetParent(_parent, true);

        RefreshInteractables();
    }
}