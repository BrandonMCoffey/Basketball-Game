using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalatroManager : MonoBehaviour
{
    public static BalatroManager Instance { get; private set; }
    public static event Action OnRoundWon;
    public static event Action OnRoundLost;

    [Header("Data")]
    [SerializeField] private List<LineupType> _allLineupTypes;

    [Header("References")]
    [SerializeField] private GameChangerDisplay _gameChangerDisplay;
    [SerializeField] private TextMeshProUGUI _opponentNameText;
    [SerializeField] private TextMeshProUGUI _targetScoreText;
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _lineupNameText;
    [SerializeField] private TextMeshProUGUI _lineupBonusText;
    [SerializeField] private TextMeshProUGUI _offensiveRatingText;
    [SerializeField] private TextMeshProUGUI _playmakingModifierText;
    [SerializeField] private TextMeshProUGUI _handsRemainingText;
    [SerializeField] private TextMeshProUGUI _discardsRemainingText;
    [SerializeField] private Button _playHandButton;
    [SerializeField] private Button _discardButton;

    private int _targetScore;
    private int _handsRemaining;
    private int _discardsRemaining;
    private int _currentScore = 0;
    private bool _isAnimating = false;
    private BossEffect _activeBossEffect;

    public GameChangerDisplay GameChangerDisplay => _gameChangerDisplay;

    private void Start()
    {
        _playHandButton.onClick.AddListener(() => StartCoroutine(PlaySelectedHandRoutine()));
        _discardButton.onClick.AddListener(() => StartCoroutine(DiscardSelectedHandRoutine()));
    }

    public void SetupRound(string opponentName, int targetScore, BossEffect activeBossEffect, int hands, int discards)
    {
        _currentScore = 0;
        _targetScore = targetScore;
        _handsRemaining = hands;
        _discardsRemaining = discards;
        _activeBossEffect = activeBossEffect;

        _opponentNameText.text = opponentName;
        UpdateScoreDisplay();
        UpdateRoundInfoDisplay();

        HandManager.Instance.ResetAndShuffleDeck();

        _activeBossEffect?.OnRoundStart(this);
        SetButtonsInteractable(true);
    }

    public void ModifyDiscards(int amount)
    {
        _discardsRemaining += amount;
        if (_discardsRemaining < 0) _discardsRemaining = 0;
        UpdateRoundInfoDisplay();
    }

    private void OnPlayOrDiscard(bool isPlay)
    {
        if (isPlay)
        {
            if (_handsRemaining <= 0) return;
            _handsRemaining--;
        }
        else
        {
            if (_discardsRemaining <= 0) return;
            _discardsRemaining--;
        }
        UpdateRoundInfoDisplay();
    }

    private void CheckWinLossCondition()
    {
        if (_currentScore >= _targetScore)
        {
            SetButtonsInteractable(false);
            OnRoundWon?.Invoke();
        }
        else if (_handsRemaining <= 0)
        {
            SetButtonsInteractable(false);
            OnRoundLost?.Invoke();
        }
    }

    private void UpdateRoundInfoDisplay()
    {
        _handsRemainingText.text = $"Hands: {_handsRemaining}";
        _discardsRemainingText.text = $"Discards: {_discardsRemaining}";
    }

    private IEnumerator PlaySelectedHandRoutine()
    {
        if (_isAnimating || _handsRemaining <= 0) yield break;

        _activeBossEffect?.OnHandPlayStart(this);

        var selectedCards = HandManager.Instance.SelectedCards
            .OrderBy(c => c.transform.localPosition.x)
            .ToList();

        if (selectedCards.Count == 0) yield break;

        _isAnimating = true;
        SetButtonsInteractable(false);

        var orderedChangers = _gameChangerDisplay.GetOrderedChangers();

        foreach (var changerUI in orderedChangers) changerUI.Data.OnHandPlayStart();

        // --- Scoring Sequence ---
        LineupType bestFitLineup = FindBestLineup(selectedCards);
        ScoreData scoreData = new ScoreData(selectedCards);

        DisplayLineupInfo(bestFitLineup);
        if (bestFitLineup != null)
        {
            scoreData.OffensiveRating += bestFitLineup.BaseOR;
            scoreData.PlaymakingModifier += bestFitLineup.DPM;
        }

        // OnScoreCalculationStart
        foreach (var changerUI in orderedChangers)
        {
            if (!changerUI.IsDisabled && changerUI.Data.OnScoreCalculationStart(scoreData))
            {
                changerUI.PlayEffectAnimation();
                yield return new WaitForSeconds(0.3f);
            }
        }

        _offensiveRatingText.text = $"OR: {scoreData.OffensiveRating:F1}";
        _playmakingModifierText.text = $"DPM: {scoreData.PlaymakingModifier:F1}";

        // OnCardContributesScore
        foreach (var card in selectedCards)
        {
            yield return new WaitForSeconds(0.4f);
            card.PlayScoreAnimation();

            scoreData.OffensiveRating += card.CardData.PPG;
            scoreData.PlaymakingModifier += card.CardData.APG;

            foreach (var changerUI in orderedChangers)
            {
                if (!changerUI.IsDisabled && changerUI.Data.OnCardContributesScore(card, scoreData))
                {
                    changerUI.PlayEffectAnimation();
                    yield return new WaitForSeconds(0.3f);
                }
            }
            _offensiveRatingText.text = $"OR: {scoreData.OffensiveRating:F1}";
            _playmakingModifierText.text = $"DPM: {scoreData.PlaymakingModifier:F1}";
        }

        // OnScoreCalculationEnd
        foreach (var changerUI in orderedChangers)
        {
            if (!changerUI.IsDisabled && changerUI.Data.OnScoreCalculationEnd(scoreData))
            {
                changerUI.PlayEffectAnimation();
                yield return new WaitForSeconds(0.3f);
            }
        }

        yield return new WaitForSeconds(1f);

        int scoreFromHand = (int)(scoreData.OffensiveRating * (scoreData.PlaymakingModifier < 1 ? 1 : scoreData.PlaymakingModifier));
        _currentScore += scoreFromHand;
        UpdateScoreDisplay();

        yield return StartCoroutine(ProcessPlayedCardsRoutine(selectedCards, true));
        ClearScoringDisplay();

        CheckWinLossCondition();
        _isAnimating = false;
        SetButtonsInteractable(true);
    }

    private IEnumerator DiscardSelectedHandRoutine()
    {
        if (_isAnimating || _discardsRemaining <= 0) yield break;
        var selectedCards = HandManager.Instance.SelectedCards.ToList();
        if (selectedCards.Count == 0) yield break;

        _activeBossEffect?.OnHandPlayStart(this);

        _isAnimating = true;
        SetButtonsInteractable(false);

        yield return StartCoroutine(ProcessPlayedCardsRoutine(selectedCards, false));

        CheckWinLossCondition();
        _isAnimating = false;
        SetButtonsInteractable(true);
    }

    private IEnumerator ProcessPlayedCardsRoutine(List<CardUI> playedCards, bool isPlay)
    {
        int cardCount = playedCards.Count;

        foreach (var card in playedCards)
        {
            HandManager.Instance.ToggleCardSelection(card);
        }

        OnPlayOrDiscard(isPlay);

        yield return StartCoroutine(HandManager.Instance.AnimateDiscardRoutine(playedCards));
        yield return StartCoroutine(HandManager.Instance.DrawCardsRoutine(cardCount));

        _activeBossEffect?.OnHandPlayEnd(this);
    }

    private LineupType FindBestLineup(List<CardUI> lineup)
    {
        return _allLineupTypes
            .Where(lt => lt.CheckCondition(lineup))
            .OrderByDescending(lt => lt.Priority)
            .FirstOrDefault();
    }

    private void DisplayLineupInfo(LineupType lineup)
    {
        if (lineup != null)
        {
            _lineupNameText.text = lineup.LineupName;
            _lineupBonusText.text = $"+{lineup.BaseOR} OR / +{lineup.DPM} DPM";
        }
        else
        {
            _lineupNameText.text = "High Card";
            _lineupBonusText.text = "";
        }
    }

    private void ClearScoringDisplay()
    {
        _lineupNameText.text = "";
        _lineupBonusText.text = "";
        _offensiveRatingText.text = "";
        _playmakingModifierText.text = "";
    }

    private void SetButtonsInteractable(bool interactable)
    {
        _playHandButton.interactable = interactable;
        _discardButton.interactable = interactable;
    }

    private void UpdateScoreDisplay()
    {
        _currentScoreText.text = $"Score: {_currentScore}";
        _targetScoreText.text = $"Target: {_targetScore}";
    }
}