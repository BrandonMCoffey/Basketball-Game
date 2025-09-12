using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [SerializeField] private TextAsset _playerJsonFile;
    public Dictionary<string, CardData> AllCards { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        AllCards = new Dictionary<string, CardData>();
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        PlayerDataList playerList = JsonUtility.FromJson<PlayerDataList>(_playerJsonFile.text);

        foreach (PlayerStats stats in playerList.players)
        {
            CardData newCard = ScriptableObject.CreateInstance<CardData>();
            newCard.InitializeData(stats.ID, stats.PlayerName, stats.TeamID, stats.JerseyNumber, stats.PPG, stats.APG, stats.RPG, stats.SPG_BPG, stats.ThreeP_Percent, stats.Tags);
            AllCards.Add(stats.ID, newCard);
        }
    }

    [System.Serializable]
    private class PlayerDataList
    {
        public List<PlayerStats> players;
    }

    [System.Serializable]
    private class PlayerStats
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
    }
}