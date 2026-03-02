using Game.Domain;
using System.Collections;
using UnityEngine;

public class CommandHandler
{
    public static ResolvedEvent MakeEvent<T>(string type, T payload)
    {
        return new ResolvedEvent
        {
            type = type,
            jsonData = JsonUtility.ToJson(payload)
        };
    }
}
