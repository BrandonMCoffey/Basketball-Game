using DG.Tweening;
using SaiUtils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LockerRoomController : MonoBehaviour
{
    [SerializeField] private List<LockerPositionSelector> _lockerPositions = new List<LockerPositionSelector>();
    [SerializeField] private RectTransform _selectedPosition;
    [SerializeField] private Button _letsGoButton;
    [SerializeField] private RectTransform _letsGoButtonT;
    [SerializeField] private RectTransform _catalog;
    [SerializeField] private Ease _lockerEaseEnter = Ease.InQuart;
    [SerializeField] private Ease _lockerEaseLeave = Ease.InQuart;
    [SerializeField] private float _lockerDelay = 0.05f;
    [SerializeField] private ActionDeckManager _deckManager; 
    [SerializeField, Range(0, 0.5f)] private float _cardSelectedOffsetY = 0.1f;

    private Vector3 CardVertical => _rectTransform.rect.height * Vector3.up;

    private int _selectedIndex = -1;
    private Vector3 _letsGoOriginalPosition;
    private Vector3 _catalogOriginalPosition;
    private bool _moving;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        var players = PlayerManager.Instance.Players;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].Init(i, players[i].PositionColor, this);
        }
        _letsGoButton.interactable = false;
        _letsGoOriginalPosition = _letsGoButtonT.anchoredPosition;
        _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition + Vector3.down * 400f, 0);
        _catalogOriginalPosition = _catalog.anchoredPosition;
        _catalog.DOAnchorPos(_catalogOriginalPosition + Vector3.right * 2000, 0);
        StartCoroutine(StartShowLockersRoutine());
    }

    public void SelectRandomPlayers(bool allNaturalPositions)
    {
        var players = GameManager.Instance.OwnedPlayers;
        players.Shuffle();
        int count = Mathf.Min(players.Count, _lockerPositions.Count);
        if (allNaturalPositions)
        {
            // TODO: Some players have multiple natural positions, but as they are removed, they may be removed from consideration for other positions.
            for (int i = 0; i < count; i++)
            {
                var player = players.FirstOrDefault(p => p.PlayerData.IsNaturalPosition(_lockerPositions[i].Position));
                player ??= players[0];
                _lockerPositions[i].AddCard(player);
                players.Remove(player);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                _lockerPositions[i].AddCard(players[i]);
            }
        }
        foreach (var locker in _lockerPositions)
        {
            var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
            locker.RectTransform.DOAnchorPos(pos, 0.5f).SetEase(_lockerEaseEnter);
        }
        bool buttonInteractable = _lockerPositions.All(selector => selector.Card != null);
        _letsGoButton.interactable = buttonInteractable;
        if (buttonInteractable) _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
    }


    public IEnumerator StartShowLockersRoutine()
    {
        _moving = true;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].RectTransform.anchoredPosition = _lockerPositions[i].OriginalPosition - CardVertical * 3;
        }

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].RectTransform.DOAnchorPos(_lockerPositions[i].OriginalPosition, 0.25f).SetEase(_lockerEaseEnter);
            yield return new WaitForSeconds(_lockerDelay);
        }
        _moving = false;
    }

    public void SelectPosition(int index)
    {
        if (_moving) return;
        StartCoroutine(SelectPositionRoutine(index));
    }
    private IEnumerator SelectPositionRoutine(int index)
    {
        _moving = true;
        if (_selectedIndex == index || index == -1)
        {
            _catalog.DOAnchorPos(_catalogOriginalPosition + Vector3.right * 2000, 1f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.5f);
            foreach (var locker in _lockerPositions)
            {
                var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
                locker.RectTransform.DOAnchorPos(pos, 0.5f).SetEase(_lockerEaseEnter);
                yield return new WaitForSeconds(_lockerDelay);
            }
            yield return new WaitForSeconds(0.1f);
            bool buttonInteractable = _lockerPositions.All(selector => selector.Card != null);
            _letsGoButton.interactable = buttonInteractable;
            if (buttonInteractable) _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
            _selectedIndex = -1;
            _moving = false;
            yield break;
        }
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            if (i == index)
            {
                _lockerPositions[i].RectTransform.DOAnchorPos(_selectedPosition.anchoredPosition, 0.5f).SetEase(Ease.InOutCubic);
            }
            else
            {
                var pos = _lockerPositions[i].OriginalPosition - CardVertical * 3;
                _lockerPositions[i].RectTransform.DOAnchorPos(pos, 0.5f).SetEase(_lockerEaseLeave);
                yield return new WaitForSeconds(_lockerDelay);
            }
        }
        _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition + Vector3.down * 400f, 0.5f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.25f);
        _catalog.DOAnchorPos(_catalogOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
        _selectedIndex = index;
        _moving = false;
    }

    public void StartGame()
    {
        var players = PlayerManager.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Place(_lockerPositions[i].Card);
        }
        _deckManager.Init();
        gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
