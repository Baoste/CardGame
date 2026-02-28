using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public class StartGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<StartGameCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        List<int> targetIds = new List<int> { 1, 2, 3 };  // TODO: 需要根据技能卡的效果来确定目标

        // return results
        CommandResult results = new CommandResult();
        results.events.Add(MakeEvent(
            "StartGame",
            new StartGameEvent    // need change
            {
                playerId = payload.playerId,
            }
        ));
        results.events.Add(MakeEvent(
            "DrawCard",
            new DrawCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = UnityEngine.Random.Range(0, 25)
            }
        ));
        return results;
    }
}
