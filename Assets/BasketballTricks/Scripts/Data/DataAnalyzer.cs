using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class DataAnalyzer : ScriptableObject
{
    [SerializeField] private bool _showActionsUnderCards;
    [SerializeField] private List<PlayerCardData> _cardsToAnalyze = new List<PlayerCardData>();
    [SerializeField] private List<CardDataAnalysis> _cardsAnalysis = new List<CardDataAnalysis>();

    [Space(20)]
    [SerializeField] private bool _showActionEffects;
    [SerializeField] private bool _showCardsUnderActions;
    [SerializeField] private ActionType _typeFilter;
    [SerializeField] private List<PlayerActionData> _actionsToAnalyze = new List<PlayerActionData>();
    [SerializeField] private List<ActionDataAnalysis> _actionsAnalysis = new List<ActionDataAnalysis>();

    [System.Serializable]
    private struct CardDataAnalysis
    {
        public PlayerCardData Card;
        [ReadOnly, HideLabel] public string Details;
        [HideInInspector] public bool ShowActionsUnderCards;
        [ReadOnly, ShowIf(nameof(ShowActionsUnderCards)), ListDrawerSettings(ShowFoldout = false)]
        public List<string> Actions;
    }

    [System.Serializable]
    private struct ActionDataAnalysis
    {
        public PlayerActionData Action;
        [ReadOnly, HideLabel] public string Details;
        [HideInInspector] public bool ShowEffects;
        [ReadOnly, HideLabel, ShowIf(nameof(ShowEffects))] public string Effects;
        [HideInInspector] public bool ShowCardsUnderActions;
        [ReadOnly, ShowIf(nameof(ShowCardsUnderActions)), ListDrawerSettings(ShowFoldout = true)]
        public List<string> CardsUsingThis;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AnalyzeData();
    }

    [Button]
    private void AnalyzeData()
    {
        _cardsAnalysis = new List<CardDataAnalysis>(_cardsToAnalyze.Count);
        foreach (var card in _cardsToAnalyze)
        {
            List<string> list = new List<string>();
            var cardActions = card.GetAllActions();
            int count = 0;
            foreach (var action in cardActions)
            {
                var actionData = action.GetData();
                list.Add($"{action.Count}x | {actionData.Name} ({actionData.AssociatedRarity})");
                count += action.Count;
            }
            _cardsAnalysis.Add(new CardDataAnalysis
            {
                Card = card,
                Details = $"{card.Rarity} card with {count} actions. ({card.PlayerData.NaturalPosition})",
                ShowActionsUnderCards = _showActionsUnderCards,
                Actions = list,
            });
        }

        _actionsAnalysis = new List<ActionDataAnalysis>(_actionsToAnalyze.Count);
        foreach (var action in _actionsToAnalyze)
        {
            if (_typeFilter != ActionType.None && action.Data.Type != _typeFilter) continue;
            List<string> list = new List<string>();
            int cardCount = 0;
            int totalCount = 0;
            foreach (var card in _cardsToAnalyze)
            {
                var cardActions = card.GetAllActions();
                foreach (var cardAction in cardActions)
                {
                    if (cardAction.GetActionSO() == action)
                    {
                        list.Add($"{cardAction.Count}x | {card.PlayerData.PlayerName} | {card.CardTitle} ({card.Rarity}, {card.PlayerData.NaturalPosition})");
                        cardCount++;
                        totalCount += cardAction.Count;
                    }
                }
            }
            _actionsAnalysis.Add(new ActionDataAnalysis
            {
                Action = action,
                Details = $"{action.Data.AssociatedRarity}-level action found in {cardCount} cards ({totalCount} copies).",
                ShowEffects = _showActionEffects,
                Effects = $"Effects: {action.Data.GetSummaryText()}",
                ShowCardsUnderActions = _showCardsUnderActions,
                CardsUsingThis = list,
            });
        }
    }

    [Button]
    private void FetchAllCardsAndActions()
    {
        _cardsToAnalyze = GetAllInstancesOfType<PlayerCardData>();
        _actionsToAnalyze = GetAllInstancesOfType<PlayerActionData>();
    }

    public static List<T> GetAllInstancesOfType<T>() where T : ScriptableObject
    {
        List<T> instances = new List<T>();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);

        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                instances.Add(asset);
            }
        }
        return instances;
    }
#endif
}