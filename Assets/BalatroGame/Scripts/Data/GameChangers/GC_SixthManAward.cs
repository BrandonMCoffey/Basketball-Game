using UnityEngine;

[CreateAssetMenu(fileName = "GC_SixthManAward", menuName = "Balatro/Game Changers/Sixth Man Award")]
public class GC_SixthManAward : GameChanger
{
    private bool _hasTriggeredThisHand = false;

    public override void OnHandPlayStart()
    {
        _hasTriggeredThisHand = false;
    }

    public override bool OnCardContributesScore(CardUI card, ScoreData data)
    {
        if (!_hasTriggeredThisHand)
        {
            data.OffensiveRating += card.CardData.PPG;
            data.PlaymakingModifier += card.CardData.APG;
            _hasTriggeredThisHand = true;
            return true;
        }
        return false;
    }
}