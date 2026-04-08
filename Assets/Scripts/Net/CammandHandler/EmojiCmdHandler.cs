using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using UnityEngine;

public sealed class EmojiCmdHandler : CommandHandler, ICommandHandler 
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<EmojiCommand>(cmd.jsonData);

        int emojiId = payload.emojiId;

        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "Emoji",
            new EmojiEvent
            (
                payload.playerId,
                true,
                emojiId
            ),
            -1
        ));

        return results;
    }
}
