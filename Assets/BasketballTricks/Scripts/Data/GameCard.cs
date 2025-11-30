using UnityEngine;

[System.Serializable]
public class GameCard
{
    [SerializeField] private PlayerCardData _cardData;
    [SerializeField] private float _xp;
    [SerializeField] private int _level = 1;

    public GameCard(PlayerCardData data)
    {
        _cardData = data;
        _xp = 0;
        _level = 1;
    }

    public PlayerData PlayerData => _cardData.PlayerData;
    public int ActionCount => _cardData.ActionCount;
    public int GetActionCount(int index) => _cardData.GetActionCount(index);
    public ActionData GetAction(int index)
    {
        var data = _cardData.GetAction(index);
        data.ActionLevel = _level;
        return data;
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
}