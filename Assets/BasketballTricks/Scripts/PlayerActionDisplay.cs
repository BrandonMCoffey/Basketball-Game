using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _orderNumber;
    [SerializeField] private TMP_Text _duration;

    public void InitAction(Color color, int order)
    {
        _icon.color = color;
        _orderNumber.text = order.ToString();
    }

    public void PressButton()
    {
        Destroy(gameObject);
    }
}
