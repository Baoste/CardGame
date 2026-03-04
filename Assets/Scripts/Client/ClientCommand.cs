using Game.Domain;
using Newtonsoft.Json;
using UnityEngine;

public static class ClientCommand
{
    // 创建新局
    public static void CreateMatch()
    {
        JoinOrCreateGameCommand cmd = new JoinOrCreateGameCommand { playerId = -1, matchIdOrEmpty = "123" };
        ClientGameState.gateway.SendCommandServerRpc("JoinOrCreateGame", JsonConvert.SerializeObject(cmd));
        Debug.Log("[Client] Requested create match");
    }

    public static void JoinMatch(string matchId)
    {
        JoinOrCreateGameCommand cmd = new JoinOrCreateGameCommand { playerId = -1, matchIdOrEmpty = matchId };
        ClientGameState.gateway.SendCommandServerRpc("JoinOrCreateGame", JsonConvert.SerializeObject(cmd));
        Debug.Log($"[Client] Requested join match {matchId}");
    }

    public static void LeaveMatch()
    {
        LeaveGameCommand cmd = new LeaveGameCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("LeaveGame", JsonConvert.SerializeObject(cmd));
    }

    public static void StartGame()
    {
        StartGameCommand cmd = new StartGameCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("StartGame", JsonConvert.SerializeObject(cmd));
    }

    public static void StartTurn(int turnPlayerId)
    {
        StartTurnCommand cmd = new StartTurnCommand { playerId = turnPlayerId };
        ClientGameState.gateway.SendCommandServerRpc("StartTurn", JsonConvert.SerializeObject(cmd));
    }

    public static void Chat(string chatContext)
    {
        ChatCommand cmd = new ChatCommand { playerId = ClientGameState.playerSlot, chatContext = chatContext };
        ClientGameState.gateway.SendCommandServerRpc("Chat", JsonConvert.SerializeObject(cmd));
    }

    public static void DrawSkillCard()
    {
        DrawSkillCardCommand cmd = new DrawSkillCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawSkillCard", JsonConvert.SerializeObject(cmd));
    }

    public static void DrawPointCard()
    {
        DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("DrawPointCard", JsonConvert.SerializeObject(cmd));
    }

    public static void EndTurn()
    {
        EndTurnCommand cmd = new EndTurnCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("EndTurn", JsonConvert.SerializeObject(cmd));
    }
}
