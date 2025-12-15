using System;
using DG.Tweening;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] RectTransform _playerData;
    [SerializeField] RectTransform _backButton;
    [SerializeField] RectTransform _shopButton1;
    [SerializeField] RectTransform _shopButton2;
    [SerializeField] RectTransform _title;

    Vector2 _playerDataOnScreenPos;
    Vector2 _backButtonOnScreenPos;
    Vector2 _shopButton1OnScreenPos;
    Vector2 _shopButton2OnScreenPos;
    Vector2 _titleOnScreenPos;
    bool _isInitialized = false;

    void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        _playerDataOnScreenPos = _playerData.anchoredPosition;
        _backButtonOnScreenPos = _backButton.anchoredPosition;
        _shopButton1OnScreenPos = _shopButton1.anchoredPosition;
        _shopButton2OnScreenPos = _shopButton2.anchoredPosition;
        _titleOnScreenPos = _title.anchoredPosition;

        _playerData.anchoredPosition = new Vector2(-2000, _playerDataOnScreenPos.y);
        _backButton.anchoredPosition = new Vector2(-1000, _backButtonOnScreenPos.y);
        _shopButton1.anchoredPosition = new Vector2(_shopButton1OnScreenPos.x, -1000);
        _shopButton2.anchoredPosition = new Vector2(_shopButton2OnScreenPos.x, -1000);
        _title.anchoredPosition = new Vector2(_titleOnScreenPos.x, 500);
    }

    public void AnimateOnScreen()
    {
        Initialize();
        _playerData.DOAnchorPos(_playerDataOnScreenPos, 0.5f).SetEase(Ease.OutBack);
        _backButton.DOAnchorPos(_backButtonOnScreenPos, 0.5f).SetEase(Ease.OutBack).SetDelay(0.1f);
        _shopButton1.DOAnchorPos(_shopButton1OnScreenPos, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
        _shopButton2.DOAnchorPos(_shopButton2OnScreenPos, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f);
        _title.DOAnchorPos(_titleOnScreenPos, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f);
    }

    public void AnimateOffScreen(Action callback)
    {
        _playerData.DOAnchorPos(new Vector2(-2000, _playerDataOnScreenPos.y), 0.5f).SetEase(Ease.InBack);
        _backButton.DOAnchorPos(new Vector2(-1000, _backButtonOnScreenPos.y), 0.5f).SetEase(Ease.InBack).SetDelay(0.1f);
        _shopButton1.DOAnchorPos(new Vector2(_shopButton1OnScreenPos.x, -1000), 0.5f).SetEase(Ease.InBack).SetDelay(0.2f);
        _shopButton2.DOAnchorPos(new Vector2(_shopButton2OnScreenPos.x, -1000), 0.5f).SetEase(Ease.InBack).SetDelay(0.3f);
        _title.DOAnchorPos(new Vector2(_titleOnScreenPos.x, 500), 0.5f).SetEase(Ease.InBack).SetDelay(0.4f).OnComplete(() => callback());
    }
}