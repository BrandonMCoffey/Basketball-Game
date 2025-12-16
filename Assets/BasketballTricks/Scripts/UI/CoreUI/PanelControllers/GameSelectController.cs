using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Video;

enum GameMode
{
    HalfTime,
    Zen,
    Tutorial
}
public class GameSelectController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] Ease _easeType = Ease.OutBack;
    [SerializeField] float _transitionTimeOnScreen = 0.5f;
    [SerializeField] float _transitionTimeOffScreen = 0.75f;
    [SerializeField] float _transitionTimeDesc = 0.4f;
    
    [Header("Description Texts")]
    [SerializeField, TextArea(5, 10)] string _tutorialDescriptionText;
    [SerializeField, TextArea(5, 10)] string _halfTimeDescriptionText;
    [SerializeField, TextArea(5, 10)] string _zenDescriptionText;
    [Header("References")]
    [SerializeField] RectTransform _leftSidePanel;
    [SerializeField] RectTransform _descriptionPanel;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] RectTransform _titlePanel;
    [SerializeField] RectTransform _halfTimeButton;
    [SerializeField] RectTransform _zenButton;
    [SerializeField] RectTransform _backButton;
    [SerializeField] RectTransform _tutorialButton;
    [SerializeField] RectTransform _beginButton;
    [SerializeField] RectTransform _exampleVideo;
    [SerializeField] RawImage _halfTimeVideoImage;
    [SerializeField] RawImage _zenVideoImage;
    [SerializeField] RawImage _tutorialVideoImage;
    [SerializeField] VideoPlayer _halfTimeVideoPlayer;
    [SerializeField] VideoPlayer _zenVideoPlayer;
    [SerializeField] VideoPlayer _tutorialVideoPlayer;

    Vector2 _leftSidePanelOnScreenPos;
    Vector2 _descriptionPanelOnScreenPos;
    Vector2 _descriptionTextOnScreenPos;
    Vector2 _titlePanelOnScreenPos;
    Vector2 _halfTimeButtonOnScreenPos;
    Vector2 _zenButtonOnScreenPos;
    Vector2 _backButtonOnScreenPos;
    Vector2 _tutorialButtonOnScreenPos;
    Vector2 _beginButtonOnScreenPos;
    Vector2 _exampleVideoOnScreenPos;

    bool _isDescriptionPanelOnScreen = false;
    bool _positionsInitialized = false;
    GameMode _currentGameMode;

    private void Start() 
    {
        AnimateOnScreen();   
    }

    public void AnimateOnScreen()
    {
        Initialize();
        AnimOnScreen();
    }

    private void Initialize()
    {
        if (_positionsInitialized) return;
        _positionsInitialized = true;
        _leftSidePanelOnScreenPos = _leftSidePanel.anchoredPosition;
        _leftSidePanel.anchoredPosition = new Vector2(-Screen.width * 2, _leftSidePanelOnScreenPos.y);

        _descriptionPanelOnScreenPos = _descriptionPanel.anchoredPosition;
        _descriptionPanel.anchoredPosition = new Vector2(Screen.width * 3, _descriptionPanelOnScreenPos.y);

        _descriptionTextOnScreenPos = _descriptionText.rectTransform.anchoredPosition;
        _descriptionText.rectTransform.anchoredPosition = new Vector2(Screen.width * 3, _descriptionTextOnScreenPos.y);

        _titlePanelOnScreenPos = _titlePanel.anchoredPosition;
        _titlePanel.anchoredPosition = new Vector2(-Screen.width * 2, _titlePanelOnScreenPos.y);

        _halfTimeButtonOnScreenPos = _halfTimeButton.anchoredPosition;
        _halfTimeButton.anchoredPosition = new Vector2(-Screen.width * 2, _halfTimeButtonOnScreenPos.y);

        _zenButtonOnScreenPos = _zenButton.anchoredPosition;
        _zenButton.anchoredPosition = new Vector2(-Screen.width * 2, _zenButtonOnScreenPos.y);

        _backButtonOnScreenPos = _backButton.anchoredPosition;
        _backButton.anchoredPosition = new Vector2(-Screen.width * 2, _backButtonOnScreenPos.y);

        _tutorialButtonOnScreenPos = _tutorialButton.anchoredPosition;
        _tutorialButton.anchoredPosition = new Vector2(Screen.width * 2f, _tutorialButtonOnScreenPos.y);

        _beginButtonOnScreenPos = _beginButton.anchoredPosition;
        _beginButton.anchoredPosition = new Vector2(Screen.width * 2, _beginButtonOnScreenPos.y);

        _exampleVideoOnScreenPos = _exampleVideo.anchoredPosition;
        _exampleVideo.anchoredPosition = new Vector2(Screen.width * 2.5f, _exampleVideoOnScreenPos.y);

        _halfTimeVideoImage.enabled = false;
        _zenVideoImage.enabled = false;
    }

    private void AnimOnScreen()
    {
        _halfTimeVideoPlayer.enabled = true;
        _zenVideoPlayer.enabled = true;
        _tutorialVideoPlayer.enabled = true;
        _leftSidePanel.DOAnchorPos(_leftSidePanelOnScreenPos, _transitionTimeOnScreen).SetEase(_easeType);
        _titlePanel.DOAnchorPos(_titlePanelOnScreenPos, _transitionTimeOnScreen).SetEase(Ease.OutQuart).SetDelay(0.12f);
        _tutorialButton.DOAnchorPos(_tutorialButtonOnScreenPos, _transitionTimeOnScreen).SetEase(_easeType).SetDelay(0.25f);
        _halfTimeButton.DOAnchorPos(_halfTimeButtonOnScreenPos, _transitionTimeOnScreen).SetEase(_easeType).SetDelay(0.35f);
        _zenButton.DOAnchorPos(_zenButtonOnScreenPos, _transitionTimeOnScreen).SetEase(_easeType).SetDelay(0.45f);
        _backButton.DOAnchorPos(_backButtonOnScreenPos, _transitionTimeOnScreen).SetEase(_easeType).SetDelay(0.55f);
    }

    public void AnimateOffScreen(Action callback = null)
    {
        _leftSidePanel.DOAnchorPos(new Vector2(-Screen.width, _leftSidePanelOnScreenPos.y), _transitionTimeOffScreen).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            _halfTimeVideoPlayer.enabled = false;
            _zenVideoPlayer.enabled = false;
            _tutorialVideoPlayer.enabled = false;
            callback?.Invoke();
        });
        _titlePanel.DOAnchorPos(new Vector2(-500, _titlePanelOnScreenPos.y), _transitionTimeOffScreen).SetEase(Ease.OutQuart).SetDelay(0.12f);
        _tutorialButton.DOAnchorPos(new Vector2(-Screen.width, _tutorialButtonOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.25f);
        _halfTimeButton.DOAnchorPos(new Vector2(-Screen.width, _halfTimeButtonOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.35f);
        _zenButton.DOAnchorPos(new Vector2(-Screen.width, _zenButtonOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.45f);
        _backButton.DOAnchorPos(new Vector2(-Screen.width, _backButtonOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.55f);

        _descriptionPanel.DOAnchorPos(new Vector2(Screen.width * 3, _descriptionPanelOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType);
        _descriptionText.rectTransform.DOAnchorPos(new Vector2(Screen.width * 3, _descriptionTextOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.1f);
        _exampleVideo.DOAnchorPos(new Vector2(Screen.width * 2.5f, _exampleVideoOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.2f);
        _beginButton.DOAnchorPos(new Vector2(Screen.width * 2, _beginButtonOnScreenPos.y), _transitionTimeOffScreen).SetEase(_easeType).SetDelay(0.3f);
        _isDescriptionPanelOnScreen = false;
    }

    public void AnimateDescriptionPanelOnScreen()
    {
        _exampleVideo.DOAnchorPos(_exampleVideoOnScreenPos, _transitionTimeDesc).SetEase(_easeType);
        _descriptionPanel.DOAnchorPos(_descriptionPanelOnScreenPos, _transitionTimeDesc).SetDelay(0.2f).SetEase(_easeType);
        _descriptionText.rectTransform.DOAnchorPos(_descriptionTextOnScreenPos, _transitionTimeDesc).SetDelay(0.3f).SetEase(_easeType);
        _beginButton.DOAnchorPos(_beginButtonOnScreenPos, _transitionTimeDesc).SetDelay(0.4f).SetEase(_easeType);
        _isDescriptionPanelOnScreen = true;
    }

    private void AnimateDescriptionBounce(Action functionality = null)
    {
        _exampleVideo.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.2f).SetEase(_easeType);
        _descriptionPanel.DOAnchorPosY(_descriptionPanelOnScreenPos.y + 50f, 0.2f).SetEase(_easeType).OnComplete(() =>
        {
            functionality?.Invoke();
            _descriptionPanel.DOAnchorPosY(_descriptionPanelOnScreenPos.y, 0.3f).SetEase(Ease.OutBounce);
            _exampleVideo.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        });
    }

    public void HalfTimeShowSelect()
    {
        _currentGameMode = GameMode.HalfTime;
        if (!_isDescriptionPanelOnScreen)
        {
            _descriptionText.text = _halfTimeDescriptionText;
            _halfTimeVideoImage.enabled = true;
            _zenVideoImage.enabled = false;
            _tutorialVideoImage.enabled = false;
            AnimateDescriptionPanelOnScreen();
            return;
        }
        
        AnimateDescriptionBounce(() =>
        {
            _descriptionText.text = _halfTimeDescriptionText;
            _halfTimeVideoImage.enabled = true;
            _zenVideoImage.enabled = false;
            _tutorialVideoImage.enabled = false;
        });   
    }

    public void SandboxSelect()
    {
        _currentGameMode = GameMode.Zen;
        if (!_isDescriptionPanelOnScreen)
        {
            _descriptionText.text = _zenDescriptionText;
            _halfTimeVideoImage.enabled = false;
            _zenVideoImage.enabled = true;
            _tutorialVideoImage.enabled = false;
            AnimateDescriptionPanelOnScreen();
            return;
        }

        AnimateDescriptionBounce(() =>
        {
            _descriptionText.text = _zenDescriptionText;
            _halfTimeVideoImage.enabled = false;
            _zenVideoImage.enabled = true;
            _tutorialVideoImage.enabled = false;
        });
    }

    public void TutorialSelect()
    {
        _currentGameMode = GameMode.Tutorial;
        if (!_isDescriptionPanelOnScreen)
        {
            _descriptionText.text = _tutorialDescriptionText;
            _tutorialVideoImage.enabled = true;
            _halfTimeVideoImage.enabled = false;
            _zenVideoImage.enabled = false;
            AnimateDescriptionPanelOnScreen();
            return;
        }

        AnimateDescriptionBounce(() =>
        {
            _descriptionText.text = _tutorialDescriptionText;
            _tutorialVideoImage.enabled = true;
            _halfTimeVideoImage.enabled = false;
            _zenVideoImage.enabled = false;
        });
    }

    public void Tutorial()
    {
        AnimateOffScreen(() =>
        {
            GameManager.Instance.LoadTutorialScene();
        });
    }

    public void Begin()
    {
        AnimateOffScreen(() =>
        {
            LoadSelectedGameModeScene();
        });
        
    }

    private void LoadSelectedGameModeScene()
    {
        switch (_currentGameMode)
        {
            case GameMode.HalfTime:
                GameManager.Instance.LoadGameScene();
                break;
            case GameMode.Zen:
                GameManager.Instance.LoadSandboxScene();
                break;
            case GameMode.Tutorial:
                GameManager.Instance.LoadTutorialScene();
                break;
            default:
                Debug.LogError("No game mode selected!");
                break;
        }
    }
}