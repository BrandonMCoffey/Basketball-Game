using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool InTransition { get; private set; }
    public static event System.Action UpdatePlayerLoadout = delegate { };

    [SerializeField] private bool _resetDataOnStart = false;
    [SerializeField] private int _startingMoney = 3000;
    [SerializeField] private int _roundsRequired = 3;
    [SerializeField] private List<PlayerCardData> _startingCards;
    [SerializeField] private List<PlayerCardData> _allCards;
    [SerializeField] private CanvasGroup _sceneTransition;

    private int _roundsPlayed;

    private GameSaveData _saveData;
    private List<GameCard> _ownedCards = new List<GameCard>();

    private int _selectedCardIndex = -1;
    private Dictionary<PlayerPosition, GameCard> _playerLoadout = new Dictionary<PlayerPosition, GameCard>();

    public List<GameCard> OwnedPlayers => new List<GameCard>(_ownedCards);
    public GameCard SelectedCard => (_selectedCardIndex >= 0 && _selectedCardIndex < _ownedCards.Count) ? _ownedCards[_selectedCardIndex] : null;

    public static event System.Action OnMoneyChanged = delegate { };
    public static event System.Action OnGameEnded = delegate { };
    public int CurrentMoney => _saveData.Money;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        StartCoroutine(TransitionToSceneRoutine("", true));
        
        if (_resetDataOnStart || !GameSaveData.Load(out _saveData) || _saveData.OwnedCardData.Count == 0)
        {
            if (!_resetDataOnStart) Debug.Log("No save data found. Initializing with starting cards.");
            _saveData = new GameSaveData(_startingMoney, _startingCards);
            _ownedCards.Clear();
            foreach (var cardData in _startingCards)
            {
                _ownedCards.Add(new GameCard(cardData));
            }
            SaveGame();
        }
        else
        {
            _ownedCards = new List<GameCard>();
            foreach (var cardSaveData in _saveData.OwnedCardData)
            {
                var card = _allCards.First(card => card.CardID == cardSaveData.CardID);
                if (card != null)
                {
                    _ownedCards.Add(new GameCard(card, cardSaveData));
                }
                else
                {
                    Debug.LogWarning($"Card with ID {cardSaveData.CardID} not found in database. It will be skipped.");
                }
            }
        }
    }

#if UNITY_EDITOR
    [Button]
    private void FillAllCards()
    {
        _allCards = DataAnalyzer.GetAllInstancesOfType<PlayerCardData>();
    }
#endif

    public void ResetRoundsPlayed()
    {
        _roundsPlayed = 0;
    }

    public void RoundCompleted()
    {
        _roundsPlayed++;
        if (_roundsPlayed >= _roundsRequired) OnGameEnded?.Invoke();
    }

    public void SaveGame()
    {
        _saveData.OwnedCardData.Clear();
        foreach (var card in _ownedCards)
        {
            var saveData = new GameCardSaveData
            {
                CardID = card.CardID,
                XP = card.XP,
                Level = card.Level,
                MatchesPlayed = card.MatchesPlayed,
                HypeScored = card.HypeScored,
                ShotsMade = card.ShotsMade,
                PassesMade = card.PassesMade,
                TricksMade = card.TricksMade
            };
            _saveData.OwnedCardData.Add(saveData);
        }
        GameSaveData.Save(_saveData);
    }

    public bool AttemptSpendMoney(int amount)
    {
        if (_saveData.Money >= amount)
        {
            _saveData.Money -= amount;
            SaveGame();
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void LoadMainMenu() => TransitionToScene("MainUI");
    public void LoadTutorialScene() => TransitionToScene("Tutorial");
    public void LoadGameScene() => TransitionToScene("HalftimeShow");
    public void LoadSandboxScene() => TransitionToScene("ZenScene");
    public void LoadDribbleMinigameScene(PlayerCardData card)
    {
        if (_ownedCards.Any(gameCard => gameCard.CardDataSO == card))
        {
            _selectedCardIndex = _ownedCards.FindIndex(gameCard => gameCard.CardDataSO == card);
            TransitionToScene("DribbleGame");
        }
        else
        {
            Debug.LogError($"Card {card.Player.PlayerName} not found in owned cards. Cannot start dribble practice.");
        }
    }
    public void StartShootingPractice(int playerIndex)
    {
        Debug.Log($"Starting shooting practice with {_ownedCards[playerIndex].PlayerName}");
        _selectedCardIndex = playerIndex;
        TransitionToScene("ShootingGame");
    }

    public List<GameCard> GetPositionLoadout()
    {
        return _playerLoadout.Values.ToList();
    }

    public void OnQuitGame()
    {
        SaveGame();
        _playerLoadout.Clear();
    }

    public void SetPositionCard(PlayerPosition position, GameCard card)
    {
        if (_playerLoadout.ContainsKey(position))
        {
            _playerLoadout[position] = card;
        }
        else
        {
            _playerLoadout.Add(position, card);
        }
        UpdatePlayerLoadout?.Invoke();
    }

    public void AddCardToOwned(PlayerCardData cardData)
    {
        var matchingCard = GetMatchingCard(cardData);
        if (matchingCard == null)
        {
            _ownedCards.Add(new GameCard(cardData));
            SaveGame();
        }
        else
        {
            Debug.LogWarning($"Card already owned. Not adding duplicate.");
            matchingCard.AddXP(50); // Grant some XP for duplicate
            SaveGame();
        }
    }

    public GameCard GetMatchingCard(PlayerCardData cardData)
    {
        return _ownedCards.Find(card => card.CardDataSO == cardData);
    }

    private void TransitionToScene(string newScene)
    {
        if (!InTransition)
        {
            StartCoroutine(TransitionToSceneRoutine(newScene));
        }
        else
        {
            Debug.LogWarning($"Scene transition already in progress. Cannot load {newScene}");
        }
    }
    private IEnumerator TransitionToSceneRoutine(string newScene, bool startup = false)
    {
        InTransition = true;
        if (!startup)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
            {
                _sceneTransition.alpha = MathFunctions.EaseInOutQuart(t);
                yield return null;
            }
            SceneManager.LoadScene(newScene);
        }
        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            _sceneTransition.alpha = 1 - MathFunctions.EaseInOutQuart(t);
            yield return null;
        }
        _sceneTransition.alpha = 0;
        InTransition = false;
    }
}