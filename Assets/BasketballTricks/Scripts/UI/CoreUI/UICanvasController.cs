using System.Collections.Generic;
using UnityEngine;
using SaiUtils.StateMachine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UICanvasController : MonoBehaviour
{
    private StateMachine _stateMachine;

    [Header("Animation Settings")]
    public float transitionDuration = 0.4f;
    public Ease transitionEase = Ease.OutExpo;

    [Header("Panel References")]
    public RectTransform menusContainer;
    public MenuReference navBarContainer;
    public List<MenuPositions> menuPositions;

    [Header("Main Menu References")]
    public MenuReference playerData;
    public MenuReference campaignButton;
    public MenuReference tournamentButton;
    public MenuReference tradeButton;

    [Header("Trade Menu References")]
    public MenuReference cardContainer;
    public MenuReference tradeBoxesContainer;

    public UIMainMenuState MainMenuState { get; private set; }
    public UIVaultState VaultState { get; private set; }
    public UIStoreState StoreState { get; private set; }
    public UITradeState TradeState { get; private set; }

    public Vector2 GetPosition(Menus menu)
    {
        return menuPositions.Find(x => x.MenuName == menu).Position;
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();
        MainMenuState = new UIMainMenuState(this);
        VaultState = new UIVaultState(this);
        StoreState = new UIStoreState(this);
        TradeState = new UITradeState(this);

        _stateMachine.AddAnyTransition(MainMenuState, new BlankPredicate());
        _stateMachine.AddAnyTransition(VaultState, new BlankPredicate());
        _stateMachine.AddAnyTransition(StoreState, new BlankPredicate());
        _stateMachine.AddAnyTransition(TradeState, new BlankPredicate());

        _stateMachine.SetState(MainMenuState);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("TrickPlays");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _stateMachine.ChangeState(MainMenuState);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _stateMachine.ChangeState(VaultState);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _stateMachine.ChangeState(StoreState);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _stateMachine.ChangeState(TradeState);
        }
        _stateMachine.Update();
    }

    void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }


}

public enum Menus
{
    MainMenu,
    Vault,
    Store,
    Trade
}

[System.Serializable]
public struct MenuPositions
{
    public Menus MenuName;
    public Vector2 Position;
}

[System.Serializable]
public struct MenuReference
{
    public RectTransform Panel;
    public Vector2 OnScreenPosition;
    public Vector2 OffScreenPosition;
}
