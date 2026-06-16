using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private static bool _initialized = false;
    public static bool isLogin = false;
    public static bool isDebugMode = true;
    public MatchGateway gateway;

    void Awake()
    {
        if (_initialized) return;
        _initialized = true;

        if (ClientGameState.gateway == null)
            ClientGameState.gateway = gateway;

        if (isDebugMode)
        {
            CardDatabase.Init("PointCards.json", CardDatabaseType.PointCard);
            CardDatabase.Init("SkillCardsT.json", CardDatabaseType.SkillCard);
        }

        DispatcherBootstrap.Init();
        Debug.Log("NetEvent ×¢²áÍê³É");
    }
}
