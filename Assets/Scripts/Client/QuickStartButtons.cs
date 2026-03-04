using FishNet.Managing;
using Game.Domain;
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

        if (GUI.Button(new Rect(1700, 110, w, h), "Leave Match"))
            ClientCommand.LeaveMatch();

        if (GUI.Button(new Rect(1700, 160, w, h), "Chat"))
            ClientCommand.Chat("Hello");

        if (GUI.Button(new Rect(1700, 210, w, h), "Start Game"))
        {
            ClientCommand.StartGame();
            ClientCommand.StartTurn(ClientGameState.playerSlot);
        }

        if (GUI.Button(new Rect(1700, 610, w, h), "End Turn"))
        {
            ClientCommand.EndTurn();
            ClientCommand.StartTurn(1 - ClientGameState.playerSlot);
        }

        if (ClientGameState.Instance.CurrentPlayerId != -1 && ClientGameState.Instance.CurrentPlayerId == ClientGameState.playerSlot)
        {
            if (GUI.Button(new Rect(1700, 310, w, h), "Draw Point Card"))
                ClientCommand.DrawPointCard();

            if (GUI.Button(new Rect(1700, 360, w, h), "S: Opponent draw"))
            {
                Card tmp = CardDatabase.Get(999);
                StartCoroutine(ClientEffectExecutor.ExcuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
            }

            if (GUI.Button(new Rect(1700, 410, w, h), "S: Discard"))
            {
                Card tmp = CardDatabase.Get(9999);
                StartCoroutine(ClientEffectExecutor.ExcuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
            }

            if (GUI.Button(new Rect(1700, 460, w, h), "S: Point - 1"))
            {
                Card tmp = CardDatabase.Get(99999);
                StartCoroutine(ClientEffectExecutor.ExcuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
            }
        }
    }
}