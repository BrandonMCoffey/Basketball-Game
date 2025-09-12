using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class CardUI : DraggableUIItem, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _ppgText;
    [SerializeField] private TextMeshProUGUI _apgText;
    [SerializeField] private TextMeshProUGUI _rpgText;

    [Header("Settings")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _tweenDuration = 0.3f;
    [SerializeField] private float _selectionOffset = 50f;

    public CardData CardData { get; private set; }

    private Vector3 _homePosition;
    private Quaternion _homeRotation;
    private bool _isSelected = false;

    public void Initialize(CardData data, HandManager manager)
    {
        CardData = data;
        SetParentContainer(manager);

        _playerNameText.text = CardData.PlayerName;
        _ppgText.text = $"PPG: {CardData.PPG:F1}";
        _apgText.text = $"APG: {CardData.APG:F1}";
        _rpgText.text = $"RPG: {CardData.RPG:F1}";
    }

    public void SetHomePose(Vector3 position, Quaternion rotation)
    {
        _homePosition = position;
        _homeRotation = rotation;

        _rectTransform.DOLocalMove(_homePosition, _tweenDuration).SetEase(Ease.OutBack);
        _rectTransform.DOLocalRotateQuaternion(_homeRotation, _tweenDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        (_parentContainer as HandManager).ToggleCardSelection(this);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (_isSelected)
        {
            (_parentContainer as HandManager).ToggleCardSelection(this);
        }
        transform.DOScale(_hoverScale, _tweenDuration);
    }

    public void ToggleSelectionVisual(bool select)
    {
        _isSelected = select;
        Vector3 targetPosition = _isSelected ? _homePosition + new Vector3(0, _selectionOffset, 0) : _homePosition;
        _rectTransform.DOLocalMove(targetPosition, _tweenDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected) return;
        _rectTransform.DOScale(_hoverScale, _tweenDuration);
        _rectTransform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected) return;
        _rectTransform.DOScale(1f, _tweenDuration);
    }

    public void PlayScoreAnimation()
    {
        _rectTransform.DOShakePosition(0.3f, strength: new Vector3(10, 10, 0), vibrato: 15).SetEase(Ease.OutCubic);
    }

    public void DisableInteraction()
    {
        _canvasGroup.blocksRaycasts = false;
    }
}