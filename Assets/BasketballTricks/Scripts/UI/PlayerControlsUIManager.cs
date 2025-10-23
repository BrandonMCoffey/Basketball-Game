using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlsUIManager : MonoBehaviour
{
    [SerializeField] private PlayerAddActionUI _addActionPrefab;
    [SerializeField] private CanvasGroup _selectActionGroup;
    [SerializeField] private Transform _selectActionRow;
    [SerializeField] private CanvasGroup _currentActionsGroup;
    [SerializeField] private Transform _currentActionsRow;
    [SerializeField] private PlayerActionDisplay _actionPrefab;

    private bool _playerSelected;
    private bool _currentActionsExpanded;
    private PlayerDestination _player;

    private void Awake()
    {
        _selectActionGroup.transform.localScale = Vector3.zero;
        _selectActionGroup.blocksRaycasts = false;
        _selectActionGroup.interactable = false;
        _currentActionsGroup.blocksRaycasts = false;
        _currentActionsGroup.interactable = false;
    }

    public void Init(PlayerDestination player)
    {
        _player = player;
        var actions = player.SpawnedPlayer.PlayerData.AllAvailableActions;
        for (int i = 0; i < actions.Count; i++)
        {
            PlayerAddActionUI newAction = Instantiate(_addActionPrefab, _selectActionRow);
            newAction.Init(this, actions[i], i);
            newAction.transform.localScale = Vector3.zero;
            newAction.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetDelay(i * 0.1f);
        }
    }

    public void AddAction(PlayerActionData action, int index)
    {
        PlayerActionDisplay newAction = Instantiate(_actionPrefab, _currentActionsRow);
        newAction.InitAction(action.Icon, index);
        newAction.transform.localScale = Vector3.zero;
        newAction.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);

        TopBarUIManager.Instance.AddAction(_player, action);
    }

    public void ToggleSelectPlayer() => SelectPlayer(!_playerSelected);
    private void SelectPlayer(bool select)
    {
        _playerSelected = select;
        _selectActionGroup.blocksRaycasts = select;
        _selectActionGroup.interactable = select;
        if (_playerSelected)
        {
            _selectActionGroup.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
        else
        {
            _selectActionGroup.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        }
        if (_currentActionsExpanded) ExpandCurrentActions(false);
    }

    public void ToggleExpandCurrentActions() => ExpandCurrentActions(!_currentActionsExpanded);
    public void ExpandCurrentActions(bool expanded)
    {
        _currentActionsExpanded = expanded;
        _currentActionsGroup.blocksRaycasts = expanded;
        _currentActionsGroup.interactable = expanded;
        if (_currentActionsExpanded)
        {
            _currentActionsGroup.transform.DOScale(Vector3.one * 1.5f, 0.25f).SetEase(Ease.InOutSine);
        }
        else
        {
            _currentActionsGroup.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InOutSine);
        }
    }
}
