using FishNet.Managing;
using Game.Domain;
using UnityEngine;

public class ServerBootstrap : MonoBehaviour
{
    public NetworkManager networkManager;

    private void Awake()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManager>();

        // Dedicated / batchmode »·¾³
        if (Application.isBatchMode)
        {
            Debug.Log("[Server] Auto start server...");
            networkManager.ServerManager.StartConnection();

            CardDatabase.Init("PointCards.json", CardDatabaseType.PointCard);
            CardDatabase.Init("SkillCardsT.json", CardDatabaseType.SkillCard);
        }
    }
}