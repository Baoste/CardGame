using Game.Domain;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardJsonUtility
{
    private class CardListWrapper<T>
    {
        public List<T> cards;
    }

    public static void ConvertCardsToJson<T>(List<T> cards, string fileName)
    {
        CardListWrapper<T> wrapper = new CardListWrapper<T> { cards = cards };
        string json = JsonConvert.SerializeObject(wrapper);
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("JSON saved to: " + filePath);
    }

    public static List<T> LoadCardsFromJson<T>(string fileName)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            CardListWrapper<T> wrapper = JsonConvert.DeserializeObject<CardListWrapper<T>>(json);
            return wrapper.cards;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
}
