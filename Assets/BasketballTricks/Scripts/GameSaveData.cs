using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "SaveData.json");

    // Note: Every field listed below will be saved and loaded automatically via JsonUtility
    public List<GameCard> OwnedCards = new List<GameCard>();


    public GameSaveData(List<PlayerCardData> startingCards)
    {
        OwnedCards = new List<GameCard>(startingCards.Count);
        foreach (var cardData in startingCards)
        {
            OwnedCards.Add(new GameCard(cardData));
        }
    }

    public static bool Load(ref GameSaveData data)
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonUtility.FromJson<GameSaveData>(json);
                if (saveData.OwnedCards.Count > 0)
                {
                    data = saveData;
                    return true;
                }
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
        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(dataToSave));
    }
}
