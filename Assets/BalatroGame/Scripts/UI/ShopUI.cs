using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private GameObject _shopItemPrefab;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Button _nextSeriesButton;

    private List<ShopItemUI> _displayedItems = new List<ShopItemUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        _nextSeriesButton.onClick.AddListener(RunManager.Instance.LeaveShopAndStartNextSeries);
        _shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        UpdateMoneyDisplay();

        foreach (Transform child in _itemContainer) Destroy(child.gameObject);
        _displayedItems.Clear();

        var offerings = ShopManager.Instance.GenerateOfferings(3);
        foreach (var item in offerings)
        {
            GameObject itemObj = Instantiate(_shopItemPrefab, _itemContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            itemUI.Initialize(item);
            _displayedItems.Add(itemUI);
        }

        _shopPanel.SetActive(true);
        CheckAffordability();
    }

    public void CloseShop()
    {
        _shopPanel.SetActive(false);
    }

    public void UpdateMoneyDisplay()
    {
        _moneyText.text = $"${RunManager.Instance.CurrentMoney}";
    }

    public void CheckAffordability()
    {
        foreach (var itemUI in _displayedItems)
        {
            itemUI.CheckIfAffordable(RunManager.Instance.CurrentMoney);
        }
    }

    public void OnItemPurchased(GameChanger purchasedItem)
    {
        UpdateMoneyDisplay();
        CheckAffordability();
    }
}