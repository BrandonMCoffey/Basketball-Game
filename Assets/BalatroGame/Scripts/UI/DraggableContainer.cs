using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class DraggableContainer : MonoBehaviour
{
    protected List<DraggableUIItem> _items = new List<DraggableUIItem>();

    public virtual void OnItemDragEnded(DraggableUIItem item)
    {
        _items = _items.OrderBy(i => i.transform.localPosition.x).ToList();
        UpdateLayout(true);
    }

    protected abstract void UpdateLayout(bool animated);
}