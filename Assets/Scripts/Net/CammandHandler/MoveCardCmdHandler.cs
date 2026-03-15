using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MoveCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<MoveCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        CardZone toZone = null;
        switch (payload.toZone)
        {
            case ParticipantType.MySkillCardsInHand:
                toZone = session.gameState.players[payload.playerId].skillCardsInHand;
                break;
            case ParticipantType.OpponentSkillCardsInHand:
                toZone = session.gameState.players[1 - payload.playerId].skillCardsInHand;
                break;
            case ParticipantType.MyPointCardsOnBoard:
                toZone = session.gameState.players[payload.playerId].pointCardsOnBoard;
                break;
            case ParticipantType.OpponentPointCardsOnBoard:
                toZone = session.gameState.players[1 - payload.playerId].pointCardsOnBoard;
                break;
            case ParticipantType.SkillCardsInDeck:
                toZone = session.gameState.skillCardsDeck;
                break;
            case ParticipantType.PointCardsInDeck:
                toZone = session.gameState.pointCardsDeck;
                break;
            case ParticipantType.MyBoardZone:
                toZone = session.gameState.players[payload.playerId].pointCardsOnBoard;
                break;
            case ParticipantType.OppentBoardZone:
                toZone = session.gameState.players[1 - payload.playerId].pointCardsOnBoard;
                break;
        }
        bool success = session.gameState.MoveCard(payload.instanceId, toZone);
        int cardId = session.instanceToCardId[payload.instanceId];

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "MoveCard",
            new MoveCardEvent    // need change
            (
                payload.playerId,
                success,
                cardId,
                payload.instanceId,
                payload.toZone
            )
        ));
        return results;
    }
}