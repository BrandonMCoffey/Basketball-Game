using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace CoffeyUtils
{
    public class SortChildObjects : MonoBehaviour
    {
        [Button]
        private void Sort()
        {
            Transform[] children = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }
            var sortedChildren = children.OrderBy(child => child.name).ToArray();
            for (int i = 0; i < sortedChildren.Length; i++)
            {
                sortedChildren[i].SetSiblingIndex(i);
            }
        }
    }
}
