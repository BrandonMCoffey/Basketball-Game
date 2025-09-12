using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New LineupType", menuName = "Balatro/Lineup Type")]
public class LineupType : ScriptableObject
{
    public enum ConditionType { None, HasTag, SameTeam }

    [Header("Lineup Properties")]
    public string LineupName;
    [TextArea] public string Description;
    public int Priority = 0;

    [Header("Scoring Bonuses")]
    public int BaseOR;
    public float DPM;

    [Header("Condition")]
    public ConditionType condition;
    public int requiredCount;
    public string requiredTagOrTeamID;

    public bool CheckCondition(List<CardUI> lineup)
    {
        if (lineup.Count < requiredCount) return false;

        switch (condition)
        {
            case ConditionType.None:
                return true;
            case ConditionType.HasTag:
                return lineup.Count(card => card.CardData.Tags.Contains(requiredTagOrTeamID)) >= requiredCount;
            case ConditionType.SameTeam:
                return lineup.GroupBy(card => card.CardData.TeamID).Any(group => group.Count() >= requiredCount);
        }
        return false;
    }
}