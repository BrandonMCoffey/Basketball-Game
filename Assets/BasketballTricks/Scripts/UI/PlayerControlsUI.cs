using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlsUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _currentActionsGroup;
    [SerializeField] private Transform _currentActionsRow;
    [SerializeField] private PlayerActionDisplay _actionPrefab;
    [SerializeField] private Image _buttonImage;
    
    private bool _currentActionsExpanded;
    private PlayerUIManager _manager;
    private int _index;
    private bool _selected;

    private void Awake()
    {
        _currentActionsGroup.blocksRaycasts = false;
        _currentActionsGroup.interactable = false;
    }

    public void Init(PlayerUIManager manager, int index, Player player)
    {
        _manager = manager;
        _index = index;
        _buttonImage.color = player != null ? player.PositionColor : new Color(0,0,0,0);
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
}
