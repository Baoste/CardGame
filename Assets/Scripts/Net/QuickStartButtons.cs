using FishNet.Managing;
using UnityEngine;

public class QuickStartButtons : MonoBehaviour
{
    public NetworkManager nm;

    private void OnGUI()
    {
        if (nm == null) return;

        const int w = 200;
        const int h = 40;

        if (GUI.Button(new Rect(10, 10, w, h), "Start Server"))
            nm.ServerManager.StartConnection();

        if (GUI.Button(new Rect(10, 60, w, h), "Start Client"))
            nm.ClientManager.StartConnection();

        if (GUI.Button(new Rect(10, 110, w, h), "Stop All"))
        {
            nm.ClientManager.StopConnection();
            nm.ServerManager.StopConnection(true);
        }

        // These buttons are for testing the ClientCommand methods. They will not work without a server and client connection.
        if (GUI.Button(new Rect(1700, 10, w, h), "Create Match"))
            ClientCommand.CreateMatch();

        if (GUI.Button(new Rect(1700, 60, w, h), "Join Match"))
            ClientCommand.JoinMatch("123");

        if (GUI.Button(new Rect(1700, 110, w, h), "Chat"))
            ClientCommand.Chat("Hello");

        if (GUI.Button(new Rect(1700, 160, w, h), "Start Game"))
            ClientCommand.StartGame();

        if (GUI.Button(new Rect(1700, 210, w, h), "Draw Point Card"))
            ClientCommand.DrawPointCard();
    }
}