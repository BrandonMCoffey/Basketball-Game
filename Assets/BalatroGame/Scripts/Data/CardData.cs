using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New CardData", menuName = "Balatro/Card Data")]
public class CardData : ScriptableObject
{
    public string ID;
    public string PlayerName;
    public string TeamID;
    public int JerseyNumber;
    public float PPG;
    public float APG;
    public float RPG;
    public float SPG_BPG;
    public float ThreeP_Percent;
    public List<string> Tags;

    public void InitializeData(string id, string playerName, string teamID, int jerseyNumber, float ppg, float apg, float rpg, float spg_bpg, float threeP_Percent, List<string> tags)
    {
        ID = id;
        PlayerName = playerName;
        TeamID = teamID;
        JerseyNumber = jerseyNumber;
        PPG = ppg;
        APG = apg;
        RPG = rpg;
        SPG_BPG = spg_bpg;
        ThreeP_Percent = threeP_Percent;
        Tags = tags;
    }
}