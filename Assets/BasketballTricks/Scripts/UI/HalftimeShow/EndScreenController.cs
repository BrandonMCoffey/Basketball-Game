using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class EndScreenController : MonoBehaviour
{

    struct EndScreenData
    {
        public string PlayerNumber;
        public string PlayerName;
        public string PlayerPos;
        public int HypeScored;
        public int XP;

        public EndScreenData(string playerNumber, string playerName, string _playerPos, int hypeScored, int _xp)
        {
            PlayerNumber = playerNumber;
            PlayerName = playerName;
            PlayerPos = _playerPos;
            HypeScored = hypeScored;
            XP = _xp;
        }

        
    }

    [SerializeField] ActionDeckManager _actionDeckManager;
    [SerializeField] List<EndScreenPanel> _endScreenPanels = new List<EndScreenPanel>();
    List<EndScreenData> _endScreenDataList = new List<EndScreenData>();
    [SerializeField] RectTransform _cardRectTransform;
    [SerializeField] Image _backgroundImage;
    [SerializeField] RectTransform _backButtonRectTransform;
    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _goalScoreText;
    [SerializeField] TMP_Text _finalScoreText;

    Vector2 _buttonPosition;
    Vector2 _cardPosition;
    private void Awake() 
    {
        _cardPosition = _cardRectTransform.anchoredPosition;
        _cardRectTransform.anchoredPosition = new Vector2(_cardRectTransform.anchoredPosition.x, -1300f);
        _cardRectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        _backgroundImage.color = new Color(1f, 1f, 1f, 0f);
        _backgroundImage.raycastTarget = false;

        _titleText.color = new Color(0.3726415f, 0.609691f, 1f, 0f);
        _titleText.raycastTarget = false;

        _buttonPosition = _backButtonRectTransform.anchoredPosition;
        _backButtonRectTransform.anchoredPosition = new Vector2(_backButtonRectTransform.anchoredPosition.x, -Screen.height -500f);
    }

    private void OnEnable() {
        GameManager.OnGameEnded += ShowEndScreen;
    }

    private void OnDisable() {
        GameManager.OnGameEnded -= ShowEndScreen;
    }

    public void ShowEndScreen()
    {
        LoadEndScreenData();
        PopulateEndScreenPanels();
        AnimateEndScreen(true);
    }

    public void HideEndScreen()
    {
        AnimateEndScreen(false);
    }

    void LoadEndScreenData()
    {
        var playerList = PlayerManager.Instance.Players;

        foreach (var player in playerList)
        {
            var endScreenData = new EndScreenData(
                player.CardData.PlayerNumber, 
                player.CardData.PlayerName, 
                GetPlayerPosition(player.Position), 
                Mathf.CeilToInt(player.CardData.HypeScored), 
                Mathf.CeilToInt(player.CardData.XP)
            );

            _endScreenDataList.Add(endScreenData);
        }

        // no joke, this is basically the only reason EndScreenData exists
        _endScreenDataList = _endScreenDataList.OrderByDescending(x => x.HypeScored).ToList();
    }

    void PopulateEndScreenPanels()
    {
        for (int i = 0; i < _endScreenDataList.Count; i++)
        {
            var endScreenData = _endScreenDataList[i];
            var endScreenPanel = _endScreenPanels[i];

            endScreenPanel.Init(endScreenData.PlayerNumber, endScreenData.PlayerName, endScreenData.PlayerPos, endScreenData.HypeScored, endScreenData.XP);
        }

        _goalScoreText.text = $"{_actionDeckManager.GoalValue}";
        _finalScoreText.text = $"{PlayerManager.Instance.Hype}";
    }

    void AnimateEndScreen(bool on)
    {
        if (on) 
        {
            _cardRectTransform.DOAnchorPos(_cardPosition, 1f).SetEase(Ease.OutQuad);
            _cardRectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.OutQuad);
            _backButtonRectTransform.DOAnchorPos(_buttonPosition, 1f)
                .SetEase(Ease.OutQuad)
                .SetDelay(0.15f);

            _backgroundImage.raycastTarget = true;
            _backgroundImage.DOFade(1f, 1f).SetEase(Ease.OutQuad);
            _titleText.DOFade(0.3176471f, 1f).SetEase(Ease.OutQuad);
        }
        else
        {
            _backButtonRectTransform.DOAnchorPos(new Vector2(_backButtonRectTransform.anchoredPosition.x, -1300f), 1f)
                .SetEase(Ease.OutQuad);
            
            _backgroundImage.DOFade(0f, 1f).SetEase(Ease.OutQuad);
            _cardRectTransform.DOAnchorPos(new Vector2(_cardRectTransform.anchoredPosition.x, -Screen.height + 500f), 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    _cardRectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                    _backgroundImage.raycastTarget = false;
                }
            );

            _titleText.DOFade(0f, 1f).SetEase(Ease.OutQuad);
        }
    }

    string GetPlayerPosition(PlayerPosition playerPos)
    {
        switch (playerPos)
        {
            case PlayerPosition.C:
                return "Center";
            case PlayerPosition.PF:
                return "Power Forward";
            case PlayerPosition.SF:
                return "Small Forward";
            case PlayerPosition.PG:
                return "Point Guard";
            case PlayerPosition.SG:
                return "Shooting Guard";
            default:
                return "N/A";
        }
    }

    
}


