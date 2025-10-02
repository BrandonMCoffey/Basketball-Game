using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlsUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _selectActionGroup;
    [SerializeField] private CanvasGroup _currentActionsGroup;
    [SerializeField] private Transform _currentActions;
    [SerializeField] private PlayerActionDisplay _actionPrefab;

    private bool _playerSelected;
    private bool _currentActionsExpanded;

    private void Awake()
    {
        _selectActionGroup.transform.localScale = Vector3.zero;
        _selectActionGroup.blocksRaycasts = false;
        _selectActionGroup.interactable = false;
        _currentActionsGroup.blocksRaycasts = false;
        _currentActionsGroup.interactable = false;
    }

    public void AddAction(int index)
    {
        PlayerActionDisplay newAction = Instantiate(_actionPrefab, _currentActions);
        newAction.InitAction(Color.HSVToRGB(index / 10f, 1f, 1f), index);
        newAction.transform.localScale = Vector3.zero;
        newAction.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
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
