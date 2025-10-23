using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _orderNumber;
    [SerializeField] private TMP_Text _duration;

    public void InitAction(Sprite image, int order)
    {
        _icon.sprite = image;
        _orderNumber.text = order.ToString();
    }

    public void PressButton()
    {
        Destroy(gameObject);
    }
}
