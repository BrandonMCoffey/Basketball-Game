using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameCardSaveData
{
    public string CardID;
    public float XP;
    public int Level;
    public int MatchesPlayed;
    public float HypeScored;
    public int ShotsMade;
    public int PassesMade;
    public int TricksMade;
}

[System.Serializable]
public class GameSaveData
{
#if UNITY_EDITOR
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "SaveData-Editor.json");
#else
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "SaveData.json");
#endif

    public int Money;

    public List<GameCardSaveData> OwnedCardData = new List<GameCardSaveData>();


    public GameSaveData(int money, List<PlayerCardData> startingCards)
    {
        Money = money;
        OwnedCardData = new List<GameCardSaveData>(startingCards.Count);
        foreach (var cardData in startingCards)
        {
            OwnedCardData.Add(new GameCardSaveData { 
                CardID = cardData.CardID,
                Level = 1,
            });
        }
    }

    public static bool Load(out GameSaveData data)
    {
        data = null;
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                data = JsonUtility.FromJson<GameSaveData>(json);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load saved cards: {e.Message}");
            }
        }
        return false;
    }

    public static void Save(GameSaveData dataToSave)
    {
        string json = JsonUtility.ToJson(dataToSave, true);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log("Game saved!");
    }
}
