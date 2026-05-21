using FishNet.Managing;
using Game.Domain;
using Newtonsoft.Json;
using UnityEngine;

public class QuickStartButtons : MonoBehaviour
{
    public NetworkManager nm;
    private string matchId = "123";
    // private string ClientIP = "49.232.222.222";
    private string ClientIP = "localhost";
    [HideInInspector] public string matchSeed = "12345";

    const int w = 200;
    const int h = 40;

    private Rect ScaleRect(float x, float y, float w, float h)
    {
        float baseW = 1920f;
        float baseH = 1080f;

        float scaleX = Screen.width / baseW;
        float scaleY = Screen.height / baseH;

        return new Rect(
            x * scaleX,
            y * scaleY,
            w * scaleX,
            h * scaleY
        );
    }

    private void OnGUI()
    {
        if (nm == null)
        {
            //if (GUI.Button(ScaleRect(1700, 10, w, h), "Start Match"))
            //{
            //    int seed = MatchData.Instance.matchSeed;
            //    ClientCommand.StartMatch(seed);
            //}
            return;
        }


        if (GUI.Button(ScaleRect(100, 10, w, h), "Start Server"))
            nm.ServerManager.StartConnection();

        if (GUI.Button(ScaleRect(100, 60, w, h), "Start Client"))
        {
            nm.TransportManager.Transport.SetClientAddress(ClientIP);
            nm.ClientManager.StartConnection();
        }

        if (GUI.Button(ScaleRect(100, 110, w, h), "Stop All"))
        {
            nm.ClientManager.StopConnection();
            nm.ServerManager.StopConnection(true);
        }

        matchId = GUI.TextField(ScaleRect(100, 160, w, h), matchId);
        matchSeed = GUI.TextField(ScaleRect(100, 210, w, h), matchSeed);
        ClientIP = GUI.TextField(ScaleRect(100, 260, w, h), ClientIP);

        // These buttons are for testing the ClientCommand methods. They will not work without a server and client connection.
        if (GUI.Button(ScaleRect(1700, 10, w, h), "Create / Join Match"))
        {
            ClientCommand.CreateMatch(matchId);
            GetCardDeckCommand cmd = new GetCardDeckCommand { playerId = -1 };
            ClientGameState.gateway.SendCommandServerRpc("GetCardDeck", JsonConvert.SerializeObject(cmd));
        }

        //if (GUI.Button(ScaleRect(1700, 60, w, h), "Join Match"))
        //    ClientCommand.JoinMatch(matchId);

        if (GUI.Button(ScaleRect(1700, 60, w, h), "Leave Match"))
            ClientCommand.LeaveMatch();

        if (GUI.Button(ScaleRect(1700, 160, w, h), "Chat"))
            ClientCommand.Chat("Hello");

        if (GUI.Button(ScaleRect(1700, 210, w, h), "Start Match"))
        {
            int seed = int.Parse(matchSeed);
            ClientCommand.StartMatch(seed);
        }

        //if (ClientGameState.playerSlot == ClientGameState.Instance.CurrentPlayerId)
        //{
        //    if (GUI.Button(new Rect(1700, 610, w, h), "End Turn"))
        //    {
        //        ClientCommand.EndTurn();
        //        ClientCommand.StartTurn(1 - ClientGameState.playerSlot);
        //    }

        //if (GUI.Button(new Rect(1700, 660, w, h), "Reveal Cards"))
        //{
        //    ClientCommand.RevealCardsAndScore();
        //}
        //}

        //if (GUI.Button(new Rect(1700, 360, w, h), "S: Opponent draw"))
        //{
        //    Card tmp = CardDatabase.Get(999);
        //    StartCoroutine(ClientEffectExecutor._StartExecuteChip(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}

        //if (GUI.Button(new Rect(1700, 410, w, h), "S: Discard"))
        //{
        //    Card tmp = CardDatabase.Get(9999);
        //    StartCoroutine(ClientEffectExecutor._StartExecuteChip(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}

        //if (GUI.Button(new Rect(1700, 460, w, h), "S: Point - 1"))
        //{
        //    Card tmp = CardDatabase.Get(99999);
        //    StartCoroutine(ClientEffectExecutor._StartExecuteChip(tmp, ClientGameState.gateway, ClientGameState.playerSlot, -1));
        //}
    }
}