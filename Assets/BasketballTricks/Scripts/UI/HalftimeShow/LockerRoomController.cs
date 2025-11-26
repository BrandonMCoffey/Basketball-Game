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
    [SerializeField] private Transform _selectedPosition;
    [SerializeField] private Button _letsGoButton;
    [SerializeField] private Transform _catalog;
    [SerializeField] private Ease _lockerEaseEnter = Ease.InQuart;
    [SerializeField] private Ease _lockerEaseLeave = Ease.InQuart;
    [SerializeField] private float _lockerDelay = 0.05f;
    [SerializeField] private ActionDeckManager _deckManager;

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
        _letsGoButton.transform.position = _letsGoOriginalPosition + Vector3.down * 400f;
        _catalogOriginalPosition = _catalog.position;
        _catalog.position = _catalogOriginalPosition + Vector3.right * Screen.width;
        StartCoroutine(StartShowLockersRoutine());
    }

    public void SelectRandomPlayers(bool allNaturalPositions)
    {
        var players = GameManager.Instance.OwnedPlayers;
        players.Shuffle();
        if (allNaturalPositions)
        {
            for (int i = 0; i < _lockerPositions.Count; i++)
            {
                var player = players.FirstOrDefault(p => p.PlayerData.IsNaturalPosition(_lockerPositions[i].Position));
                if (player == null) player = players[0];
                _lockerPositions[i].AddCard(player);
                players.Remove(player);
            }
        }
        else
        {
            for (int i = 0; i < _lockerPositions.Count; i++)
            {
                _lockerPositions[i].AddCard(players[i]);
            }
        }
    }


    public IEnumerator StartShowLockersRoutine()
    {
        _moving = true;
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].transform.position = _lockerPositions[i].OriginalPosition + Vector3.down * Screen.height;
        }
        for (int i = 0; i < _lockerPositions.Count; i++)
        {
            _lockerPositions[i].transform.DOMove(_lockerPositions[i].OriginalPosition, 0.5f).SetEase(_lockerEaseEnter);
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
        if (_selectedIndex == index)
        {
            _catalog.DOMove(_catalogOriginalPosition + Vector3.right * Screen.width, 1f).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(0.5f);
            foreach (var locker in _lockerPositions)
            {
                var pos = locker.OriginalPosition + (locker.Card != null ? Vector3.up * 100f : Vector3.zero);
                locker.transform.DOMove(pos, 0.5f).SetEase(_lockerEaseEnter);
                yield return new WaitForSeconds(_lockerDelay);
            }
            yield return new WaitForSeconds(0.1f);
            bool buttonInteractable = _lockerPositions.All(selector => selector.Card != null);
            _letsGoButton.interactable = buttonInteractable;
            if (buttonInteractable) _letsGoButton.transform.DOMove(_letsGoOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
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
                _lockerPositions[i].transform.DOMove(pos, 0.5f).SetEase(_lockerEaseLeave);
                yield return new WaitForSeconds(_lockerDelay);
            }
        }
        _letsGoButton.transform.DOMove(_letsGoOriginalPosition + Vector3.down * 400f, 0.5f).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.25f);
        _catalog.DOMove(_catalogOriginalPosition, 0.5f).SetEase(Ease.InOutCubic);
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
