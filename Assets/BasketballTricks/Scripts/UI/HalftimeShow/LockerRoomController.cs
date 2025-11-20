using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LockerRoomController : MonoBehaviour
{
    [SerializeField] private List<LockerPositionSelector> _lockerPositions = new List<LockerPositionSelector>();
    [SerializeField] private Transform _selectedPosition;
    [SerializeField] private Button _letsGoButton;
    [SerializeField] private Transform _catalog;

    private int _selectedIndex = -1;
    private Vector3 _letsGoOriginalPosition;
    private Vector3 _catalogOriginalPosition;
    private bool _moving;

    private void Start()
    {
        var players = PlayerManager.Instance.Players;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].Init(i, players[i].PositionColor, this);
        }
        _letsGoButton.interactable = false;
        _letsGoOriginalPosition = _letsGoButton.transform.position;
        _catalogOriginalPosition = _catalog.position;
        _catalog.position = _catalogOriginalPosition + Vector3.right * Screen.width;
    }

    public void SelectPosition(int index)
    {
        if (_moving) return;
        StartCoroutine(SelectPositionRoutine(index));
    }
    private IEnumerator SelectPositionRoutine(int index)
    {
        _moving = true;
        if (_selectedIndex == index)
        {
            _catalog.DOMove(_catalogOriginalPosition + Vector3.right * Screen.width, 1f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.5f);
            foreach (var locker in _lockerPositions)
            {
                locker.transform.DOMove(locker.OriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
            }
            yield return new WaitForSeconds(0.1f);
            _letsGoButton.interactable = _lockerPositions.All(selector => selector.HasGameCard);
            _letsGoButton.transform.DOMove(_letsGoOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
            _selectedIndex = -1;
            _moving = false;
            yield break;
        }
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            if (i == index)
            {
                _lockerPositions[i].transform.DOMove(_selectedPosition.position, 0.5f).SetEase(Ease.InOutCubic);
            }
            else
            {
                var pos = _lockerPositions[i].OriginalPosition + Vector3.down * Screen.height;
                _lockerPositions[i].transform.DOMove(pos, 0.5f).SetEase(Ease.InOutCubic);
            }
        }
        _letsGoButton.transform.DOMove(_letsGoOriginalPosition + Vector3.down * 400f, 0.5f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.25f);
        _catalog.DOMove(_catalogOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
        _selectedIndex = index;
        _moving = false;
    }
}
