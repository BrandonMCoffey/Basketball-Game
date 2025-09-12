using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public abstract class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected DraggableContainer _parentContainer;
    protected RectTransform _rectTransform;
    protected CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetParentContainer(DraggableContainer container)
    {
        _parentContainer = container;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.SetAsLastSibling();
        DOTween.Kill(transform);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentContainer.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos);

        _rectTransform.localPosition = localPos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _parentContainer.OnItemDragEnded(this);
    }
}