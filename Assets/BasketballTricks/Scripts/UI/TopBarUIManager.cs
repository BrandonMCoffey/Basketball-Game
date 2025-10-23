using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBarUIManager : MonoBehaviour
{
    public static TopBarUIManager Instance;

    [SerializeField] private TopBarActionUI _barActionPrefab;
    [SerializeField] private List<TopBarActionUI> _activeBars;

    private void Awake()
    {
        Instance = this;
    }

    public void AddAction(Player player, PlayerActionData action)
    {
        TopBarActionUI bar = Instantiate(_barActionPrefab, transform);
        if (bar != null)
        {
            bar.SetAction(player, action.Data);
            _activeBars.Add(bar);
        }
    }
}
