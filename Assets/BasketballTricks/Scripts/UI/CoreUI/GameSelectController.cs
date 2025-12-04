using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

enum GameMode
{
    HalfTime,
    Zen
}
public class GameSelectController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] Ease _easeType = Ease.OutBack;
    
    [Header("Description Texts")]
    [SerializeField, TextArea(5, 10)] string _halfTimeDescriptionText;
    [SerializeField, TextArea(5, 10)] string _zenDescriptionText;
    [Header("References")]
    [SerializeField] RectTransform _leftSidePanel;
    [SerializeField] RectTransform _descriptionPanel;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] RectTransform _titlePanel;
    [SerializeField] RectTransform _halfTimeButton;
    [SerializeField] RectTransform _zenButton;
    [SerializeField] RectTransform _beginButton;
    [SerializeField] RectTransform _exampleVideo;
    [SerializeField] RawImage _halfTimeVideoImage;
    [SerializeField] RawImage _zenVideoImage;

    Vector2 _leftSidePanelOnScreenPos;
    Vector2 _leftSidePanelOnScreenPos2;
    Vector2 _descriptionPanelOnScreenPos;
    Vector2 _descriptionTextOnScreenPos;
    Vector2 _titlePanelOnScreenPos;
    Vector2 _halfTimeButtonOnScreenPos;
    Vector2 _zenButtonOnScreenPos;
    Vector2 _beginButtonOnScreenPos;
    Vector2 _exampleVideoOnScreenPos;

    bool _isDescriptionPanelOnScreen = false;
    GameMode _currentGameMode;

    void Start()
    {
        Initialize();
        AnimateOnScreen();
    }

    // Sets initial positions of UI elements
    void Initialize()
    {
        _leftSidePanelOnScreenPos = _leftSidePanel.anchoredPosition;
        _leftSidePanel.anchoredPosition = new Vector2(-Screen.width * 2, _leftSidePanelOnScreenPos.y);

        _descriptionPanelOnScreenPos = _descriptionPanel.anchoredPosition;
        _descriptionPanel.anchoredPosition = new Vector2(Screen.width * 2, _descriptionPanelOnScreenPos.y);

        _descriptionTextOnScreenPos = _descriptionText.rectTransform.anchoredPosition;
        _descriptionText.rectTransform.anchoredPosition = new Vector2(Screen.width * 2, _descriptionTextOnScreenPos.y);

        _titlePanelOnScreenPos = _titlePanel.anchoredPosition;
        _titlePanel.anchoredPosition = new Vector2(-Screen.width * 2, _titlePanelOnScreenPos.y);

        _halfTimeButtonOnScreenPos = _halfTimeButton.anchoredPosition;
        _halfTimeButton.anchoredPosition = new Vector2(-Screen.width * 2, _halfTimeButtonOnScreenPos.y);

        _zenButtonOnScreenPos = _zenButton.anchoredPosition;
        _zenButton.anchoredPosition = new Vector2(-Screen.width * 2, _zenButtonOnScreenPos.y);

        _beginButtonOnScreenPos = _beginButton.anchoredPosition;
        _beginButton.anchoredPosition = new Vector2(Screen.width * 2, _beginButtonOnScreenPos.y);

        _exampleVideoOnScreenPos = _exampleVideo.anchoredPosition;
        _exampleVideo.anchoredPosition = new Vector2(Screen.width * 2.5f, _exampleVideoOnScreenPos.y);

        _halfTimeVideoImage.enabled = false;
        _zenVideoImage.enabled = false;
    }

    // Animates UI elements onto the screen
    public void AnimateOnScreen()
    {
        _leftSidePanel.DOAnchorPos(_leftSidePanelOnScreenPos, 0.3f).SetEase(_easeType);
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, 0.3f).SetEase(_easeType).SetDelay(0.12f);
        _halfTimeButton.DOAnchorPos(_halfTimeButtonOnScreenPos, 0.3f).SetEase(_easeType).SetDelay(0.25f);
        _zenButton.DOAnchorPos(_zenButtonOnScreenPos, 0.3f).SetEase(_easeType).SetDelay(0.35f);
    }

    public void AnimateOffScreen(Action callback = null)
    {
        _leftSidePanel.DOAnchorPos(new Vector2(-Screen.width * 2, _leftSidePanelOnScreenPos.y), 0.3f).SetEase(_easeType);
        _titlePanel.DOAnchorPos(new Vector2(-Screen.width * 2, _titlePanelOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.12f);
        _halfTimeButton.DOAnchorPos(new Vector2(-Screen.width * 2, _halfTimeButtonOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.25f);
        _zenButton.DOAnchorPos(new Vector2(-Screen.width * 2, _zenButtonOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.35f).OnComplete(() =>
        {
            callback?.Invoke();
        });

        _descriptionPanel.DOAnchorPos(new Vector2(Screen.width * 2, _descriptionPanelOnScreenPos.y), 0.3f).SetEase(_easeType);
        _descriptionText.rectTransform.DOAnchorPos(new Vector2(Screen.width * 2, _descriptionTextOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.1f);
        _exampleVideo.DOAnchorPos(new Vector2(Screen.width * 2.5f, _exampleVideoOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.2f);
        _beginButton.DOAnchorPos(new Vector2(Screen.width * 2, _beginButtonOnScreenPos.y), 0.3f).SetEase(_easeType).SetDelay(0.3f);
        _isDescriptionPanelOnScreen = false;
    }

    public void AnimateDescriptionPanelOnScreen()
    {
        _exampleVideo.DOAnchorPos(_exampleVideoOnScreenPos, 0.3f).SetEase(_easeType);
        _descriptionPanel.DOAnchorPos(_descriptionPanelOnScreenPos, 0.3f).SetDelay(0.2f).SetEase(_easeType);
        _descriptionText.rectTransform.DOAnchorPos(_descriptionTextOnScreenPos, 0.3f).SetDelay(0.3f).SetEase(_easeType);
        _beginButton.DOAnchorPos(_beginButtonOnScreenPos, 0.3f).SetDelay(0.4f).SetEase(_easeType);
        _isDescriptionPanelOnScreen = true;
    }

    void AnimateDescriptionBounce()
    {
        _exampleVideo.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.2f).SetEase(_easeType);
        _descriptionPanel.DOAnchorPosY(_descriptionPanelOnScreenPos.y + 50f, 0.2f).SetEase(_easeType).OnComplete(() =>
        {
            _descriptionPanel.DOAnchorPosY(_descriptionPanelOnScreenPos.y, 0.3f).SetEase(Ease.OutBounce);
            _exampleVideo.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        });
    }

    public void HalfTimeShowSelect()
    {
        _descriptionText.text = _halfTimeDescriptionText;
        _halfTimeVideoImage.enabled = true;
        _zenVideoImage.enabled = false;
        
        if (!_isDescriptionPanelOnScreen)
        {
            AnimateDescriptionPanelOnScreen();
            return;
        }
        
        if (_currentGameMode == GameMode.HalfTime) return;
        AnimateDescriptionBounce();   
        _currentGameMode = GameMode.HalfTime;
    }

    public void SandboxSelect()
    {
        _descriptionText.text = _zenDescriptionText;
        _halfTimeVideoImage.enabled = false;
        _zenVideoImage.enabled = true;
        
        if (!_isDescriptionPanelOnScreen)
        {
            AnimateDescriptionPanelOnScreen();
            return;
        }

        if (_currentGameMode == GameMode.Zen) return;
        AnimateDescriptionBounce();
        _currentGameMode = GameMode.Zen;
    }

    public void Begin()
    {
        if (_currentGameMode == GameMode.HalfTime)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("HalftimeShow");
        }
        else if (_currentGameMode == GameMode.Zen)
        {
            Debug.Log("Zen Mode Selected - Scene Not Yet Implemented");
        }
    }
}
