using System.Collections.Generic;

public class ScoreData
{
    public List<CardUI> Lineup;
    public float OffensiveRating;
    public float PlaymakingModifier;

    public ScoreData(List<CardUI> lineup)
    {
        Lineup = lineup;
        OffensiveRating = 0;
        PlaymakingModifier = 0;
    }
}
