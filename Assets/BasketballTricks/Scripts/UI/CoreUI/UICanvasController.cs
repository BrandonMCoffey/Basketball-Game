using UnityEngine;
using SaiUtils.StateMachine;
using Sirenix.OdinInspector;
using CoffeyUtils.Sound;

public class UICanvasController : MonoBehaviour
{
    public static UICanvasController Instance { get; private set; }
    private StateMachine _stateMachine;

    public UIMainMenuState MainMenuState { get; private set; }
    public UIGameSelectState GameSelectState { get; private set; }
    public UIShopState ShopState { get; private set; }
    public UIVaultState VaultState { get; private set; }
    public UISettingsState SettingsState { get; private set; }
    public UILeaderboardState LeaderboardState { get; private set; }
    public UITradeState TradeState { get; private set; }

    [SerializeField] MainMenuController _mainMenuController;
    [SerializeField] GameSelectController _gameSelectController;
    [SerializeField] ShopController _shopController;
    [SerializeField] VaultController _vaultController;
    [SerializeField] SettingsController _settingsController;
    [SerializeField] LeaderboardController _leaderboardController;
    [SerializeField] TradeController _tradeController;
    public MainMenuController MainMenuController => _mainMenuController;
    public GameSelectController GameSelectController => _gameSelectController;
    public ShopController ShopController => _shopController;
    public VaultController VaultController => _vaultController;
    public SettingsController SettingsController => _settingsController;
    public LeaderboardController LeaderboardController => _leaderboardController;
    public TradeController TradeController => _tradeController;
    [ReadOnly] public bool HaveDelayOnMainMenu = true;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        MainMenuState = new UIMainMenuState(this);
        GameSelectState = new UIGameSelectState(this);
        ShopState = new UIShopState(this);
        VaultState = new UIVaultState(this);
        SettingsState = new UISettingsState(this);
        LeaderboardState = new UILeaderboardState(this);
        TradeState = new UITradeState(this);

        _stateMachine.AddAnyTransition(MainMenuState, new BlankPredicate());
        _stateMachine.AddAnyTransition(GameSelectState, new BlankPredicate());
        _stateMachine.AddAnyTransition(ShopState, new BlankPredicate());
        _stateMachine.AddAnyTransition(VaultState, new BlankPredicate());
        _stateMachine.AddAnyTransition(SettingsState, new BlankPredicate());
        _stateMachine.AddAnyTransition(LeaderboardState, new BlankPredicate());
        _stateMachine.AddAnyTransition(TradeState, new BlankPredicate());

        // Start in main menu
        _stateMachine.SetState(MainMenuState);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SoundManager.PlayMusicNow(MusicTracksEnum.MainMenu);
    }

    public void ChangeToGameSelect(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(GameSelectState, delay));
    public void ChangeToMainMenu(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(MainMenuState, delay));
    public void ChangeToShop(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(ShopState, delay));
    public void ChangeToVault(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(VaultState, delay));    
    public void ChangeToSettings(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(SettingsState, delay));
    public void ChangeToLeaderboard(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(LeaderboardState, delay));
    public void ChangeToTrade(float delay = 0.5f) => StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(TradeState, delay));

    private void Update()
    {
        _stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeToMainMenu();
        }
    }
}