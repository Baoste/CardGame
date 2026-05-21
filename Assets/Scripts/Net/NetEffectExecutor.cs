using Game.Server;
using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public static class NetEffectExecutor
    {
        public static void ExecuteOp(int playerId, EffectOp op, MatchSession session, CommandResult results, 
            List<int> selectedSourceIds, List<int> selectedTargetIds)
        {
            switch (op.type)
            {
                case EffectType.DrawPoint:
                    DrawPointCards(playerId, op, session, results, selectedTargetIds);
                    break;

                case EffectType.DrawSkill:
                    DrawSkillCards(playerId, op, session, results);
                    break;

                case EffectType.DrawPointToResolve:
                    DrawPointCardsToResolve(playerId, op, session, results);
                    break;

                case EffectType.Discard:
                    DiscardCards(playerId, op, session, results, selectedTargetIds);
                    break;

                case EffectType.ModifyPoint:
                    ModifyPoint(playerId, op, session, results, selectedTargetIds);
                    break;

                case EffectType.Move:
                    MoveCards(playerId, op, session, results, selectedSourceIds, selectedTargetIds);
                    break;

                case EffectType.AddActionPoint:
                    AddActionPoint(playerId, op, session, results);
                    break;

                case EffectType.Peek:
                    PeekTopCards(playerId, op, session, results);
                    break;

                case EffectType.ChangeCardState:
                    ChangeCardsState(playerId, op, session, results, selectedTargetIds);
                    break;

                case EffectType.Judge:
                    // 这个效果不需要执行
                    break;
            }
        }

        private static void DrawPointCards(int playerId, EffectOp op, MatchSession session, CommandResult results, List<int> selectedTargetIds)
        {
            int drawNum = op.value.Evaluate(session.gameState, session.ctx);

            ParticipantType participantType;
            if (selectedTargetIds.Count == 0)
            {
                participantType = op.target.participantType;
            }
            else
            {
                participantType = (ParticipantType)selectedTargetIds[0];
            }

            int casterId = playerId;
            if (participantType == ParticipantType.OpponentPointCardsOnBoard || participantType == ParticipantType.OppentBoardZone)
                casterId = 1 - playerId;

            for (int i = 0; i < drawNum; i++)
            {
                int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
                CardVisualState cardState = session.gameState.GetCardState(drawCardInstanceId);
                session.gameState.AddCard(casterId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Point, cardState);

                // return
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "DrawPointCard",
                    new DrawPointCardEvent    // need change
                    (
                        casterId,
                        drawCardInstanceId != -1,
                        session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                        drawCardInstanceId,
                        cardState,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void DrawSkillCards(int playerId, EffectOp op, MatchSession session, CommandResult results)
        {
            int drawNum = op.value.Evaluate(session.gameState, session.ctx);

            ParticipantType participantType = op.target.participantType;
            int casterId = playerId;
            // 根据op参数区分抽到不同的玩家手里
            if (participantType == ParticipantType.OpponentSkillCardsInHand)
                casterId = 1 - playerId;

            for (int i = 0; i < drawNum; i++)
            {
                int drawCardInstanceId = session.gameState.skillCardsDeck.Draw();
                session.gameState.AddCard(casterId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Skill);

                // return
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "DrawSkillCard",
                    new DrawSkillCardEvent    // need change
                    (
                        casterId,
                        drawCardInstanceId != -1,
                        session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                        drawCardInstanceId,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void DrawPointCardsToResolve(int playerId, EffectOp op, MatchSession session, CommandResult results)
        {
            // TODO: change to op.value
            int count = op.source.maxSelectCount.Evaluate(session.gameState, session.ctx);
            for (int i = 0; i < count; i++)
            {
                int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
                int drawCardId = session.instanceToCardId[drawCardInstanceId];
                session.gameState.AddToResolve(drawCardId, drawCardInstanceId);

                results.events.Enqueue(CommandHandler.MakeEvent(
                    "DrawPointCardToResolve",
                    new DrawPointCardToResolveEvent    // need change
                    (
                        playerId,
                        drawCardInstanceId != -1,
                        drawCardId,
                        drawCardInstanceId,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void DiscardCards(int playerId, EffectOp op, MatchSession session, CommandResult results, List<int> selectedTargetIds)
        {
            bool success = true;
            for (int i = 0; i < selectedTargetIds.Count; i++)
            {
                int cardInstanceId = selectedTargetIds[i];
                success = success && session.gameState.RemoveCard(cardInstanceId);
            }

            // return
            results.events.Enqueue(CommandHandler.MakeEvent(
                "DiscardCard",
                new DiscardCardEvent    // need change
                (
                    playerId,
                    success,
                    selectedTargetIds,
                    op.effectAnimationType
                ),
                -1
            ));
        }

        private static void ModifyPoint(int playerId, EffectOp op, MatchSession session, CommandResult results, List<int> selectedTargetIds)
        {
            int pointChange = op.value.Evaluate(session.gameState, session.ctx);
            int targeValue = -1;
            for (int i = 0; i < selectedTargetIds.Count; i++)
            {
                bool success = session.gameState.instancePointMap.TryGetValue(selectedTargetIds[i], out int value);
                if (success)
                {
                    targeValue = Math.Clamp(value + pointChange, 1, 10);
                    session.gameState.instancePointMap[selectedTargetIds[i]] = targeValue;
                }

                // return
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "ModifyPoint",
                    new ModifyPointEvent    // need change
                    (
                        playerId,
                        success,
                        selectedTargetIds[i],
                        targeValue,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void MoveCards(int playerId, EffectOp op, MatchSession session, CommandResult results, List<int> selectedSourceIds, List<int> selectedTargetIds)
        {
            ParticipantType selectZone;
            if (selectedTargetIds.Count == 0)
            {
                selectZone = op.target.participantType;
            }
            else
            {
                selectZone = (ParticipantType)selectedTargetIds[0];
            }

            for (int i = 0; i < selectedSourceIds.Count; i++)
            {
                CardZone toZone = null;
                switch (selectZone)
                {
                    case ParticipantType.MySkillCardsInHand:
                        toZone = session.gameState.players[playerId].skillCardsInHand;
                        break;
                    case ParticipantType.OpponentSkillCardsInHand:
                        toZone = session.gameState.players[1 - playerId].skillCardsInHand;
                        break;
                    case ParticipantType.MyPointCardsOnBoard:
                        toZone = session.gameState.players[playerId].pointCardsOnBoard;
                        break;
                    case ParticipantType.OpponentPointCardsOnBoard:
                        toZone = session.gameState.players[1 - playerId].pointCardsOnBoard;
                        break;
                    case ParticipantType.SkillCardsInDeck:
                        toZone = session.gameState.skillCardsDeck;
                        break;
                    case ParticipantType.PointCardsInDeck:
                        toZone = session.gameState.pointCardsDeck;
                        break;
                    case ParticipantType.MyBoardZone:
                        toZone = session.gameState.players[playerId].pointCardsOnBoard;
                        break;
                    case ParticipantType.OppentBoardZone:
                        toZone = session.gameState.players[1 - playerId].pointCardsOnBoard;
                        break;
                }
                bool success = session.gameState.MoveCard(selectedSourceIds[i], toZone);
                int cardId = session.instanceToCardId[selectedSourceIds[i]];

                // return
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "MoveCard",
                    new MoveCardEvent    // need change
                    (
                        playerId,
                        success,
                        cardId,
                        selectedSourceIds[i],
                        selectZone,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void AddActionPoint(int playerId, EffectOp op, MatchSession session, CommandResult results)
        {
            int apCount = op.value.Evaluate(session.gameState, session.ctx);

            session.gameState.players[playerId].actionPoint = Math.Max(3, session.gameState.players[playerId].actionPoint + apCount);

            // return results
            results.events.Enqueue(CommandHandler.MakeEvent(
                "AddActionPoint",
                new AddActionPointEvent    // need change
                (
                    playerId,
                    true,
                    apCount,
                    false
                ),
                -1
            ));
        }

        private static void PeekTopCards(int playerId, EffectOp op, MatchSession session, CommandResult results)
        {
            int count = op.value.Evaluate(session.gameState, session.ctx);
            for (int i = 0; i < count; i++)
            {
                int drawCardInstanceId = session.gameState.pointCardsDeck.Peek(i);
                int drawCardId = session.instanceToCardId[drawCardInstanceId];

                results.events.Enqueue(CommandHandler.MakeEvent(
                    "PeekTopCard",
                    new PeekTopCardEvent    // need change
                    (
                        playerId,
                        true,
                        drawCardId,
                        drawCardInstanceId,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }

        private static void ChangeCardsState(int playerId, EffectOp op, MatchSession session, CommandResult results, List<int> selectedTargetIds)
        {
            //List<int> ids = ClientEffectContext.Instance.selectedTargetIds;
            int count = selectedTargetIds.Count;
            CardVisualState cardState = (CardVisualState)op.value.Evaluate(session.gameState, session.ctx);
            for (int i = 0; i < count; i++)
            {
                bool success = session.gameState.SetCardState(
                    selectedTargetIds[i],
                    cardState
                );

                results.events.Enqueue(CommandHandler.MakeEvent(
                    "ChangeCardState",
                    new ChangeCardStateEvent    // need change
                    (
                        playerId,
                        success,
                        selectedTargetIds[i],
                        cardState,
                        op.effectAnimationType
                    ),
                    -1
                ));
            }
        }
    }
}
