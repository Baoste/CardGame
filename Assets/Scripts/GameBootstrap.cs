using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private static bool _initialized = false;

    void Awake()
    {
        if (_initialized) return;
        _initialized = true;

        CardDatabase.Init("PointCards.json", CardDatabaseType.PointCard);
        CardDatabase.Init("SkillCardsT.json", CardDatabaseType.SkillCard);

        DispatcherBootstrap.Init();
        Debug.Log("NetEvent ×¢²áÍê³É");
    }
}
