using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public class CommandHandler
{
    public static ResolvedEvent MakeEvent<T>(string type, T payload, int sendId)
    {
        return new ResolvedEvent
        {
            type = type,
            jsonData = JsonConvert.SerializeObject(payload),
            sendId = sendId
        };
    }
}
