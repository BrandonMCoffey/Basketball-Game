using UnityEngine;

[System.Serializable]
public class GameCard
{
    [SerializeField] private PlayerCardData _cardData;
    [SerializeField] private float _xp;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _matchesPlayed;
    [SerializeField] private float _hypeScored;
    [SerializeField] private int _shotsMade;
    [SerializeField] private int _passesMade;
    [SerializeField] private int _tricksMade;

    public GameCard(PlayerCardData data)
    {
        _cardData = data;
        _xp = 0;
        _level = 1;
    }

    public GameCard(PlayerCardData data, GameCardSaveData saveData)
    {
        _cardData = data;
        _xp = saveData.XP;
        _level = saveData.Level;
        _matchesPlayed = saveData.MatchesPlayed;
        _hypeScored = saveData.HypeScored;
        _shotsMade = saveData.ShotsMade;
        _passesMade = saveData.PassesMade;
        _tricksMade = saveData.TricksMade;
    }

    public int TricksMade => _tricksMade;
    public float XP => _xp;
    public int Level => _level;
    public int MatchesPlayed => _matchesPlayed;
    public float HypeScored => _hypeScored;
    public int ShotsMade => _shotsMade;
    public int PassesMade => _passesMade;

    public PlayerData PlayerData => _cardData.Player;
    public PlayerCardData CardDataSO => _cardData;
    public string CardID => _cardData.CardID;
    public string PlayerName => _cardData.Player.PlayerName;
    public Sprite PlayerSprite => _cardData.PlayerSprite;
    public string PlayerNumber => _cardData.Player.PlayerNumber;
    public void SetPlayerArt(PlayerArt art) => _cardData.SetPlayerArt(art);
    public int ActionCount => _cardData.ActionCount;
    public int GetActionCount(int index) => _cardData.GetActionCount(index);
    public ActionData GetAction(int index)
    {
        var data = _cardData.GetAction(index);
        data.ActionLevel = _level;
        return data;
    }
    public int GetActionIndex(PlayerActionData actionData) => _cardData.GetActionIndex(actionData);

    public void PrepareForGameplay()
    {
        _cardData.PrepareForGameplay();
    }
    public void IncrementMatchesPlayed()
    {
        _matchesPlayed++;
    }
    public void IncrementHypeScored(float amount)
    {
        _hypeScored += amount;
    }
    public void IncrementAction(ActionType type)
    {
        switch (type)
        {
            case ActionType.Trick:
                _tricksMade++;
                break;
            case ActionType.Pass:
                _passesMade++;
                break;
            case ActionType.Shot:
                _shotsMade++;
                break;
        }
    }

    public void AddXP(float amount)
    {
        if (_level >= 3) return;
        _xp += amount;
        float xpForNextLevel = (_level + 1) * 100f;
        while (_xp >= xpForNextLevel)
        {
            _xp -= xpForNextLevel;
            _level++;
            xpForNextLevel = (_level + 1) * 100f;
        }
    }

    public string GetStatValueByIndex(int index)
    {
        return index switch
        {
            0 => _matchesPlayed.ToString(),
            1 => _hypeScored.ToString("F0"),
            2 => _shotsMade.ToString(),
            3 => _passesMade.ToString(),
            4 => _tricksMade.ToString(),
            _ => "",
        };
    }

    public string GetStatData()
    {
        return $"{_cardData.Player.PlayerNumber} ({_cardData.CardTitle}): level {_level} ({_xp} XP). {_matchesPlayed} matches, {_shotsMade} shots, {_passesMade} passes, {_tricksMade} tricks.";
    }
}