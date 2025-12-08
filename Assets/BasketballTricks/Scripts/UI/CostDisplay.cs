using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private LongButtonController _disablePlayButton;
    [SerializeField] private Color _unusedColor = Color.blue;
    [SerializeField] private Color _spentColor = Color.green;
    [SerializeField] private Color _overSpentColor = Color.red;
    [SerializeField] private Color _overMaxColor = Color.white;
    [SerializeField] private List<Image> _costList = new List<Image>();

    private void OnValidate()
    {
        UpdateCostDisplay(3, 5);
    }

    private void Awake()
    {
        PlayerManager.UpdateEnergy += UpdateCostDisplay;
    }

    public void UpdateCostDisplay(float cost, float max) => UpdateCostDisplay(Mathf.RoundToInt(cost), Mathf.RoundToInt(max));
    public void UpdateCostDisplay(int cost, int max)
    {
        if (_costText != null) _costText.text = $"{cost}/{max}";
        if (_disablePlayButton != null) _disablePlayButton.Interactable = cost <= max;

        if (cost > _costList.Count || max > _costList.Count)
        {
            //Debug.LogWarning("Not enough cost objects to display cost");
            cost = Mathf.Min(cost, _costList.Count);
            max = Mathf.Min(max, _costList.Count);
        }
        int i = 0;
        for (; i < cost; i++)
        {
            _costList[i].color = i < max ? _spentColor : _overSpentColor;
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