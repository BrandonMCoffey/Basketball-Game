using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Button _buyButton;

    private GameChanger _itemData;

    public void Initialize(GameChanger itemData)
    {
        _itemData = itemData;
        _nameText.text = itemData.Name;
        _descriptionText.text = itemData.Description;
        _priceText.text = $"${itemData.Price}";
        _buyButton.onClick.AddListener(OnBuyClicked);
    }

    public void CheckIfAffordable(int currentMoney)
    {
        _buyButton.interactable = currentMoney >= _itemData.Price;
    }

    private void OnBuyClicked()
    {
        ShopManager.Instance.AttemptPurchase(_itemData);
        _buyButton.interactable = false;
    }
}