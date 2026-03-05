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
        switch (payload.effect.target.participantType)
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
            case ParticipantType.CardsToResolve:
                toZone = session.gameState.cardsToResolve;
                break;
        }
        session.gameState.MoveCard(payload.instanceId, toZone);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "MoveCards",
            new MoveCardEvent    // need change
            {
                playerId = payload.playerId,
                selectedId = payload.instanceId,
                toZone = payload.effect.target.participantType
            }
        ));
        return results;
    }
}