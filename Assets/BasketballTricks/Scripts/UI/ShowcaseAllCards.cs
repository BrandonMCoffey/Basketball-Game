using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowcaseAllCards : MonoBehaviour
{
    [SerializeField] private List<PlayerCardData> _allCards = new List<PlayerCardData>();
    [SerializeField] private List<PlayerCard> _cardVisuals;
    [SerializeField] private bool _orderAlphabetically;
    [SerializeField] private bool _orderByCost;
    [SerializeField] private bool _orderByHype;
    [SerializeField] private bool _orderByRarity;
    [SerializeField] private List<PlayerActionData> _allActions = new List<PlayerActionData>();
    [SerializeField] private List<ActionCard> _actionVisuals;

#if UNITY_EDITOR
    [Button]
    private void UpdateAll()
    {
        _cardVisuals = GetComponentsInChildren<PlayerCard>().ToList();
        _actionVisuals = GetComponentsInChildren<ActionCard>().ToList();

        _allCards = DataAnalyzer.GetAllInstancesOfType<PlayerCardData>();
        _allActions = DataAnalyzer.GetAllInstancesOfType<PlayerActionData>();
        if (_orderAlphabetically) _allActions = _allActions.OrderBy(action => action.Data.Name).ToList();
        if (_orderByCost) _allActions = _allActions.OrderBy(action => action.Data.Effects.Cost).ToList();
        if (_orderByHype) _allActions = _allActions.OrderBy(action => action.Data.Effects.HypeGain).ToList();
        if (_orderByRarity) _allActions = _allActions.OrderBy(action => action.Data.AssociatedRarity).ToList();

        int count = Mathf.Min(_cardVisuals.Count, _allCards.Count);
        int i = 0;
        for (; i < count; i++)
        {
            _cardVisuals[i].gameObject.SetActive(true);
            _cardVisuals[i].SetData(new GameCard(_allCards[i]));
        }
        for (; i < _cardVisuals.Count; i++)
        {
            _cardVisuals[i].gameObject.SetActive(false);
        }
        count = Mathf.Min(_actionVisuals.Count, _allActions.Count);
        i = 0;
        for (; i < count; i++)
        {
            _actionVisuals[i].gameObject.SetActive(true);
            _actionVisuals[i].RefreshVisuals(_allActions[i].Data);
        }
        for (; i < _actionVisuals.Count; i++)
        {
            _actionVisuals[i].gameObject.SetActive(false);
        }
    }
#endif
}
