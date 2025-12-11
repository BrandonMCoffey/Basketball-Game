using System.Collections.Generic;
using UnityEngine;

public class EnableOnAwake : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objs = new List<GameObject>();
    [SerializeField] private List<GameObject> _disableObjs = new List<GameObject>();

    private void Awake()
    {
        foreach (var obj in _objs)
        {
            if (obj != null) obj.SetActive(true);
        }
        foreach (var obj in _disableObjs)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}
