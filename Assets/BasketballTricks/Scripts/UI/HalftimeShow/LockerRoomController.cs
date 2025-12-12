using CoffeyUtils.Sound;
using DG.Tweening;
using SaiUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockerRoomController : MonoBehaviour
{
    [SerializeField] private List<LockerPositionSelector> _lockerPositions = new List<LockerPositionSelector>();
    [SerializeField] private RectTransform _selectedPosition;
    [SerializeField] private LongButtonController _letsGoButton;
    [SerializeField] private RectTransform _letsGoButtonT;
    [SerializeField] private RectTransform _promptStart;
    [SerializeField] private RectTransform _catalog;
    [SerializeField] private Ease _catalogEaseIn = Ease.OutCubic;
    [SerializeField] private Ease _lockerEaseEnter = Ease.InQuart;
    [SerializeField] private Ease _lockerEaseLeave = Ease.InQuart;
    [SerializeField] private float _betweenLockerDelay = 0.1f;
    [SerializeField] private float _lockerAnimTime = 0.25f;
    [SerializeField] private float _buttonAnimTime = 0.25f;
    [SerializeField] private float _catalogAnimTime = 0.5f;
    [SerializeField] private ActionDeckManager _deckManager;
    [SerializeField, Range(0, 0.5f)] private float _cardSelectedOffsetY = 0.1f;

    private Vector3 CardVertical => _rectTransform.rect.height * Vector3.up;

    private int _selectedIndex = -1;
    private Vector3 _letsGoOriginalPosition;
    private Vector3 _promptStartOriginalPosition;
    private Vector3 _catalogOriginalPosition;
    private bool _moving;
    private RectTransform _rectTransform;
    private HandCatalog _handCatalogRef;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _handCatalogRef = _catalog.GetComponent<HandCatalog>();
        var players = PlayerManager.Instance.Players;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].Init(i, this);
        }
        _letsGoButton.Interactable = false;
        _letsGoOriginalPosition = _letsGoButtonT.anchoredPosition;
        _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition + Vector3.down * 400f, 0);
        _promptStartOriginalPosition = _promptStart.anchoredPosition;
        _promptStart.DOAnchorPos(_promptStartOriginalPosition + Vector3.down * 400f, 0);
        _catalogOriginalPosition = _catalog.anchoredPosition;
        _catalog.DOAnchorPos(_catalogOriginalPosition + Vector3.right * 2000, 0);
        StartCoroutine(StartShowLockersRoutine());
        SoundManager.PlayMusicNow(MusicTracksEnum.LockerRoom);
    }

    public void SelectRandomPlayers(bool allNaturalPositions)
    {
        var players = GameManager.Instance.OwnedPlayers;
        players.Shuffle();
        _catalog.DOAnchorPos(_catalogOriginalPosition + Vector3.right * 2000, 0.2f).SetEase(Ease.InOutCubic);
        int count = Mathf.Min(players.Count, _lockerPositions.Count);
        if (allNaturalPositions)
        {
            // TODO: Some players have multiple natural positions, but as they are removed, they may be removed from consideration for other positions.
            for (int i = 0; i < count; i++)
            {
                var player = players.FirstOrDefault(p => p.PlayerData.IsNaturalPosition(_lockerPositions[i].Position));
                player ??= players[0];
                _lockerPositions[i].AddCard(player, true);
                players.Remove(player);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                _lockerPositions[i].AddCard(players[i], true);
            }
        }
        foreach (var locker in _lockerPositions)
        {
            var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
            locker.RectTransform.DOAnchorPos(pos, _lockerAnimTime).SetEase(_lockerEaseEnter);
        }
        _promptStart.DOAnchorPos(_promptStartOriginalPosition + Vector3.down * 400f, _buttonAnimTime).SetEase(Ease.InOutCubic);
        bool buttonInteractable = _lockerPositions.All(selector => selector.Card != null);
        _letsGoButton.Interactable = buttonInteractable;
        if (buttonInteractable) _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition, _buttonAnimTime).SetEase(Ease.InOutCubic);
    }


    public IEnumerator StartShowLockersRoutine()
    {
        _moving = true;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].RectTransform.anchoredPosition = _lockerPositions[i].OriginalPosition - CardVertical * 3;
        }

        yield return new WaitForSeconds(_lockerAnimTime);
        foreach (LockerPositionSelector locker in _lockerPositions)
        {
            var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
            locker.RectTransform.DOAnchorPos(pos, _lockerAnimTime).SetEase(_lockerEaseEnter);
            yield return new WaitForSeconds(_betweenLockerDelay);
        }
        var promptOffset = _lockerPositions.Any(selector => selector.Card != null) ? Vector3.down * 400f : Vector3.zero;
        _promptStart.DOAnchorPos(_promptStartOriginalPosition + promptOffset,  _buttonAnimTime).SetEase(Ease.InOutCubic);
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
            _catalog.DOAnchorPos(_catalogOriginalPosition + Vector3.right * 2000, _catalogAnimTime).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(_catalogAnimTime * 0.5f);
            if (_selectedIndex >= 0)
            {
                var locker = _lockerPositions[_selectedIndex];
                var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
                locker.RectTransform.DOAnchorPos(pos, _lockerAnimTime).SetEase(Ease.InOutCubic);
                yield return new WaitForSeconds(_betweenLockerDelay);
            }
            for (int i = 0; i < _lockerPositions.Count; i++)
            {
                if (i == index) continue;
                var locker = _lockerPositions[i];
                var pos = locker.OriginalPosition + (locker.Card != null ? CardVertical * _cardSelectedOffsetY : Vector3.zero);
                locker.RectTransform.DOAnchorPos(pos, _lockerAnimTime).SetEase(_lockerEaseEnter);
                yield return new WaitForSeconds(_betweenLockerDelay);
            }
            if (!_lockerPositions.Any(selector => selector.Card != null))
            {
                _promptStart.DOAnchorPos(_promptStartOriginalPosition, _buttonAnimTime).SetEase(Ease.InOutCubic);
            }
            yield return new WaitForSeconds(0.1f);
            bool buttonInteractable = _lockerPositions.All(selector => selector.Card != null);
            _letsGoButton.Interactable = buttonInteractable;
            if (buttonInteractable) _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition, _buttonAnimTime).SetEase(Ease.InOutCubic);
            _selectedIndex = -1;
            _moving = false;
            yield break;
        }
        _promptStart.DOAnchorPos(_promptStartOriginalPosition + Vector3.down * 400f, _buttonAnimTime).SetEase(Ease.InOutCubic);
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            if (i == index)
            {
                _lockerPositions[i].RectTransform.DOAnchorPos(_selectedPosition.anchoredPosition, _lockerAnimTime).SetEase(Ease.InOutCubic);
            }
            else
            {
                var pos = _lockerPositions[i].OriginalPosition - CardVertical * 3;
                _lockerPositions[i].RectTransform.DOAnchorPos(pos, _lockerAnimTime).SetEase(_lockerEaseLeave);
            }
            yield return new WaitForSeconds(_betweenLockerDelay);
        }
        _letsGoButtonT.DOAnchorPos(_letsGoOriginalPosition + Vector3.down * 400f, _buttonAnimTime).SetEase(Ease.InOutCubic);
        _handCatalogRef.HideCards();
        _selectedIndex = index;
        _catalog.DOAnchorPos(_catalogOriginalPosition, _catalogAnimTime * 0.5f).SetEase(_catalogEaseIn).OnComplete(() =>
        {
            //Debug.Log("Showing cards in catalog");
            _handCatalogRef.ShowCardsFiltered(_lockerPositions[_selectedIndex].Position);
        });
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

        StartCoroutine(HideLockersRoutine(() => {
            gameObject.SetActive(false);
            _deckManager.gameObject.SetActive(true);
        }));
        SoundManager.PlayMusicNow(MusicTracksEnum.Gameplay);
    }

    IEnumerator HideLockersRoutine(Action onComplete = null)
    {
        _moving = true;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            var pos = _lockerPositions[i].OriginalPosition - CardVertical * 2;
            _lockerPositions[i].RectTransform.DOAnchorPos(pos, _lockerAnimTime);
            yield return new WaitForSeconds(_betweenLockerDelay);
        }
        _rectTransform.DOAnchorPos(new Vector2(_rectTransform.anchoredPosition.x, Screen.height * 2), _lockerAnimTime).SetEase(Ease.InBack);
        yield return new WaitForSeconds(_lockerAnimTime);
        _moving = false;
        onComplete?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ResetGameLoadout();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
