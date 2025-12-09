using UnityEngine;
using DG.Tweening;
using System.Collections;

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

    public void Enable()
    {
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, 0.5f).SetEase(Ease.OutQuart).SetDelay(0.2f);
        StartCoroutine(AnimateOnScreenRoutine());
    }

    IEnumerator AnimateOnScreenRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _playButton.gameObject.SetActive(true);
        _tradeButton.gameObject.SetActive(true);
        _leaderButton.gameObject.SetActive(true);
    }

    public void AnimateOffScreen()
    {
        _titlePanel.DOAnchorPos(new Vector2(_titlePanelOnScreenPos.x, Screen.height), 0.5f).SetEase(Ease.OutQuart);
        _playButton.AnimateOffScreen();
        _tradeButton.AnimateOffScreen();
        _leaderButton.AnimateOffScreen();

        Invoke("HideButtons", 1.5f);
    }

    void HideButtons()
    {
        _playButton.gameObject.SetActive(false);
        _tradeButton.gameObject.SetActive(false);
        _leaderButton.gameObject.SetActive(false);
    }
}