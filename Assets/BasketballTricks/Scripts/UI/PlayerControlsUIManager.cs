using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlsUIManager : MonoBehaviour
{
    [SerializeField] private PlayerAddActionUI _addActionPrefab;
    [SerializeField] private CanvasGroup _currentActionsGroup;
    [SerializeField] private Transform _currentActionsRow;
    [SerializeField] private PlayerActionDisplay _actionPrefab;

    private bool _playerSelected;
    private bool _currentActionsExpanded;
    private Player _player;

    private void Awake()
    {
        _currentActionsGroup.blocksRaycasts = false;
        _currentActionsGroup.interactable = false;
    }

    public void Init(Player player)
    {
        _player = player;
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
