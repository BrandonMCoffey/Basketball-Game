using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private Color _unusedColor = Color.blue;
    [SerializeField] private Color _spentColor = Color.green;
    [SerializeField] private Color _overSpentColor = Color.red;
    [SerializeField] private Color _overMaxColor = Color.white;
    [SerializeField] private List<Image> _costList = new List<Image>();

    private void OnValidate()
    {
        UpdateCostDisplay(3, 5);
    }

    public void UpdateCostDisplay(int cost, int max)
    {
        if (cost > _costList.Count || max > _costList.Count)
        {
            Debug.LogWarning("Not enough cost objects to display cost");
            cost = Mathf.Min(cost, _costList.Count);
            max = Mathf.Min(max, _costList.Count);
        }
        int i = 0;
        for (; i < Mathf.Min(cost, max); i++)
        {
            _costList[i].color = i <= max ? _spentColor : _overSpentColor;
        }
        for (; i < max; i++)
        {
            _costList[i].color = _unusedColor;
        }
        for (; i < _costList.Count; i++)
        {
            _costList[i].color = _overMaxColor;
        }
    }
}