using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private OpponentNameList _opponentNameList;
    [SerializeField] private List<BossEffect> _allBossEffects;
    [SerializeField] private int _handsPerRound = 4;
    [SerializeField] private int _discardsPerRound = 4;

    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private ShopUI _shopUI;
    [SerializeField] private GameObject _gameOver;

    private int _currentSeriesIndex = 0;
    private int _currentOpponentIndex = 0;
    private int _currentMoney;
    private List<string> _availableNames;
    private BossEffect _currentSeriesBoss;
    private List<GameChanger> _activeGameChangers = new List<GameChanger>();

    public int CurrentMoney => _currentMoney;
    public List<GameChanger> ActiveGameChangers => _activeGameChangers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameManager.OnRoundWon += HandleRoundWon;
        GameManager.OnRoundLost += HandleRoundLost;
    }

    private void OnDisable()
    {
        GameManager.OnRoundWon -= HandleRoundWon;
        GameManager.OnRoundLost -= HandleRoundLost;
    }

    private void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        _availableNames = new List<string>(_opponentNameList.Names);
        var rnd = new System.Random();
        _availableNames = _availableNames.OrderBy(item => rnd.Next()).ToList();

        _currentSeriesIndex = 0;
        StartNewSeries();
    }

    private void StartNewSeries()
    {
        _currentOpponentIndex = 0;
        _currentSeriesBoss = _allBossEffects[Random.Range(0, _allBossEffects.Count)];
        Debug.Log($"Start New Series");
        StartNextRound();
    }

    private void StartNextRound()
    {
        bool isBossRound = _currentOpponentIndex >= 2;
        BossEffect activeBoss = isBossRound ? _currentSeriesBoss : null;

        int targetScore = CalculateTargetScore(activeBoss);
        string opponentName = GetRandomOpponentName();

        _gameManager.SetupRound(opponentName, targetScore, activeBoss, _handsPerRound, _discardsPerRound);
    }

    private int CalculateTargetScore(BossEffect activeBoss)
    {
        int baseScore = 300;
        int seriesBonus = _currentSeriesIndex * 800;
        int opponentBonus = _currentOpponentIndex * 400;
        int totalScore = baseScore + seriesBonus + opponentBonus;

        if (activeBoss is DoubleScoreRequirementEffect)
        {
            totalScore *= 2;
        }

        return totalScore;
    }

    private string GetRandomOpponentName()
    {
        if (_availableNames.Count == 0) return "Opponent";
        string name = _availableNames[0];
        _availableNames.RemoveAt(0);
        return name;
    }

    private void HandleRoundWon()
    {
        Debug.Log("Round Won");

        if (_currentOpponentIndex == 0) _currentMoney += 50;
        if (_currentOpponentIndex == 1) _currentMoney += 75;
        if (_currentOpponentIndex == 2) _currentMoney += 100;

        if (_currentOpponentIndex >= 2)
        {
            _shopUI.OpenShop();
        }
        else
        {
            _currentOpponentIndex++;
            StartNextRound();
        }
    }

    public void LeaveShopAndStartNextSeries()
    {
        _shopUI.CloseShop();
        _currentSeriesIndex++;
        StartNewSeries();
    }

    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney >= amount)
        {
            _currentMoney -= amount;
            return true;
        }
        return false;
    }

    public void AquireGameChanger(GameChanger newChanger)
    {
        _activeGameChangers.Add(newChanger);
        _gameManager.GameChangerDisplay.AddGameChanger(newChanger);
    }

    private void HandleRoundLost()
    {
        Debug.Log("Round Lost - Game Over");
        _gameOver.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}