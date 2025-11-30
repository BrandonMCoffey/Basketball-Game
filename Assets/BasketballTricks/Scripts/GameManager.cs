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

    [SerializeField] private List<GameCard> _ownedPlayers;
    [SerializeField] private CanvasGroup _sceneTransition;

    private int _selectedCardIndex = -1;
    private Dictionary<PlayerPosition, GameCard> _playerLoadout = new Dictionary<PlayerPosition, GameCard>();

    // Creates a copy of the list to prevent external modification
    public List<GameCard> OwnedPlayers => new List<GameCard>(_ownedPlayers);
    public GameCard SelectedCard => (_selectedCardIndex >= 0 && _selectedCardIndex < _ownedPlayers.Count) ? _ownedPlayers[_selectedCardIndex] : null;

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
        Debug.Log($"Starting dribble practice with {_ownedPlayers[playerIndex].PlayerData.PlayerName}");
        _selectedCardIndex = playerIndex;
        TransitionToScene("DribbleGame");
    }

    public void StartShootingPractice(int playerIndex)
    {
        Debug.Log($"Starting shooting practice with {_ownedPlayers[playerIndex].PlayerData.PlayerName}");
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
                _sceneTransition.alpha = EaseInOutQuart(t);
                yield return null;
            }
            SceneManager.LoadScene(newScene);
        }
        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            _sceneTransition.alpha = 1 - EaseInOutQuart(t);
            yield return null;
        }
        InTransition = false;
    }

    public static float EaseInOutQuart(float x)
    {
        return x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
    }
}

[System.Serializable]
public class GameCard
{
    [SerializeField] private PlayerCardData _cardData;
    [SerializeField] private float _xp;
    [SerializeField] private int _level = 1;

    public GameCard(PlayerCardData data)
    {
        _cardData = data;
        _xp = 0;
        _level = 1;
    }

    public PlayerData PlayerData => _cardData.PlayerData;
    public int ActionCount => _cardData.ActionCount;
    public int GetActionCount(int index) => _cardData.GetActionCount(index);
    public ActionData GetAction(int index)
    {
        var data = _cardData.GetAction(index);
        data.ActionLevel = _level;
        return data;
    }

    public void AddXP(float amount)
    {
        if (_level >= 3) return;
        _xp += amount;
        float xpForNextLevel = (_level + 1) * 100f;
        while (_xp >= xpForNextLevel)
        {
            _xp -= xpForNextLevel;
            _level++;
            xpForNextLevel = (_level + 1) * 100f;
        }
    }
}