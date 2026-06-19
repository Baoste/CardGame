using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EmojiEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<EmojiEvent>(ev.jsonData);

        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.emojiId }, 0);

        string context = payload.emojiId.ToString();
        Debug.Log($"[Client]Event#{ev.Index} type = {ev.type} slot = {payload.playerId} payload = {context}");

        return true;
    }

    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("EmojiTest", objects);
    }
}
