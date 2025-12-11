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
    [SerializeField] private List<PlayerCardData> _startingCards;
    [SerializeField] private GameSaveData _saveData;
    [SerializeField] private CanvasGroup _sceneTransition;

    private int _selectedCardIndex = -1;
    private Dictionary<PlayerPosition, GameCard> _playerLoadout = new Dictionary<PlayerPosition, GameCard>();

    public List<GameCard> OwnedPlayers => new List<GameCard>(_saveData.OwnedCards); // Creates a copy of the list to prevent external modification
    public GameCard SelectedCard => (_selectedCardIndex >= 0 && _selectedCardIndex < _saveData.OwnedCards.Count) ? _saveData.OwnedCards[_selectedCardIndex] : null;

    public static event System.Action OnMoneyChanged = delegate { };
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
        
        if (_resetDataOnStart || !GameSaveData.Load(ref _saveData))
        {
            if (!_resetDataOnStart) Debug.Log("No save data found. Initializing with starting cards.");
            _saveData = new GameSaveData(_startingMoney, _startingCards);
            GameSaveData.Save(_saveData);
        }
    }

    public bool AttemptSpendMoney(int amount)
    {
        if (_saveData.Money >= amount)
        {
            _saveData.Money -= amount;
            GameSaveData.Save(_saveData);
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void StartGame()
    {
        TransitionToScene("TrickPlays");
    }

    public void LoadCatalog()
    {
        TransitionToScene("CardCatalog");
    }

    public void StartDribblePractice(int playerIndex)
    {
        Debug.Log($"Starting dribble practice with {_saveData.OwnedCards[playerIndex].PlayerName}");
        _selectedCardIndex = playerIndex;
        TransitionToScene("DribbleGame");
    }

    public void StartShootingPractice(int playerIndex)
    {
        Debug.Log($"Starting shooting practice with {_saveData.OwnedCards[playerIndex].PlayerName}");
        _selectedCardIndex = playerIndex;
        TransitionToScene("ShootingGame");
    }

    public List<GameCard> GetPositionLoadout()
    {
        return _playerLoadout.Values.ToList();
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
            _saveData.OwnedCards.Add(new GameCard(cardData));
            GameSaveData.Save(_saveData);
        }
        else
        {
            Debug.LogWarning($"Card already owned. Not adding duplicate.");
            matchingCard.AddXP(50); // Grant some XP for duplicate
        }
    }

    public GameCard GetMatchingCard(PlayerCardData cardData)
    {
        return _saveData.OwnedCards.Find(card => card.CardDataSO == cardData);
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