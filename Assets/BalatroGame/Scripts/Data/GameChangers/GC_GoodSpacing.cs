using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "GC_GoodSpacing", menuName = "Balatro/Game Changers/Good Spacing")]
public class GC_GoodSpacing : GameChanger
{
    public override bool OnScoreCalculationStart(ScoreData data)
    {
        bool hasCenter = data.Lineup.Any(card => card.CardData.Tags.Contains("Center"));
        if (!hasCenter)
        {
            data.PlaymakingModifier += 10;
            return true;
        }
        return false;
    }
}