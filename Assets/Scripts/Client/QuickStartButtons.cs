using FishNet.Managing;
using Game.Domain;
using UnityEngine;

public class QuickStartButtons : MonoBehaviour
{
    public NetworkManager nm;
    private string matchId = "123";
    private string matchSeed = "12345";

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

        matchId = GUI.TextField(new Rect(10, 160, w, h), matchId);
        matchSeed = GUI.TextField(new Rect(10, 210, w, h), matchSeed);

        // These buttons are for testing the ClientCommand methods. They will not work without a server and client connection.
        if (GUI.Button(new Rect(1700, 10, w, h), "Create / Join Match"))
            ClientCommand.CreateMatch(matchId);

        //if (GUI.Button(new Rect(1700, 60, w, h), "Join Match"))
        //    ClientCommand.JoinMatch(matchId);

        if (GUI.Button(new Rect(1700, 60, w, h), "Leave Match"))
            ClientCommand.LeaveMatch();

        if (GUI.Button(new Rect(1700, 160, w, h), "Chat"))
            ClientCommand.Chat("Hello");

        if (GUI.Button(new Rect(1700, 210, w, h), "Start Game"))
        {
            int seed = int.Parse(matchSeed);
            ClientCommand.StartGame(seed);
        }

        if (ClientGameState.playerSlot == ClientGameState.Instance.CurrentPlayerId)
        {
            if (GUI.Button(new Rect(1700, 610, w, h), "End Turn"))
            {
                ClientCommand.EndTurn();
                ClientCommand.StartTurn(1 - ClientGameState.playerSlot);
            }

            //if (GUI.Button(new Rect(1700, 660, w, h), "Reveal Cards"))
            //{
            //    ClientCommand.RevealCardsAndScore();
            //}
        }

        //if (GUI.Button(new Rect(1700, 360, w, h), "S: Opponent draw"))
        //{
        //    Card tmp = CardDatabase.Get(999);
        //    StartCoroutine(ClientEffectExecutor.ExecuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}

        //if (GUI.Button(new Rect(1700, 410, w, h), "S: Discard"))
        //{
        //    Card tmp = CardDatabase.Get(9999);
        //    StartCoroutine(ClientEffectExecutor.ExecuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}

        //if (GUI.Button(new Rect(1700, 460, w, h), "S: Point - 1"))
        //{
        //    Card tmp = CardDatabase.Get(99999);
        //    StartCoroutine(ClientEffectExecutor.ExecuteCard(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}
    }
}