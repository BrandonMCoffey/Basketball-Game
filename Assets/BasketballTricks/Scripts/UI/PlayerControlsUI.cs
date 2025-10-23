using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerControlsUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _currentActionsGroup;
    [SerializeField] private Transform _currentActionsRow;
    [SerializeField] private PlayerActionDisplay _actionPrefab;
    
    private bool _currentActionsExpanded;
    private PlayerUIManager _manager;
    private int _index;
    private bool _selected;

    private void Awake()
    {
        _currentActionsGroup.blocksRaycasts = false;
        _currentActionsGroup.interactable = false;
    }

    public void Init(PlayerUIManager manager, int index)
    {
        _manager = manager;
        _index = index;
    }

    public void ToggleSelected() => _manager.ToggleSelectPlayer(_index);
    public void SetSelected(bool selected)
    {
        _selected = selected;
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

    public void AddAction(PlayerActionData action, int index)
    {
        PlayerActionDisplay newAction = Instantiate(_actionPrefab, _currentActionsRow);
        newAction.InitAction(action.Data.Icon, index);
        newAction.transform.localScale = Vector3.zero;
        newAction.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);

        //TopBarUIManager.Instance.AddAction(_player, action);
    }
}
