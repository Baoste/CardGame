using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCardDeckEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<GetCardDeckEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        CardDatabase.InitFromString(payload.pointCardDeckJson, payload.skillCardDeckJson);

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=GetCardDeck");

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield break;
    }
}