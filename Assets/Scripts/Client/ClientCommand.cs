using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public static class ClientCommand
{
    // 创建新局
    public static void CreateMatch(string matchId)
    {
        JoinOrCreateMatchCommand cmd = new JoinOrCreateMatchCommand { playerId = -1, matchIdOrEmpty = matchId };
        ClientGameState.gateway.SendCommandServerRpc("JoinOrCreateMatch", JsonConvert.SerializeObject(cmd));
        Debug.Log("[Client] Requested create match");
    }

    public static void JoinMatch(string matchId)
    {
        JoinOrCreateMatchCommand cmd = new JoinOrCreateMatchCommand { playerId = -1, matchIdOrEmpty = matchId };
        ClientGameState.gateway.SendCommandServerRpc("JoinOrCreateMatch", JsonConvert.SerializeObject(cmd));
        Debug.Log($"[Client] Requested join match {matchId}");
    }

    public static void LeaveMatch()
    {
        LeaveMatchCommand cmd = new LeaveMatchCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("LeaveMatch", JsonConvert.SerializeObject(cmd));
    }

    public static void StartMatch(int seed)
    {
        StartMatchCommand cmd = new StartMatchCommand { playerId = ClientGameState.playerSlot, seed = seed };
        ClientGameState.gateway.SendCommandServerRpc("StartMatch", JsonConvert.SerializeObject(cmd));
    }

    public static IEnumerator StartGame()
    {
        StartGameCommand cmd = new StartGameCommand { playerId = ClientGameState.playerSlot};
        ClientGameState.gateway.SendCommandServerRpc("StartGame", JsonConvert.SerializeObject(cmd));
        yield return new WaitForSecondsRealtime(8f);
        AssignRolesCommand cmd2 = new AssignRolesCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("AssignRoles", JsonConvert.SerializeObject(cmd2));
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

    public static void EndTurn()
    {
        EndTurnCommand cmd = new EndTurnCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("EndTurn", JsonConvert.SerializeObject(cmd));
    }

    public static void RevealCardsAndScore()
    {
        RevealCardsAndScoreCommand cmd = new RevealCardsAndScoreCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("RevealCardsAndScore", JsonConvert.SerializeObject(cmd));
    }

    public static void SumPoint()
    {
        SumPointCommand cmd = new SumPointCommand { playerId = ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("SumPoint", JsonConvert.SerializeObject(cmd));
        cmd = new SumPointCommand { playerId = 1 - ClientGameState.playerSlot };
        ClientGameState.gateway.SendCommandServerRpc("SumPoint", JsonConvert.SerializeObject(cmd));
    }
}
