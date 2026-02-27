using FishNet.Managing;
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
        }
    }
}