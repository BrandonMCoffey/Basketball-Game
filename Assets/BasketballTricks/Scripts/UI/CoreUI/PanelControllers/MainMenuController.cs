using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using Sirenix.OdinInspector;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] UIButtonController _playButton;
    [SerializeField] UIButtonController _shopButton;
    [SerializeField] UIButtonController _leaderButton;
    [SerializeField] UIButtonController _vaultButton;
    [SerializeField] RectTransform _titlePanel;

    [SerializeField, ReadOnly] Vector2 _titlePanelOnScreenPos;
    [SerializeField, ReadOnly] bool _isInitialized = false;
    [SerializeField, ReadOnly] bool _delayOnStart = true;


    public void AnimateOnScreen()
    {
        Initialize();
        AnimOn();
    }

    private void Initialize()
    {
        if (_isInitialized) return;
        _titlePanelOnScreenPos = _titlePanel.anchoredPosition;
        _titlePanel.anchoredPosition = new Vector2(_titlePanelOnScreenPos.x, Screen.height);
        _isInitialized = true;
    }

    private void AnimOn()
    {
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, 0.5f).SetEase(Ease.OutQuart).SetDelay(_delayOnStart ? 0.25f : 0f);
        _delayOnStart = false;
        StartCoroutine(AnimateOnScreenRoutine());
    }

    IEnumerator AnimateOnScreenRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _playButton.AnimateOnScreen();
        _shopButton.AnimateOnScreen();
        _leaderButton.AnimateOnScreen();
        _vaultButton.AnimateOnScreen();
    }

    public void AnimateOffScreen(Action callback = null)
    {
        _titlePanel.DOAnchorPos(new Vector2(_titlePanelOnScreenPos.x, Screen.height), 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            callback?.Invoke();
        });
        _playButton.AnimateOffScreen();
        _shopButton.AnimateOffScreen();
        _leaderButton.AnimateOffScreen();
        _vaultButton.AnimateOffScreen();
    }

}