using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountSceneBootstrap : MonoBehaviour
{
    private NetworkManager nm;

    void Start()
    {
        nm = FindAnyObjectByType<NetworkManager>();
        nm.TransportManager.Transport.SetClientAddress("49.232.222.222");
        //nm.TransportManager.Transport.SetClientAddress("localhost");
        nm.ClientManager.StartConnection();
    }
}
