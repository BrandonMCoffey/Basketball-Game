using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerCard : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private PlayerData _data;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _playerName2;
    [SerializeField] private TMP_Text _shootingStat;
    [SerializeField] private TMP_Text _dribblingStat;
    [SerializeField] private TMP_Text _teamPlayStat;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _teamImage;
    [SerializeField] private Image _raycastImage;

    [Header("Hover Effect")]
    [SerializeField] private float _popScale = 1.1f;
    [SerializeField] private float _hoverRotateMult = 10f;
    [SerializeField] private float _returnSpeed = 4f;
    [SerializeField] private float _dragMoveSpeed = 10f;
    [SerializeField] private float _dragRotateSpeed = 5f;
    [SerializeField] private float _maxDragRotation = 20f;
    [SerializeField] private float _dragRotateSmoothing = 5f;

    protected int _index;
    private RectTransform _rectTransform;
    private Vector3 _initialScale;
    private Quaternion _initialRotation;
    private Vector3 _originalPosition;
    private bool _isHovering;
    private bool _isDragging;
    private Vector3 _dragVelocity;
    private Vector3 _currentDragDelta;
    private Vector3 _currentDragPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialScale = _rectTransform.localScale;
        _initialRotation = _rectTransform.localRotation;
    }

    private void OnValidate()
    {
        if (_data != null) UpdateVisuals();
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

    public void SetIndex(int index)
    {
        _index = index;
    }

    public void SetData(PlayerData data)
    {
        _data = data;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_playerName != null) _playerName.text = _data.PlayerName;
        if (_playerName2 != null) _playerName2.text = _data.PlayerName;
        if (_shootingStat != null) _shootingStat.text = $"{_data.ShootingStat}";
        if (_dribblingStat != null) _dribblingStat.text = $"{_data.DribblingStat}";
        if (_teamPlayStat != null) _teamPlayStat.text = $"{_data.TeamPlayStat}";
        if (_data.PlayerSprite != null) _playerImage.sprite = _data.PlayerSprite;
        if (_data.TeamLogo != null) _teamImage.sprite = _data.TeamLogo;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isHovering || _isDragging) return;
        _isHovering = true;
        _rectTransform.localScale = _initialScale * _popScale;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_isHovering || _isDragging) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        float tiltX = (localPoint.y / (_rectTransform.rect.height * 0.5f)) * _hoverRotateMult;
        float tiltY = -(localPoint.x / (_rectTransform.rect.width * 0.5f)) * _hoverRotateMult;

        _rectTransform.localRotation = _initialRotation * Quaternion.Euler(tiltX, tiltY, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isHovering || _isDragging) return;
        _isHovering = false;
        _rectTransform.localScale = _initialScale;
        _rectTransform.localRotation = _initialRotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDragging) return;
        _isDragging = true;
        _originalPosition = _rectTransform.position;
        _raycastImage.raycastTarget = false;
        _dragVelocity = Vector3.zero;
        _currentDragDelta = Vector3.zero;

        _isHovering = false;
        _rectTransform.localScale = _initialScale * _popScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _currentDragPosition = (Vector3)eventData.position;
        _currentDragDelta = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;
        StartCoroutine(ReturnToPosition());
    }

    private IEnumerator ReturnToPosition()
    {
        Vector3 currentPosition = _rectTransform.position;
        Quaternion currentRotation = _rectTransform.localRotation;

        for (float t = 0; t < 1f; t += Time.deltaTime * _returnSpeed)
        {
            float delta = GameManager.EaseInOutQuart(t);
            _rectTransform.position = Vector3.Lerp(currentPosition, _originalPosition, delta);
            _rectTransform.localRotation = Quaternion.Lerp(currentRotation, _initialRotation, delta);
            _rectTransform.localScale = Vector3.Lerp(_initialScale * _popScale, _initialScale, delta);
            yield return null;
        }

        _rectTransform.position = _originalPosition;
        _rectTransform.localRotation = _initialRotation;
        _rectTransform.localScale = _initialScale;

        _raycastImage.raycastTarget = true;
    }
}