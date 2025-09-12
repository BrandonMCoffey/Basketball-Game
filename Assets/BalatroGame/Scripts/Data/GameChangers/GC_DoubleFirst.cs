using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "GC_DoubleFirst", menuName = "Balatro/Game Changers/Double First")]
public class GC_DoubleFirst : GameChanger
{
    public override bool OnScoreCalculationStart(ScoreData data)
    {
        if (data.Lineup.Count > 0)
        {
            CardUI unicorn = data.Lineup.OrderByDescending(card => card.CardData.RPG).First();
            data.OffensiveRating += unicorn.CardData.RPG;
            return true;
        }
        return false;
    }
}