using UnityEngine;
using SaiUtils.StateMachine;
using System.Collections;

public class UICanvasController : MonoBehaviour
{
    public static UICanvasController Instance { get; private set; }
    private StateMachine _stateMachine;

    public UIMainMenuState MainMenuState { get; private set; }
    public UIGameSelectState GameSelectState { get; private set; }
    public UIStoreState StoreState { get; private set; }
    public UITradeState TradeState { get; private set; }

    [SerializeField] MainMenuController _mainMenuController;
    public MainMenuController MainMenuController => _mainMenuController;
    [SerializeField] GameSelectController _gameSelectController;
    public GameSelectController GameSelectController => _gameSelectController;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        MainMenuState = new UIMainMenuState(this);
        GameSelectState = new UIGameSelectState(this);

        _stateMachine.AddAnyTransition(MainMenuState, new BlankPredicate());
        _stateMachine.AddAnyTransition(GameSelectState, new BlankPredicate());

        // Start in main menu
        _stateMachine.SetState(MainMenuState);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeToGameSelect(float delay = 0.35f)
    {
        StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(GameSelectState, delay));
    }

    public void ChangeToMainMenu(float delay = 0.35f)
    {
        StartCoroutine(_stateMachine.ChangeStateWithDelayCoroutine(MainMenuState, delay));
    }

}
    

