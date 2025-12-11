using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        GameManager.OnMoneyChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDisable()
    {
        GameManager.OnMoneyChanged -= UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        _text.text = $"{GameManager.Instance.CurrentMoney}";
    }
}
