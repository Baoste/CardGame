using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class GetCardDeckCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<GetCardDeckCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        CardDatabase.GetValues(out List<PointCard> pointCards, out List<SkillCard> skillCards);
        CardListWrapper<PointCard> pWrapper = new CardListWrapper<PointCard> { cards = pointCards };
        CardListWrapper<SkillCard> sWrapper = new CardListWrapper<SkillCard> { cards = skillCards };
        string pointCardDeckJson = JsonConvert.SerializeObject(pWrapper);
        string skillCardDeckJson = JsonConvert.SerializeObject(sWrapper);
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "GetCardDeck",
            new GetCardDeckEvent
            (
                payload.playerId,
                true,
                pointCardDeckJson,
                skillCardDeckJson
            ),
            -1
        ));
        return results;
    }

    private class CardListWrapper<T>
    {
        public List<T> cards;
    }
}