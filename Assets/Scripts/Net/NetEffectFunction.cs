using Game.Server;

namespace Game.Domain
{
    public static class NetEffectFunction
    {
        public static void EndExecuteSkill(int playerId, int instanceId, MatchSession session, CommandResult results)
        {
            session.gameState.ClearResolve();
            results.events.Enqueue(CommandHandler.MakeEvent(
                "ClearCardsToResolve",
                new ClearCardsToResolveEvent
                (
                    playerId,
                    true,
                    false
                ),
                -1
            ));

            bool success = session.gameState.RemoveCard(instanceId);
            results.events.Enqueue(CommandHandler.MakeEvent(
                "DiscardCard",
                new DiscardCardEvent
                (
                    playerId,
                    success,
                    instanceId
                ),
                -1
            ));
        }

        public static bool SpendActionPoint(int playerId, int instanceId, MatchSession session, CommandResult results, int apCount)
        {
            if (session.gameState.players[playerId].actionPoint >= apCount)
            {
                session.gameState.players[playerId].actionPoint -= apCount;
            }
            else
            {
                SendInvalidEvent(playerId, instanceId, results, InvalidActionType.NotEnoughAP);
                return false;
            }

            results.events.Enqueue(CommandHandler.MakeEvent(
                "SpendActionPoint",
                new SpendActionPointEvent    // need change
                (
                    playerId,
                    true
                ),
                -1
            ));
            return true;
        }

        public static void SendInvalidEvent(int playerId, int instanceId, CommandResult results, InvalidActionType type)
        {
            results.events.Enqueue(CommandHandler.MakeEvent(
                "InvalidAction",
                new InvalidActionEvent    // need change
                (
                    playerId,
                    type,
                    instanceId
                ),
                playerId
            ));
        }
    }
}