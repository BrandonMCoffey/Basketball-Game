using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private List<GameChanger> _allGameChangers;

    private List<GameChanger> _currentOfferings = new List<GameChanger>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public List<GameChanger> GenerateOfferings(int count)
    {
        _currentOfferings.Clear();
        var availableChangers = _allGameChangers.Except(RunManager.Instance.ActiveGameChangers).ToList();

        var rnd = new System.Random();
        availableChangers = availableChangers.OrderBy(item => rnd.Next()).ToList();

        _currentOfferings = availableChangers.Take(count).ToList();
        return _currentOfferings;
    }

    public void AttemptPurchase(GameChanger item)
    {
        if (RunManager.Instance.TrySpendMoney(item.Price))
        {
            RunManager.Instance.AquireGameChanger(item);
            _currentOfferings.Remove(item);
            ShopUI.Instance.OnItemPurchased(item);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }
}