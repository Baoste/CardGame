using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMatchInput : MonoBehaviour
{
    [Header("Persisted (copy from log for quick test)")]
    public string matchId;
    public string token;

    void OnEnable()
    {
        MatchGateway.OnClientGetDeck += OnGetDeck;
        MatchGateway.OnClientJoined += OnJoined;
        MatchGateway.OnClientEvent += OnEvent;
    }

    void OnDisable()
    {
        MatchGateway.OnClientGetDeck -= OnGetDeck;
        MatchGateway.OnClientJoined -= OnJoined;
        MatchGateway.OnClientEvent -= OnEvent;
    }
    void OnGetDeck(string pointCardDeckJson, string skillCardDeckJson)
    {
        CardDatabase.InitFromString(pointCardDeckJson, skillCardDeckJson);
    }


    void OnJoined(string matchId, int slot, string token, string snapshotJson)
    {
        // inputField.text = matchId;
        ClientGameState.playerSlot = slot;
        Debug.Log($"[UI] Joined match {matchId}, slot {slot}");
    }

    void OnEvent(Game.Domain.NetEvent ev)
    {
        if (ev.Index > ClientGameState.lastEventIndex)
            ClientGameState.lastEventIndex = ev.Index;
    }
}
