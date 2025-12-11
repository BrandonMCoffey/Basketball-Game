using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] UIButtonController _playButton;
    [SerializeField] UIButtonController _tradeButton;
    [SerializeField] UIButtonController _leaderButton;
    [SerializeField] RectTransform _titlePanel;

    Vector2 _titlePanelOnScreenPos;

    private void Start() 
    {
        _titlePanelOnScreenPos = _titlePanel.anchoredPosition;
        _titlePanel.anchoredPosition = new Vector2(_titlePanelOnScreenPos.x, Screen.height);
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, 0.5f).SetEase(Ease.OutQuart).SetDelay(0.2f);
    }

    public void AnimateOnScreen()
    {
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, 0.5f).SetEase(Ease.OutQuart).SetDelay(0.2f);
        StartCoroutine(AnimateOnScreenRoutine());
    }

    IEnumerator AnimateOnScreenRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _playButton.AnimateOnScreen();
        _tradeButton.AnimateOnScreen();
        _leaderButton.AnimateOnScreen();
    }

    public void AnimateOffScreen(Action callback = null)
    {
        _titlePanel.DOAnchorPos(new Vector2(_titlePanelOnScreenPos.x, Screen.height), 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            callback?.Invoke();
        });
        _playButton.AnimateOffScreen();
        _tradeButton.AnimateOffScreen();
        _leaderButton.AnimateOffScreen();
    }

}