using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InitCardDatabase : MonoBehaviour
{
    public string fileName;

    void Awake()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = ReadJsonFile(filePath);

        CardDatabase.Init(ConvertJsonToList(json));
    }

    public class CardListWrapper
    {
        public Card[] cards; // 需要封装在数组中
    }

    public string ReadJsonFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }

    public List<Card> ConvertJsonToList(string json)
    {
        // 使用 JsonUtility 将 JSON 字符串转为一个包含 Card 对象的数组
        CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>("{\"cards\":" + json + "}");
        List<Card> cardList = new List<Card>(wrapper.cards);
        return cardList;
    }
}
