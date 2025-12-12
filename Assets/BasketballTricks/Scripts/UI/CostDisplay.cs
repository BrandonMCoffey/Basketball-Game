using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Color _unusedColor = Color.blue;
    [SerializeField] private Color _spentColor = Color.green;
    [SerializeField] private bool _overrideStartingEnergy = false;
    [SerializeField, ShowIf("_overrideStartingEnergy")] private int _startingEnergy = 5;
    //[SerializeField] private Color _overSpentColor = Color.red;
    //[SerializeField] private Color _overMaxColor = Color.white;
    [SerializeField] private List<StaminaElemController> _costList = new();

    [SerializeField] bool _removeTopToBottom = false;

    private void Awake()
    {
        PlayerManager.UpdateEnergy += UpdateCostDisplay;
    }

    IEnumerator Start() 
    {
        foreach (var costElem in _costList)
        {
            costElem.SetRefs(_unusedColor, _spentColor, this);
        }
        yield return new WaitForSeconds(0.2f);
        if (_overrideStartingEnergy) UpdateCostDisplay(0, _startingEnergy); 
    }

    public void UpdateCostDisplay(float cost, float max) => UpdateCostDisplay(Mathf.RoundToInt(cost), Mathf.RoundToInt(max));
    public void UpdateCostDisplay(int cost, int max)
    {
        if (_costText != null) _costText.text = $"{cost}/{max}";

        if (cost > _costList.Count || max > _costList.Count)
        {
            //Debug.LogWarning("Not enough cost objects to display cost");
            cost = Mathf.Min(cost, _costList.Count);
            max = Mathf.Min(max, _costList.Count);
        }

        if (_removeTopToBottom)
        {
            int spent = 0;
            for (int i = _costList.Count - 1; i >= 0; i--)
            {
                if (i >= max)
                {
                    _costList[i].Hide();
                }
                else if (spent < cost)
                {
                    _costList[i].Spend((spent) * 0.1f);
                    spent++;
                }
                else
                {
                    _costList[i].Unuse();
                }
            }
        }
        else
        {
            int i = 0;
            int jumpIndex = 0;
            for (; i < cost; i++) { _costList[i].Spend(jumpIndex * 0.1f); jumpIndex++; }
            for (; i < max; i++) { _costList[i].Unuse(); }
            for (; i < _costList.Count; i++) { _costList[i].Hide(); }
        }
    }
}