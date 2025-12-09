using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private LongButtonController _disablePlayButton;
    [SerializeField] private Color _unusedColor = Color.blue;
    [SerializeField] private Color _spentColor = Color.green;
    //[SerializeField] private Color _overSpentColor = Color.red;
    //[SerializeField] private Color _overMaxColor = Color.white;
    [SerializeField] private List<StaminaElemController> _costList = new();

    private void OnValidate()
    {
        //UpdateCostDisplay(3, 5); sorry i know onvalidate is dope, but I had to do it through start....
    }

    private void Awake()
    {
        PlayerManager.UpdateEnergy += UpdateCostDisplay;
    }

    private void Start() 
    {
        foreach (var costElem in _costList)
        {
            costElem.SetRefs(_unusedColor, _spentColor, this);
        }
        UpdateCostDisplay(0, 5);    
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
        int jumpIndex = 0;
        for (; i < cost; i++) { _costList[i].Spend(jumpIndex * 0.1f); jumpIndex++; }
        for (; i < max; i++) { _costList[i].Unuse(); }
        for (; i < _costList.Count; i++) { _costList[i].Hide(); }
    }
}